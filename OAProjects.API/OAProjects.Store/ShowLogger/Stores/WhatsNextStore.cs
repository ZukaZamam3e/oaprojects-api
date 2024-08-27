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

        IEnumerable<WhatsNextModel> query = new List<WhatsNextModel>();

        // Get list of coming soon, currently airing, and season ended and the season
        IEnumerable<WhatsNextModel> latest = from x in _context.SL_TV_EPISODE_INFO
                                             join tv in _context.SL_TV_INFO on x.TV_INFO_ID equals tv.TV_INFO_ID
                                             where x.AIR_DATE >= fourMonthsAgo
                                                && x.AIR_DATE <= inOneMonth
                                                && x.SEASON_NUMBER != 0
                                             group new { x, tv } by new { tv.SHOW_NAME, tv.TV_INFO_ID } into grp
                                             select new WhatsNextModel
                                             {
                                                 TvInfoId = grp.Key.TV_INFO_ID,
                                                 ShowName = grp.Key.SHOW_NAME,
                                             };

        // Get list of all watched shows
        IEnumerable<WhatsNextModelInfo> watched = from x in _context.SL_SHOW
                                                  join episode in _context.SL_TV_EPISODE_INFO on x.INFO_ID equals episode.TV_INFO_ID
                                                  where x.INFO_ID != null
                                                     && x.USER_ID == userId
                                                     && x.SHOW_TYPE_ID == (int)CodeValueIds.TV
                                                  group new { x, episode } by new { x.USER_ID, x.SHOW_NAME, episode.TV_INFO_ID } into grp
                                                  select new WhatsNextModelInfo
                                                  {
                                                      TvInfoId = grp.Key.TV_INFO_ID,
                                                      ShowName = grp.Key.SHOW_NAME,
                                                      LastSeasonWatched = grp.Max(m => m.episode.SEASON_NUMBER.Value)
                                                  };

        // See if person watched the last season

        // take out the episodes that have been watched already

        return query;
    }
}
