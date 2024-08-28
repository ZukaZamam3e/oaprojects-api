using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Config;
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

    public IEnumerable<WhatsNextModel> GetWhatsNext(int userId)
    {
        DateTime today = DateTime.Now.Date;
        DateTime fourMonthsAgo = today.AddMonths(-4);
        DateTime inSevenDays = today.AddDays(7);
        DateTime inOneMonth = today.AddMonths(1);

        // Get list of coming soon, currently airing, and season ended and the season
        List<WhatsNextModelInfo> latest = (from x in _context.SL_TV_EPISODE_INFO
                                             join tv in _context.SL_TV_INFO on x.TV_INFO_ID equals tv.TV_INFO_ID
                                             where x.AIR_DATE >= fourMonthsAgo
                                                && x.AIR_DATE <= inOneMonth
                                                && x.SEASON_NUMBER != 0
                                             group new { x, tv } by new { tv.SHOW_NAME, tv.TV_INFO_ID } into grp
                                             select new WhatsNextModelInfo
                                             {
                                                 TvInfoId = grp.Key.TV_INFO_ID,
                                                 ShowName = grp.Key.SHOW_NAME,
                                                 SeasonNumber = grp.Max(m => m.x.SEASON_NUMBER.Value)
                                             }).ToList();

        // Get list of all watched shows
        List<WhatsNextModelInfo> watchedShows = (from x in _context.SL_SHOW
                                                       join episode in _context.SL_TV_EPISODE_INFO on x.INFO_ID equals episode.TV_EPISODE_INFO_ID
                                                       where x.INFO_ID != null
                                                           && x.USER_ID == userId
                                                           && x.SHOW_TYPE_ID == (int)CodeValueIds.TV
                                                       group new { x, episode } by new { x.USER_ID, x.SHOW_NAME, episode.TV_INFO_ID } into grp
                                                       select new WhatsNextModelInfo
                                                       {
                                                           TvInfoId = grp.Key.TV_INFO_ID,
                                                           ShowName = grp.Key.SHOW_NAME,
                                                           SeasonNumber = grp.Max(m => m.episode.SEASON_NUMBER.Value)
                                                       }).ToList();

        // See if person watched the last season
        List<WhatsNextModelInfo> lastSeasonShows = (from x in watchedShows
                                                    join l in latest on x.TvInfoId equals l.TvInfoId
                                                    join ei in _context.SL_TV_EPISODE_INFO on new { l.TvInfoId, l.SeasonNumber } equals new { TvInfoId = ei.TV_INFO_ID, SeasonNumber = ei.SEASON_NUMBER.Value }
                                                    where l.SeasonNumber - 1 == x.SeasonNumber
                                                       || l.SeasonNumber == x.SeasonNumber
                                                    group ei by new { x.TvInfoId, l.SeasonNumber, x.ShowName } into grp
                                                    select new WhatsNextModelInfo
                                                    {
                                                        TvInfoId = grp.Key.TvInfoId,
                                                        ShowName = grp.Key.ShowName,
                                                        SeasonNumber = grp.Key.SeasonNumber,
                                                        TvEpisodeIds = grp.Select(m => m.TV_EPISODE_INFO_ID).ToArray()
                                                    }).ToList();
        //select new WhatsNextModelInfo
        //{

        //};



        // take out the episodes that have been watched already
        int[] episodeIds = lastSeasonShows.SelectMany(m => m.TvEpisodeIds).ToArray();

        int[] watchedEpisodes = _context.SL_SHOW.Where(m => m.USER_ID == userId && m.INFO_ID != null && episodeIds.Contains(m.INFO_ID.Value)).Select(m => m.INFO_ID.Value).ToArray();

        int[] missingEpisodes = episodeIds.Where(m => !watchedEpisodes.Contains(m)).ToArray();

        List<WhatsNextModel> query = _context.SL_TV_EPISODE_INFO.Where(m => missingEpisodes.Contains(m.TV_EPISODE_INFO_ID) && m.AIR_DATE != null).Select(m => new WhatsNextModel
        {
            TvInfoId = m.TV_INFO_ID,
            ShowName = m.TV_INFO.SHOW_NAME,
            TvEpisodeInfoId = m.TV_EPISODE_INFO_ID,
            EpisodeName = m.EPISODE_NAME,
            AirDate = m.AIR_DATE.Value,
            //Status = 
        }).ToList();

        return query;
    }
}
