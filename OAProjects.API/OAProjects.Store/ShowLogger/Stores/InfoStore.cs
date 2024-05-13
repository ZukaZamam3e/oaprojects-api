using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Linq.Expressions;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Client;
using TMDbLib.Objects.General;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.Search;
using TMDbLib.Objects.TvShows;

namespace OAProjects.Store.ShowLogger.Stores;
public class InfoStore : IInfoStore
{
    private readonly ShowLoggerDbContext _context;
    private readonly ApisConfig _apisConfig;

    public InfoStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public async Task<DownloadResultModel> Download(int userId, InfoApiDownloadModel downloadInfo)
    {
        DownloadResultModel result = downloadInfo.API switch
        {
            INFO_API.TMDB_API => await DownloadFromTMDb(downloadInfo),
        };

        return result;
    }

    public async Task<ApiResultModel<IEnumerable<ApiSearchResultModel>>> Search(int userId, InfoApiSearchModel searchInfo)
    {
        ApiResultModel<IEnumerable<ApiSearchResultModel>> result = searchInfo.API switch
        {
            INFO_API.TMDB_API => await SearchFromTMDb(searchInfo),
            _ => throw new NotImplementedException()
        };

        return result;
    }

    public async Task<ApiResultModel<IEnumerable<ApiSearchResultModel>>> SearchFromTMDb(InfoApiSearchModel searchInfo)
    {
        ApiResultModel<IEnumerable<ApiSearchResultModel>> model = new ApiResultModel<IEnumerable<ApiSearchResultModel>>
        {
            ApiResultContents = new List<ApiSearchResultModel>(),
            Result = ApiResults.Success
        };

        IEnumerable<ApiSearchResultModel> query = new List<ApiSearchResultModel>();

        if (string.IsNullOrEmpty(searchInfo.Name))
        {
            model.Result = ApiResults.SearchNameMissing;
            return model;
        }
        else if (searchInfo.Name.Length < 3)
        {
            model.Result = ApiResults.SearchNameTooShort;
            return model;
        }


        if (!string.IsNullOrEmpty(_apisConfig.TMDbAPIKey))
        {
            TMDbClient client = new TMDbClient(_apisConfig.TMDbAPIKey);

            switch (searchInfo.Type)
            {
                case INFO_TYPE.TV:
                    {
                        SearchContainer<SearchTv> searchContainer = await client.SearchTvShowAsync(searchInfo.Name);

                        model.ApiResultContents = searchContainer.Results.Select(m => new ApiSearchResultModel
                        {
                            Api = INFO_API.TMDB_API,
                            Type = INFO_TYPE.TV,
                            Id = m.Id.ToString(),
                            Name = m.Name,
                            AirDate = m.FirstAirDate,
                            Link = $"{_apisConfig.TMDbURL}{TMDBApiPaths.TV}{m.Id}",
                            ImageUrl = !string.IsNullOrEmpty(m.PosterPath) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{m.PosterPath}" : ""
                        });

                        break;
                    }

                case INFO_TYPE.MOVIE:
                    {
                        SearchContainer<SearchMovie> searchContainer = await client.SearchMovieAsync(searchInfo.Name);

                        model.ApiResultContents = searchContainer.Results.Select(m => new ApiSearchResultModel
                        {
                            Api = INFO_API.TMDB_API,
                            Type = INFO_TYPE.MOVIE,
                            Id = m.Id.ToString(),
                            Name = m.Title,
                            AirDate = m.ReleaseDate,
                            Link = $"{_apisConfig.TMDbURL}{TMDBApiPaths.Movie}{m.Id}",
                            ImageUrl = !string.IsNullOrEmpty(m.PosterPath) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{m.PosterPath}" : ""
                        });

                        break;
                    }
            }
        }
        else
        {
            model.Result = ApiResults.TMDBApiKeyMissing;
        }

        return model;
    }

