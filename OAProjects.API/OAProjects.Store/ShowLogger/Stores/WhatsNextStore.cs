using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Book;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Transaction;
using OAProjects.Models.ShowLogger.Models.WatchList;
using OAProjects.Models.ShowLogger.Models.WhatsNext;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using TMDbLib.Objects.TvShows;

namespace OAProjects.Store.ShowLogger.Stores;
public class WhatsNextStore : IWhatsNextStore
{
    private readonly ShowLoggerDbContext _context;
    private readonly ApisConfig _apisConfig;

    public WhatsNextStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public IEnumerable<WhatsNextShowModel> GetWhatsNext(int userId, Expression<Func<WhatsNextShowModel, bool>> predicate)
    {
        DateTime today = DateTime.Now.Date;

        // get subscriptions
        SL_WHATS_NEXT_SUB[] subscriptions = _context.SL_WHATS_NEXT_SUB.Where(m => m.USER_ID == userId).ToArray();


        int[] tvInfoIds = subscriptions.Select(m => m.TV_INFO_ID).ToArray();

        SL_TV_EPISODE_INFO[] episodes = _context.SL_TV_EPISODE_INFO.Include(m => m.TV_INFO).Where(m => tvInfoIds.Contains(m.TV_INFO_ID)).ToArray();

        int[] episodeIds = episodes.Select(m => m.TV_EPISODE_INFO_ID).ToArray();
        int[] watchedEpisodes = _context.SL_SHOW.Where(m => m.USER_ID == userId && m.INFO_ID != null && episodeIds.Contains(m.INFO_ID.Value)).Select(m => m.INFO_ID.Value).ToArray();
        //int[] missingEpisodes = episodeIds.Where(m => !watchedEpisodes.Contains(m)).ToArray();

        //List<WhatsNextShowModel> query = episodes
        //    .Where(m => m.AIR_DATE != null && subscriptions.Any(n => n.TV_INFO_ID == m.TV_INFO_ID && n.SUBSCRIBE_DATE <= m.AIR_DATE) && missingEpisodes.Contains(m.TV_EPISODE_INFO_ID))
        //    .GroupBy(m => new
        //    {
        //        m.TV_INFO_ID,
        //        m.TV_INFO.SHOW_NAME,
        //        m.SEASON_NUMBER,
        //        m.SEASON_NAME
        //    })
        //    .Select(grp => new WhatsNextShowModel
        //    {
        //        TvInfoId = grp.Key.TV_INFO_ID,
        //        ShowName = grp.Key.SHOW_NAME,
        //        SeasonNumber = grp.Key.SEASON_NUMBER,
        //        SeasonName = grp.Key.SEASON_NAME,
        //        AirDate = grp.Min(m => m.AIR_DATE.Value),
        //        Status = grp.Min(episode => episode.AIR_DATE) > today ? "Coming Soon" :
        //                    grp.Max(episode => episode.AIR_DATE) > today ? "Currently Airing" :
        //                    grp.Max(episode => episode.AIR_DATE) <= today ? "Season Ended" : "",
        //        Episodes = grp.Select(episode => new WhatsNextEpisodeModel
        //        {
        //            TvEpisodeInfoId = episode.TV_EPISODE_INFO_ID,
        //            AirDate = episode.AIR_DATE.Value,
        //            SeasonNumber = episode.SEASON_NUMBER,
        //            SeasonName = episode.SEASON_NAME,
        //            EpisodeNumber = episode.EPISODE_NUMBER,
        //            EpisodeName = episode.EPISODE_NAME
        //        }).OrderBy(m => m.AirDate)
        //    }).ToList();

        List<WhatsNextShowModel> query = (from s in subscriptions
                                          join e in episodes on s.TV_INFO_ID equals e.TV_INFO_ID
                                          join ti in _context.SL_TV_INFO on e.TV_INFO_ID equals ti.TV_INFO_ID
                                          where s.SUBSCRIBE_DATE <= e.AIR_DATE
                                             && episodeIds.Contains(e.TV_EPISODE_INFO_ID)
                                          group e by new { s, ti, e.TV_INFO.SHOW_NAME, e.SEASON_NUMBER, e.SEASON_NAME } into grp
                                          select new WhatsNextShowModel
                                          {
                                              WhatsNextSubId = grp.Key.s.WHATS_NEXT_SUB_ID,
                                              TvInfoId = grp.Key.s.TV_INFO_ID,
                                              ShowName = grp.Key.SHOW_NAME,
                                              SeasonNumber = grp.Key.SEASON_NUMBER,
                                              SeasonName = grp.Key.SEASON_NAME,
                                              StartDate = grp.Min(m => m.AIR_DATE.Value),
                                              EndDate = grp.Max(m => m.AIR_DATE.Value),
                                              SeasonStatus = grp.Min(episode => episode.AIR_DATE) > today ? "Coming Soon" :
                                                             grp.Max(episode => episode.AIR_DATE) > today ? "Currently Airing" :
                                                             grp.Max(episode => episode.AIR_DATE) <= today ? "Season Ended" : "",
                                              PosterUrl = !string.IsNullOrEmpty(grp.Key.ti.POSTER_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{grp.Key.ti.POSTER_URL}" : "",
                                              BackdropUrl = !string.IsNullOrEmpty(grp.Key.ti.BACKDROP_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{grp.Key.ti.BACKDROP_URL}" : "",
                                              InfoUrl = _apisConfig.GetTvInfoUrl(grp.Key.ti.API_TYPE, grp.Key.ti.API_ID),
                                              SeasonUrl = _apisConfig.GetTvInfoSeasonUrl(grp.Key.ti.API_TYPE, grp.Key.ti.API_ID, grp.Key.SEASON_NUMBER),
                                              NextAirDate = grp.Where(m => !watchedEpisodes.Contains(m.TV_EPISODE_INFO_ID)).Min(episode => episode.AIR_DATE) > today ? 
                                                grp.Where(m => !watchedEpisodes.Contains(m.TV_EPISODE_INFO_ID)).Min(episode => episode.AIR_DATE) 
                                                : null,
                                              Episodes = grp.Where(m => !watchedEpisodes.Contains(m.TV_EPISODE_INFO_ID)).Select(episode => new WhatsNextEpisodeModel
                                              {
                                                  TvEpisodeInfoId = episode.TV_EPISODE_INFO_ID,
                                                  AirDate = episode.AIR_DATE.Value,
                                                  SeasonNumber = episode.SEASON_NUMBER,
                                                  SeasonName = episode.SEASON_NAME,
                                                  EpisodeNumber = episode.EPISODE_NUMBER,
                                                  EpisodeName = episode.EPISODE_NAME,
                                                  EpisodeOverview = episode.EPISODE_OVERVIEW,
                                                  ImageUrl = !string.IsNullOrEmpty(episode.IMAGE_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{episode.IMAGE_URL}" : "",
                                                  Runtime = episode.RUNTIME
                                              }).OrderBy(m => m.AirDate)
                                          }).ToList();

        query = query.Where(m => m.Episodes.Any()).ToList();

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).ToList();
        }

        return query;
    }

    public IEnumerable<WhatsNextWatchEpisodeModel> GetWhatsNextSubs(Expression<Func<WhatsNextWatchEpisodeModel, bool>>? predicate)
    {
        IEnumerable<WhatsNextWatchEpisodeModel> query = from m in _context.SL_WHATS_NEXT_SUB
                                               join ti in _context.SL_TV_INFO on m.TV_INFO_ID equals ti.TV_INFO_ID
                                               select new WhatsNextWatchEpisodeModel
                                               {
                                                   WhatsNextSubId = m.WHATS_NEXT_SUB_ID,
                                                   UserId = m.USER_ID,
                                                   TvInfoId = m.TV_INFO_ID,
                                                   ShowName = ti.SHOW_NAME,
                                                   SubscribeDate = m.SUBSCRIBE_DATE,
                                                   IncludeSpecials = m.INCLUDE_SPECIALS,
                                                   LastDataRefresh = ti.LAST_DATA_REFRESH,
                                                   PosterUrl = !string.IsNullOrEmpty(ti.POSTER_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{ti.POSTER_URL}" : "",
                                                   BackdropUrl = !string.IsNullOrEmpty(ti.BACKDROP_URL) ? $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{ti.BACKDROP_URL}" : "",
                                                   InfoUrl = _apisConfig.GetTvInfoUrl(ti.API_TYPE, ti.API_ID),
                                                   Status = ti.STATUS
                                               };

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public int CreateWhatsNextSub(int userId, WhatsNextWatchEpisodeModel model)
    {
        SL_WHATS_NEXT_SUB entity = new SL_WHATS_NEXT_SUB
        {
            USER_ID = userId,
            TV_INFO_ID = model.TvInfoId,
            SUBSCRIBE_DATE = model.SubscribeDate,
            INCLUDE_SPECIALS = model.IncludeSpecials,
        };

        _context.SL_WHATS_NEXT_SUB.Add(entity);
        _context.SaveChanges();
        int id = entity.WHATS_NEXT_SUB_ID;

        return id;
    }

    public int UpdateWhatsNextSub(int userId, WhatsNextWatchEpisodeModel model)
    {
        SL_WHATS_NEXT_SUB? entity = _context.SL_WHATS_NEXT_SUB.FirstOrDefault(m => m.WHATS_NEXT_SUB_ID == model.WhatsNextSubId && m.USER_ID == userId);

        int updated = 0;

        if (entity != null)
        {
            entity.SUBSCRIBE_DATE = model.SubscribeDate;
            entity.INCLUDE_SPECIALS = model.IncludeSpecials;

            updated = _context.SaveChanges();
        }

        return updated;
    }

    public bool DeleteWhatsNextSub(int userId, int whatsNextSubId)
    {
        bool result = false;
        SL_WHATS_NEXT_SUB? entity = _context.SL_WHATS_NEXT_SUB.FirstOrDefault(m => m.WHATS_NEXT_SUB_ID == whatsNextSubId && m.USER_ID == userId);

        if (entity != null)
        {
            _context.SL_WHATS_NEXT_SUB.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }

    public int WatchEpisode(int userId, int tvEpisodeInfoId, DateTime dateWatched)
    {
        int newShowId = -1;
        SL_TV_EPISODE_INFO? episodeInfo = _context.SL_TV_EPISODE_INFO.Include(m => m.TV_INFO).First(m => m.TV_EPISODE_INFO_ID == tvEpisodeInfoId);

        if (episodeInfo != null)
        {
            SL_SHOW entity = new SL_SHOW
            {
                USER_ID = userId,
                SHOW_TYPE_ID = (int)CodeValueIds.TV,
                SHOW_NAME = episodeInfo.TV_INFO.SHOW_NAME,
                DATE_WATCHED = dateWatched.Date,
                SEASON_NUMBER = episodeInfo.SEASON_NUMBER,
                EPISODE_NUMBER = episodeInfo.EPISODE_NUMBER,
                INFO_ID = tvEpisodeInfoId
            };

            _context.SL_SHOW.Add(entity);
            _context.SaveChanges();
            newShowId = entity.SHOW_ID;
        }

        return newShowId;
    }
}
