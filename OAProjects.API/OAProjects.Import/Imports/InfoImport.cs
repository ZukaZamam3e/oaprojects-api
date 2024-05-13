using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Import.Config;
using System.Collections.Generic;
using TMDbLib.Client;
using TMDbLib.Objects.Movies;
using TMDbLib.Objects.TvShows;

namespace OAProjects.Import.Imports;

public interface IInfoImport : IImport { };

public class InfoImport : IInfoImport
{
    private readonly ShowLoggerDbContext _context;
    private readonly DataConfig _dataConfig;
    private readonly ApiConfig _apiConfig;

    public InfoImport(ShowLoggerDbContext context,
        DataConfig dataConfig,
        ApiConfig apiConfig)
    {
        _context = context;
        _dataConfig = dataConfig;
        _apiConfig = apiConfig;
    }

    public void RunImport()
    {
        TMDbClient client = new TMDbClient(_apiConfig.TMDbApiKey);

        // SL_TV_INFO

        Console.WriteLine("----------- Info Import Started -----------");
        Console.WriteLine("Importing SL_TV_INFO");

        string tvInfoJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_tv_info));
        IEnumerable<TvInfoImport> tvInfoData = JsonConvert.DeserializeObject<IEnumerable<TvInfoImport>>(tvInfoJson);

        int tvInfoCount = tvInfoData.Count();
        Console.WriteLine($"Items to be imported: {tvInfoCount}");

        for (int i = 0; i < tvInfoCount; i+=100) 
        {
            List<TvInfoImport> data = tvInfoData.Skip(i).Take(100).ToList();

            data.ForEach(m =>
            {
                m.ENTITY = new SL_TV_INFO
                {
                    SHOW_NAME = m.SHOW_NAME,
                    SHOW_OVERVIEW = m.SHOW_OVERVIEW,
                    API_TYPE = m.API_TYPE,
                    API_ID = m.API_ID,
                    LAST_DATA_REFRESH = m.LAST_DATA_REFRESH,
                    LAST_UPDATED = m.LAST_UPDATED,
                    POSTER_URL = m.IMAGE_URL,
                };
            });

            _context.SL_TV_INFO.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.TV_INFO_ID,
                NEW_ID = m.ENTITY.TV_INFO_ID,
                TABLE_ID = (int)TableIds.SL_TV_INFO
            }));

            _context.SaveChanges();
        }

        int[] tvInfoApiIds = _context.SL_TV_INFO.Where(m => string.IsNullOrEmpty(m.BACKDROP_URL) || string.IsNullOrEmpty(m.STATUS)).Select(m => int.Parse(m.API_ID)).ToArray();
        string tvInfoUpdatesJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, "tv_info_updates.json"));
        List<TvInfoUpdate> tvInfoUpdates = JsonConvert.DeserializeObject<List<TvInfoUpdate>>(tvInfoUpdatesJson) ?? new List<TvInfoUpdate>();

        IEnumerable<int> missingTvInfoApiIds = tvInfoApiIds.Where(m => !tvInfoUpdates.Any(n => n.ApiId == m));

        foreach (int apiId in missingTvInfoApiIds)
        {
            TvShow? tvShow = client.GetTvShowAsync(apiId).Result;

            tvInfoUpdates.Add(new TvInfoUpdate
            {
                ApiId = apiId,
                Status = tvShow.Status,
                BackdropUrl = tvShow.BackdropPath
            });

            Thread.Sleep(50);
        }

        tvInfoUpdatesJson = JsonConvert.SerializeObject(tvInfoUpdates, Formatting.Indented);
        File.WriteAllText(Path.Join(_dataConfig.DataFolderPath, "tv_info_updates.json"), tvInfoUpdatesJson);

        string[] tvInfoSqls = tvInfoUpdates.Select(m => string.Format(m.UpdateSql, _dataConfig.ShowLoggerDbName)).ToArray();
        string tvInfoSql = string.Join("\n", tvInfoSqls);

        _context.Database.ExecuteSqlRaw(tvInfoSql);
        _context.SaveChanges();

        int tvInfoImportCount = _context.SL_TV_INFO.Count();
        Console.WriteLine($"Items that were imported: {tvInfoImportCount}");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("Importing SL_TV_EPISODE_INFO");

        string tvEpisodeInfoJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_tv_episode_info));
        IEnumerable<TvEpisodeInfoImport> tvEpisodeInfoData = JsonConvert.DeserializeObject<IEnumerable<TvEpisodeInfoImport>>(tvEpisodeInfoJson);

        int tvEpisodeInfoCount = tvEpisodeInfoData.Count();
        Console.WriteLine($"Items to be imported: {tvEpisodeInfoCount}");

        for (int i = 0; i < tvEpisodeInfoCount; i += 100)
        {
            List<TvEpisodeInfoImport> data = tvEpisodeInfoData.Skip(i).Take(100).ToList();

            int[] oldIds = data.Select(m => m.TV_INFO_ID).ToArray();

            Dictionary<int, int> dictIds = _context.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_TV_INFO && oldIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_TV_EPISODE_INFO
                {
                    TV_INFO_ID = dictIds[m.TV_INFO_ID],
                    SEASON_NAME = m.SEASON_NAME,
                    EPISODE_NAME = m.EPISODE_NAME,
                    SEASON_NUMBER = m.SEASON_NUMBER,
                    EPISODE_NUMBER = m.EPISODE_NUMBER,
                    EPISODE_OVERVIEW = m.EPISODE_OVERVIEW,
                    RUNTIME = m.RUNTIME,
                    AIR_DATE = m.AIR_DATE,
                    IMAGE_URL = m.IMAGE_URL,
                    API_TYPE = m.API_TYPE,
                    API_ID = m.API_ID,
                };
            });

            _context.SL_TV_EPISODE_INFO.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.TV_EPISODE_INFO_ID,
                NEW_ID = m.ENTITY.TV_EPISODE_INFO_ID,
                TABLE_ID = (int)TableIds.SL_TV_EPISODE_INFO
            }));

            _context.SaveChanges();
        }

        int tvEpisodeInfoImportCount = _context.SL_TV_EPISODE_INFO.Count();
        Console.WriteLine($"Items that were imported: {tvEpisodeInfoImportCount}");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("Importing SL_MOVIE_INFO");

        string movieInfoJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_movie_info));
        IEnumerable<MovieInfoImport> movieInfoData = JsonConvert.DeserializeObject<IEnumerable<MovieInfoImport>>(movieInfoJson);

        int movieInfoCount = movieInfoData.Count();

        Console.WriteLine($"Items to be imported: {movieInfoCount}");


        for (int i = 0; i < movieInfoCount; i += 100)
        {
            List<MovieInfoImport> data = movieInfoData.Skip(i).Take(100).ToList();

            data.ForEach(m =>
            {
                m.ENTITY = new SL_MOVIE_INFO
                {
                    MOVIE_NAME = m.MOVIE_NAME,
                    MOVIE_OVERVIEW = m.MOVIE_OVERVIEW,
                    RUNTIME = m.RUNTIME,
                    AIR_DATE = m.AIR_DATE,
                    LAST_DATA_REFRESH = m.LAST_DATA_REFRESH,
                    LAST_UPDATED = m.LAST_UPDATED,
                    POSTER_URL = m.IMAGE_URL,
                    API_TYPE = m.API_TYPE,
                    API_ID = m.API_ID,
                };
            });

            _context.SL_MOVIE_INFO.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.MOVIE_INFO_ID,
                NEW_ID = m.ENTITY.MOVIE_INFO_ID,
                TABLE_ID = (int)TableIds.SL_MOVIE_INFO
            }));

            _context.SaveChanges();
        }

        int[] movieApiIds = _context.SL_MOVIE_INFO.Where(m => string.IsNullOrEmpty(m.BACKDROP_URL) || string.IsNullOrEmpty(m.POSTER_URL)).Select(m => int.Parse(m.API_ID)).ToArray();
        string movieInfoUpdatesJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, "movie_info_updates.json"));
        List<MovieInfoUpdate> movieInfoUpdates = JsonConvert.DeserializeObject<List<MovieInfoUpdate>>(movieInfoUpdatesJson) ?? new List<MovieInfoUpdate>();

        IEnumerable<int> missingApiIds = movieApiIds.Where(m => !movieInfoUpdates.Any(n => n.ApiId == m));

        foreach (int apiId in missingApiIds)
        {
            Movie? movie = client.GetMovieAsync(apiId).Result;

            movieInfoUpdates.Add(new MovieInfoUpdate
            {
                ApiId = apiId,
                PosterUrl = movie.PosterPath,
                BackdropUrl = movie.BackdropPath
            });

            Thread.Sleep(50);
        }

        movieInfoUpdatesJson = JsonConvert.SerializeObject(movieInfoUpdates, Formatting.Indented);
        File.WriteAllText(Path.Join(_dataConfig.DataFolderPath, "movie_info_updates.json"), movieInfoUpdatesJson);

        string[] movieInfoSqls = movieInfoUpdates.Select(m => string.Format(m.UpdateSql, _dataConfig.ShowLoggerDbName)).ToArray();
        string movieInfoSql = string.Join("\n", movieInfoSqls);

        _context.Database.ExecuteSqlRaw(movieInfoSql);
        _context.SaveChanges();

        int movieInfoImportCount = _context.SL_MOVIE_INFO.Count();
        Console.WriteLine($"Items that were imported: {movieInfoImportCount}");

        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("Importing SL_TV_EPISODE_ORDER");

        string episodeOrderJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_tv_episode_order));
        IEnumerable<EpisodeOrderImport> episodeOrderData = JsonConvert.DeserializeObject<IEnumerable<EpisodeOrderImport>>(episodeOrderJson);

        int episodeOrderCount = episodeOrderData.Count();

        Console.WriteLine($"Items to be imported: {episodeOrderCount}");


        for (int i = 0; i < episodeOrderCount; i += 100)
        {
            List<EpisodeOrderImport> data = episodeOrderData.Skip(i).Take(100).ToList();

            int[] oldTvInfoIds = data.Select(m => m.TV_INFO_ID).ToArray();
            int[] oldTvEpoisodeInfoIds = data.Select(m => m.TV_EPISODE_INFO_ID).ToArray();

            Dictionary<int, int> dictTvInfoIds = _context.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_TV_INFO && oldTvInfoIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);
            Dictionary<int, int> dictTvEpisodeInfoIds = _context.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_TV_EPISODE_INFO && oldTvEpoisodeInfoIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_TV_EPISODE_ORDER
                {
                    TV_INFO_ID = dictTvInfoIds[m.TV_INFO_ID],
                    TV_EPISODE_INFO_ID = dictTvEpisodeInfoIds[m.TV_EPISODE_INFO_ID],
                    EPISODE_ORDER = m.EPISODE_ORDER
                };
            });

            _context.SL_TV_EPISODE_ORDER.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.TV_EPISODE_ORDER_ID,
                NEW_ID = m.ENTITY.TV_EPISODE_ORDER_ID,
                TABLE_ID = (int)TableIds.SL_TV_EPISODE_ORDER
            }));

            _context.SaveChanges();
        }

        int episodeOrderImportCount = _context.SL_TV_EPISODE_ORDER.Count();
        Console.WriteLine($"Items that were imported: {episodeOrderImportCount}");

        Console.WriteLine("----------- Info Import Finished -----------");

    }
}