    private async Task<DownloadResultModel> DownloadFromTMDb(InfoApiDownloadModel downloadInfo)
    {
        DownloadResultModel downloadResult = new DownloadResultModel
        {
            Result = ApiResults.Success,
            API = INFO_API.TMDB_API,
            Type = downloadInfo.Type,
            Id = -1
        };

        if (!string.IsNullOrEmpty(_apisConfig.TMDbAPIKey))
        {
            TMDbClient client = new TMDbClient(_apisConfig.TMDbAPIKey);

            switch (downloadInfo.Type)
            {
                case INFO_TYPE.TV:
                    {
                        TvShow show = await client.GetTvShowAsync(int.Parse(downloadInfo.Id));
                        TvInfoModel info = new TvInfoModel();

                        info.ShowName = show.Name;
                        info.ShowOverview = show.Overview.Substring(0, show.Overview.Length > 4000 ? 4000 : show.Overview.Length);
                        info.ApiType = (int)INFO_API.TMDB_API;
                        info.ApiId = show.Id.ToString();
                        info.PosterUrl = show.PosterPath;
                        info.BackdropUrl = show.BackdropPath;
                        info.Status = show.Status;

                        List<TvEpisodeInfoModel> episodes = new List<TvEpisodeInfoModel>();

                        int requestCount = 1;
                        Stopwatch requestTimer = Stopwatch.StartNew();

                        foreach (var season in show.Seasons)
                        {
                            requestCount++;
                            TvSeason? seasonData = await client.GetTvSeasonAsync(show.Id, season.SeasonNumber);

                            if (seasonData != null)
                            {
                                episodes.AddRange(seasonData.Episodes.Select(m => new TvEpisodeInfoModel
                                {
                                    ApiType = (int)INFO_API.TMDB_API,
                                    ApiId = m.Id.ToString(),
                                    SeasonName = season.Name,
                                    EpisodeName = m.Name,
                                    SeasonNumber = season.SeasonNumber,
                                    EpisodeNumber = m.EpisodeNumber,
                                    EpisodeOverview = m.Overview.Substring(0, m.Overview.Length > 4000 ? 4000 : m.Overview.Length),
                                    Runtime = m.Runtime,
                                    AirDate = m.AirDate,
                                    ImageUrl = m.StillPath
                                }));
                            }

                            if (requestCount >= 30)
                            {
                                if (requestTimer.Elapsed.Seconds < 1)
                                {
                                    Thread.Sleep(1000);
                                }

                                requestCount = 0;
                                requestTimer.Restart();
                            }
                        }

                        info.Episodes = episodes;

                        downloadResult.Id = UpdateTvInfo(info);
                        break;

                    }

                case INFO_TYPE.MOVIE:
                    {
                        Movie movie = await client.GetMovieAsync(int.Parse(downloadInfo.Id));
                        MovieInfoModel info = new MovieInfoModel
                        {
                            MovieName = movie.Title,
                            MovieOverview = movie.Overview.Substring(0, movie.Overview.Length > 4000 ? 4000 : movie.Overview.Length),
                            ApiType = (int)INFO_API.TMDB_API,
                            ApiId = movie.Id.ToString(),
                            Runtime = movie.Runtime,
                            AirDate = movie.ReleaseDate,
                            PosterURL = movie.PosterPath,
                            BackdropURL = movie.BackdropPath,
                        };

                        downloadResult.Id = UpdateMovieInfo(info);

                        break;
                    }
            }
        }
        else
        {
            downloadResult.Result = ApiResults.TMDBApiKeyMissing;
        }

        return downloadResult;
    }

    public IEnumerable<TvInfoModel> GetTvInfos(Expression<Func<TvInfoModel, bool>>? predicate)
    {
        IEnumerable<TvInfoModel> query = _context.SL_TV_INFO.Select(m => new TvInfoModel
        {
            TvInfoId = m.TV_INFO_ID,
            ShowName = m.SHOW_NAME,
            ShowOverview = m.SHOW_OVERVIEW,
            ApiType = (int)INFO_API.TMDB_API,
            ApiId = m.API_ID,
            LastDataRefresh = m.LAST_DATA_REFRESH,
            LastUpdated = m.LAST_UPDATED,
            PosterUrl = !string.IsNullOrEmpty(m.POSTER_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{m.POSTER_URL}" : "",
            Status = m.STATUS
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public IEnumerable<TvInfoSeasonModel> GetTvInfoSeasons(int tvInfoId)
    {
        IEnumerable<TvInfoSeasonModel> query = _context.SL_TV_EPISODE_INFO
            .Where(m => m.TV_INFO_ID == tvInfoId)
            .GroupBy(m => new { m.TV_INFO_ID, m.SEASON_NAME, m.SEASON_NUMBER })
            .Select(m => new TvInfoSeasonModel
            {
                TvInfoId = m.Key.TV_INFO_ID,
                SeasonNumber = m.Key.SEASON_NUMBER ?? -1,
                SeasonName = m.Key.SEASON_NAME ?? "",
                EpisodeCount = m.Count()
            });

        return query;
    }

    public IEnumerable<TvEpisodeInfoModel> GetTvEpisodeInfos(Expression<Func<TvEpisodeInfoModel, bool>>? predicate)
    {
        IEnumerable<TvEpisodeInfoModel> query = _context.SL_TV_EPISODE_INFO
            .Select(m => new TvEpisodeInfoModel
            {
                TvEpisodeInfoId = m.TV_EPISODE_INFO_ID,
                TvInfoId = m.TV_INFO_ID,
                SeasonName = m.SEASON_NAME,
                EpisodeName = m.EPISODE_NAME,
                ApiType = (int)INFO_API.TMDB_API,
                ApiId = m.API_ID,
                SeasonNumber = m.SEASON_NUMBER,
                EpisodeNumber = m.EPISODE_NUMBER,
                EpisodeOverview = m.EPISODE_OVERVIEW,
                Runtime = m.RUNTIME,
                AirDate = m.AIR_DATE,
                ImageUrl = !string.IsNullOrEmpty(m.IMAGE_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{m.IMAGE_URL}" : "",
            });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public long UpdateTvInfo(TvInfoModel model)
    {
        SL_TV_INFO? entity = _context.SL_TV_INFO.FirstOrDefault(m => m.API_TYPE == model.ApiType && m.API_ID == model.ApiId);

        if (entity == null)
        {
            entity = new SL_TV_INFO
            {
                API_TYPE = model.ApiType,
                API_ID = model.ApiId,
            };

            _context.SL_TV_INFO.Add(entity);
        }

        entity.SHOW_NAME = model.ShowName;
        entity.SHOW_OVERVIEW = model.ShowOverview;
        entity.POSTER_URL = model.PosterUrl;
        entity.BACKDROP_URL = model.BackdropUrl;
        entity.STATUS = model.Status;

        entity.LAST_DATA_REFRESH = DateTime.Now;
        entity.LAST_UPDATED = DateTime.Now;

        _context.SaveChanges();
        long id = entity.TV_INFO_ID;

        foreach (TvEpisodeInfoModel episode in model.Episodes)
        {
            SL_TV_EPISODE_INFO? episodeEntity = _context.SL_TV_EPISODE_INFO.FirstOrDefault(m => m.API_TYPE == model.ApiType && m.API_ID == episode.ApiId);

            if (episodeEntity == null)
            {
                episodeEntity = new SL_TV_EPISODE_INFO
                {
                    TV_INFO_ID = entity.TV_INFO_ID,
                    API_TYPE = model.ApiType,
                    API_ID = episode.ApiId,
                };

                _context.SL_TV_EPISODE_INFO.Add(episodeEntity);
            }

            episodeEntity.SEASON_NUMBER = episode.SeasonNumber;
            episodeEntity.EPISODE_NUMBER = episode.EpisodeNumber;

            episodeEntity.EPISODE_NAME = episode.EpisodeName;
            episodeEntity.EPISODE_OVERVIEW = episode.EpisodeOverview;

            episodeEntity.SEASON_NAME = episode.SeasonName;
            episodeEntity.EPISODE_NAME = episode.EpisodeName;

            episodeEntity.RUNTIME = episode.Runtime;
            episodeEntity.AIR_DATE = episode.AirDate;
            episodeEntity.IMAGE_URL = episode.ImageUrl;
        }

        entity.LAST_DATA_REFRESH = DateTime.Now;
        entity.LAST_UPDATED = DateTime.Now;
        _context.SaveChanges();

        return id;
    }

    public long RefreshTvInfo(int infoId)
    {
        //TvInfoModel model = GetTvInfos(m => m.TvInfoId == infoId).First();

        //DownloadResultModel downloadModel = await Download(-1, new InfoApiDownloadModel
        //{
        //    API = (INFO_API)tv.ApiType,
        //    Type = INFO_TYPE.TV,
        //    Id = tv.ApiId,
        //});

        //return result;
        return -1;
    }

    public bool DeleteTvInfo(int userId, int tvInfoId)
    {
        SL_TV_INFO entity = _context.SL_TV_INFO.FirstOrDefault(m => m.TV_INFO_ID == tvInfoId);

        if (entity != null)
        {
            IEnumerable<SL_TV_EPISODE_INFO> episodeEntities = _context.SL_TV_EPISODE_INFO.Where(m => m.TV_INFO_ID == tvInfoId);

            int[] episodeIds = episodeEntities.Select(m => m.TV_EPISODE_INFO_ID).ToArray();

            IEnumerable<SL_SHOW> showEntities = _context.SL_SHOW.Where(m => m.SHOW_TYPE_ID == (int)CodeValueIds.TV && m.INFO_ID != null && episodeIds.Contains(m.INFO_ID.Value));

            foreach (SL_SHOW show in showEntities)
            {
                show.INFO_ID = null;
            }

            _context.SL_TV_EPISODE_INFO.RemoveRange(episodeEntities);
            _context.SL_TV_INFO.Remove(entity);
            _context.SaveChanges();

            return true;
        }

        return false;
    }

    public IEnumerable<MovieInfoModel> GetMovieInfos(Expression<Func<MovieInfoModel, bool>>? predicate = null)
    {
        IEnumerable<MovieInfoModel> query = _context.SL_MOVIE_INFO.Select(m => new MovieInfoModel
        {
            MovieInfoId = m.MOVIE_INFO_ID,
            MovieName = m.MOVIE_NAME,
            MovieOverview = m.MOVIE_OVERVIEW,
            ApiType = m.API_TYPE,
            ApiId = m.API_ID,
            Runtime = m.RUNTIME,
            AirDate = m.AIR_DATE,
            PosterURL = m.POSTER_URL,
            LastDataRefresh = m.LAST_DATA_REFRESH,
            LastUpdated = m.LAST_UPDATED,
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public long UpdateMovieInfo(MovieInfoModel model)
    {
        SL_MOVIE_INFO? entity = _context.SL_MOVIE_INFO.FirstOrDefault(m => m.API_TYPE == model.ApiType && m.API_ID == model.ApiId);

        if (entity == null)
        {
            entity = new SL_MOVIE_INFO();
            _context.SL_MOVIE_INFO.Add(entity);
        }

        entity.API_TYPE = model.ApiType;
        entity.API_ID = model.ApiId;

        entity.MOVIE_NAME = model.MovieName;
        entity.MOVIE_OVERVIEW = model.MovieOverview;

        entity.RUNTIME = model.Runtime;
        entity.AIR_DATE = model.AirDate;

        entity.POSTER_URL = model.PosterURL;
        entity.BACKDROP_URL = model.BackdropURL;

        entity.LAST_DATA_REFRESH = DateTime.Now;
        entity.LAST_UPDATED = DateTime.Now;


        _context.SaveChanges();
        long id = entity.MOVIE_INFO_ID;

        return id;
    }

    public bool DeleteMovieInfo(int userId, int movieInfoId)
    {
        SL_MOVIE_INFO entity = _context.SL_MOVIE_INFO.FirstOrDefault(m => m.MOVIE_INFO_ID == movieInfoId);

        int[] codeValueIds = new int[]
        {
            (int)CodeValueIds.MOVIE,
            (int)CodeValueIds.AMC,
        };

        if (entity != null)
        {
            IEnumerable<SL_SHOW> showEntities = _context.SL_SHOW.Where(m => codeValueIds.Contains(m.SHOW_TYPE_ID) && m.INFO_ID == entity.MOVIE_INFO_ID);

            foreach (SL_SHOW show in showEntities)
            {
                show.INFO_ID = null;
            }

            _context.SL_MOVIE_INFO.Remove(entity);
            _context.SaveChanges();

            return true;
        }

        return false;
    }

    public async Task<DownloadResultModel> RefreshInfo(int userId, int infoId, INFO_TYPE type)
    {
        InfoApiDownloadModel apiDownloadModel = new InfoApiDownloadModel();

        switch (type)
        {
            case INFO_TYPE.TV:
                {
                    TvEpisodeInfoModel episode = GetTvEpisodeInfos(m => m.TvEpisodeInfoId == infoId).First();
                    TvInfoModel model = GetTvInfos(m => m.TvInfoId == episode.TvInfoId).First();

                    UpdateTvInfo(model);

                    apiDownloadModel = new InfoApiDownloadModel
                    {
                        API = (INFO_API)model.ApiType,
                        Type = INFO_TYPE.MOVIE,
                        Id = model.ApiId
                    };

                    break;
                }

            case INFO_TYPE.MOVIE:
                {
                    MovieInfoModel model = GetMovieInfos(m => m.MovieInfoId == infoId).First();
                    
                    apiDownloadModel = new InfoApiDownloadModel
                    {
                        API = (INFO_API)model.ApiType,
                        Type = INFO_TYPE.MOVIE,
                        Id = model.ApiId
                    };

                    break;
                }
        }

        return await Download(userId, apiDownloadModel);
    }

    public async Task<string[]> RefreshRecurringTvShows()
    {
        DateTime today = DateTime.Today;
        SL_TV_INFO[] tvInfos = _context.SL_TV_INFO.Where(m => m.STATUS == "Returning Series" && m.LAST_DATA_REFRESH < today).ToArray();

        foreach(SL_TV_INFO tvInfo in tvInfos)
        {
            await DownloadFromTMDb(new InfoApiDownloadModel
            {
                API = INFO_API.TMDB_API,
                Type = INFO_TYPE.TV,
                Id = tvInfo.API_ID
            });
        }

        return tvInfos.Select(m => m.SHOW_NAME).ToArray();
    }
}