public class TvInfoImport
{
    public int TV_INFO_ID { get; set; }
    public string SHOW_NAME { get; set; }
    public string SHOW_OVERVIEW { get; set; }
    public int API_TYPE { get; set; }
    public string API_ID { get; set; }
    public DateTime LAST_DATA_REFRESH { get; set; }
    public DateTime LAST_UPDATED { get; set; }
    public string IMAGE_URL { get; set; }
    public SL_TV_INFO ENTITY { get; set; }
}

public class TvEpisodeInfoImport
{
    public int TV_EPISODE_INFO_ID { get; set; }
    public int TV_INFO_ID { get; set; }
    public string? SEASON_NAME { get; set; }
    public string? EPISODE_NAME { get; set; }
    public int? SEASON_NUMBER { get; set; }
    public int? EPISODE_NUMBER { get; set; }
    public string? EPISODE_OVERVIEW { get; set; }
    public int? RUNTIME { get; set; }
    public DateTime? AIR_DATE { get; set; }
    public string? IMAGE_URL { get; set; }
    public int API_TYPE { get; set; }
    public string API_ID { get; set; }
    public SL_TV_EPISODE_INFO ENTITY { get; set; }
}

public class MovieInfoImport
{
    public int MOVIE_INFO_ID { get; set; }
    public string MOVIE_NAME { get; set; }
    public string? MOVIE_OVERVIEW { get; set; }
    public int? RUNTIME { get; set; }
    public DateTime? AIR_DATE { get; set; }
    public DateTime LAST_DATA_REFRESH { get; set; }
    public DateTime LAST_UPDATED { get; set; }
    public string? IMAGE_URL { get; set; }
    public int API_TYPE { get; set; }
    public string API_ID { get; set; }
    public SL_MOVIE_INFO ENTITY { get; set; }

}

public class EpisodeOrderImport
{
    public int TV_EPISODE_ORDER_ID { get; set; }
    public int TV_INFO_ID { get; set; }
    public int TV_EPISODE_INFO_ID { get; set; }
    public int EPISODE_ORDER { get; set; }
    public SL_TV_EPISODE_ORDER ENTITY { get; set; }

}

public class TvInfoUpdate
{
    public int ApiId { get; set; }

    public string Status { get; set; }

    public string BackdropUrl { get; set; }

    public string UpdateSql => $"UPDATE [{{0}}].[dbo].[SL_TV_INFO] SET STATUS='{Status}', BACKDROP_URL='{BackdropUrl}' WHERE API_ID={ApiId};";
}

public class MovieInfoUpdate
{
    public int ApiId { get; set; }

    public string PosterUrl { get; set; }

    public string BackdropUrl { get; set; }

    public string UpdateSql => $"UPDATE [{{0}}].[dbo].[SL_MOVIE_INFO] SET POSTER_URL='{PosterUrl}', BACKDROP_URL='{BackdropUrl}' WHERE API_ID={ApiId}";
}
