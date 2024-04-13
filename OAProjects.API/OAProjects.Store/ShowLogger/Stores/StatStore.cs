using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Stat;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores;
public class StatStore : IStatStore
{
    private readonly ShowLoggerDbContext _context;

    public StatStore(ShowLoggerDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TvStatModel> GetTVStats(int userId)
    {
        IEnumerable<SL_SHOW> shows = _context.SL_SHOW.Where(m => m.SHOW_TYPE_ID == (int)CodeValueIds.TV && m.USER_ID == userId)
            .OrderBy(m => m.SHOW_NAME)
            .ThenBy(m => m.DATE_WATCHED)
            .ThenBy(m => m.SHOW_ID);

        TvStatModel model = new TvStatModel();
        int count = 0;
        SL_SHOW? previousShow = null;

        List<TvStatModel> list = new List<TvStatModel>();

        foreach (SL_SHOW? show in shows)
        {
            if (model.ShowName != show.SHOW_NAME || show.RESTART_BINGE)
            {
                if (previousShow != null)
                {
                    model.LastWatched = previousShow.DATE_WATCHED;
                    model.LatestSeasonNumber = previousShow.SEASON_NUMBER;
                    model.LatestEpisodeNumber = previousShow.EPISODE_NUMBER;
                    model.EpisodesWatched = (model.LatestSeasonNumber == previousShow.SEASON_NUMBER && model.LatestEpisodeNumber == previousShow.EPISODE_NUMBER ? count : ++count);
                    model.ShowId = previousShow.SHOW_ID;

                    list.Add(model);
                }

                model = new TvStatModel
                {
                    UserId = userId,
                    ShowName = show.SHOW_NAME,
                    FirstWatched = show.DATE_WATCHED,
                    StartingSeasonNumber = show.SEASON_NUMBER,
                    StartingEpisodeNumber = show.EPISODE_NUMBER,
                };

                count = 1;
            }
            else if (previousShow != null && previousShow.DATE_WATCHED.AddMonths(4) < show.DATE_WATCHED)
            {
                model.LastWatched = previousShow.DATE_WATCHED;
                model.LatestSeasonNumber = previousShow.SEASON_NUMBER;
                model.LatestEpisodeNumber = previousShow.EPISODE_NUMBER;
                model.EpisodesWatched = (model.LatestSeasonNumber == previousShow.SEASON_NUMBER && model.LatestEpisodeNumber == previousShow.EPISODE_NUMBER ? count : ++count);
                model.ShowId = previousShow.SHOW_ID;

                list.Add(model);

                model = new TvStatModel
                {
                    UserId = userId,
                    ShowName = show.SHOW_NAME,
                    FirstWatched = show.DATE_WATCHED,
                    StartingSeasonNumber = show.SEASON_NUMBER,
                    StartingEpisodeNumber = show.EPISODE_NUMBER,
                };

                count = 1;
            }
            else
            {
                ++count;
            }

            previousShow = show;
        }


        return list;
    }

    public IEnumerable<MovieStatModel> GetMovieStats(int userId)
    {
        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID).ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        List<MovieStatModel> query = _context.SL_SHOW
            .Where(m => m.USER_ID == userId
                && (m.SHOW_TYPE_ID == (int)CodeValueIds.AMC || m.SHOW_TYPE_ID == (int)CodeValueIds.MOVIE))
            .Select(m => new MovieStatModel
            {
                UserId = m.USER_ID,
                MovieName = m.SHOW_NAME,
                ShowId = m.SHOW_ID,
                ShowTypeId = m.SHOW_TYPE_ID,
                ShowTypeIdZ = showTypeIds[m.SHOW_TYPE_ID],
                DateWatched = m.DATE_WATCHED,
            }).ToList();

        int[] amcIds = query.Where(m => m.ShowTypeId == (int)CodeValueIds.AMC).Select(m => m.ShowId).ToArray();

        if (amcIds.Length > 0)
        {
            IEnumerable<IGrouping<int, SL_TRANSACTION>> transactions = _context.SL_TRANSACTION
                .Where(m => m.SHOW_ID != null && m.USER_ID == userId)
                .AsEnumerable().GroupBy(m => m.SHOW_ID.Value);

            if (transactions != null && transactions.Count() > 0)
            {
                foreach (IGrouping<int, SL_TRANSACTION> transaction in transactions)
                {
                    MovieStatModel movie = query.First(m => m.ShowId == transaction.Key);

                    movie.AlistTicketAmt = transaction.Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.ALIST_TICKET).Sum(m => m.COST_AMT);
                    movie.TicketAmt = transaction.Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.TICKET).Sum(m => m.COST_AMT);
                    movie.PurchaseAmt = transaction.Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.PURCHASE).Sum(m => m.COST_AMT);
                    movie.BenefitsAmt = transaction.Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.BENEFITS).Sum(m => m.COST_AMT);
                    movie.RewardsAmt = transaction.Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.REWARDS).Sum(m => m.COST_AMT);
                    movie.TotalAmt = movie.PurchaseAmt - movie.BenefitsAmt - movie.RewardsAmt;
                }
            }
        }

        return query;
    }

    public IEnumerable<YearStatModel> GetYearStats(int userId, Dictionary<int, string> users)
    {
        int[] friends = _context.SL_FRIEND.Where(m => m.USER_ID == userId).Select(m => m.FRIEND_USER_ID)
            .Union(_context.SL_FRIEND.Where(m => m.FRIEND_USER_ID == userId).Select(m => m.USER_ID)).ToArray();

        IEnumerable<YearStatModel> modelShows = (from x in _context.SL_SHOW
                                                  group new { x } by new { x.USER_ID, x.DATE_WATCHED.Year } into g
                                                  select new YearStatModel
                                                  {
                                                      UserId = g.Key.USER_ID,
                                                      Name = users[g.Key.USER_ID],
                                                      Year = g.Key.Year,
                                                      TvCnt = g.Count(m => m.x.SHOW_TYPE_ID == (int)CodeValueIds.TV),
                                                      MoviesCnt = g.Count(m => m.x.SHOW_TYPE_ID == (int)CodeValueIds.MOVIE),
                                                      AmcCnt = g.Count(m => m.x.SHOW_TYPE_ID == (int)CodeValueIds.AMC),
                                                  }).Where(m => m.UserId == userId || friends.Contains(m.UserId)).ToList();

        int[] purchases = new int[] { (int)CodeValueIds.PURCHASE, (int)CodeValueIds.TAX, (int)CodeValueIds.TICKET };
        int[] discounts = new int[] { (int)CodeValueIds.BENEFITS, (int)CodeValueIds.REWARDS };

        IEnumerable<YearStatModel> modelTransactions = (from t in _context.SL_TRANSACTION
                                                         group new { t } by new { t.USER_ID, t.TRANSACTION_DATE.Year } into g
                                                         select new YearStatModel
                                                         {
                                                             UserId = g.Key.USER_ID,
                                                             Name = users[g.Key.USER_ID],
                                                             Year = g.Key.Year,
                                                             AListMembership = g.Where(m => m.t.TRANSACTION_TYPE_ID == (int)CodeValueIds.ALIST).Sum(m => m.t.COST_AMT),
                                                             AListTickets = g.Where(m => m.t.TRANSACTION_TYPE_ID == (int)CodeValueIds.ALIST_TICKET).Sum(m => m.t.COST_AMT),
                                                             AmcPurchases = g.Where(m => purchases.Contains(m.t.TRANSACTION_TYPE_ID)).Sum(m => m.t.COST_AMT)
                                                                            - g.Where(m => discounts.Contains(m.t.TRANSACTION_TYPE_ID)).Sum(m => m.t.COST_AMT),
                                                         }).Where(m => m.UserId == userId || friends.Contains(m.UserId)).ToList();

        IEnumerable<YearStatModel> tvRuntimes = (from x in _context.SL_SHOW
                                                  join ei in _context.SL_TV_EPISODE_INFO on x.INFO_ID equals ei.TV_EPISODE_INFO_ID
                                                  where x.SHOW_TYPE_ID == (int)CodeValueIds.TV && x.INFO_ID != null
                                                  group new { x, ei } by new { x.USER_ID, x.DATE_WATCHED.Year } into g
                                                  select new YearStatModel
                                                  {
                                                      UserId = g.Key.USER_ID,
                                                      Year = g.Key.Year,
                                                      TvCnt = g.Count(),
                                                      TvRuntime = g.Sum(m => m.ei.RUNTIME) ?? 0
                                                  }).Where(m => m.UserId == userId || friends.Contains(m.UserId)).ToList();

        IEnumerable<YearStatModel> movieRuntimes = (from x in _context.SL_SHOW 
                                                     join mi in _context.SL_MOVIE_INFO on x.INFO_ID equals mi.MOVIE_INFO_ID
                                                     where x.SHOW_TYPE_ID == (int)CodeValueIds.MOVIE && x.INFO_ID != null
                                                     group new { x, mi } by new { x.USER_ID, x.DATE_WATCHED.Year } into g
                                                     select new YearStatModel
                                                     {
                                                         UserId = g.Key.USER_ID,
                                                         Year = g.Key.Year,
                                                         MoviesCnt = g.Count(),
                                                         MoviesRuntime = g.Sum(m => m.mi.RUNTIME) ?? 0
                                                     }).Where(m => m.UserId == userId || friends.Contains(m.UserId)).ToList();

        IEnumerable<YearStatModel> amcRuntimes = (from x in _context.SL_SHOW
                                                   join mi in _context.SL_MOVIE_INFO on x.INFO_ID equals mi.MOVIE_INFO_ID
                                                   where x.SHOW_TYPE_ID == (int)CodeValueIds.AMC && x.INFO_ID != null
                                                   group new { x, mi } by new { x.USER_ID, x.DATE_WATCHED.Year } into g
                                                   select new YearStatModel
                                                   {
                                                       UserId = g.Key.USER_ID,
                                                       Year = g.Key.Year,
                                                       AmcCnt = g.Count(),
                                                       AmcRuntime = g.Sum(m => m.mi.RUNTIME) ?? 0
                                                   }).Where(m => m.UserId == userId || friends.Contains(m.UserId)).ToList();

        IEnumerable<YearStatModel> model = (from s in modelShows
                                             join t in modelTransactions on new { s.UserId, s.Year } equals new { t.UserId, t.Year } into ts
                                             from t in ts.DefaultIfEmpty()
                                             join rtv in tvRuntimes on new { s.UserId, s.Year } equals new { rtv.UserId, rtv.Year } into rtvs
                                             from rtv in rtvs.DefaultIfEmpty()
                                             join rmovies in movieRuntimes on new { s.UserId, s.Year } equals new { rmovies.UserId, rmovies.Year } into rmoviess
                                             from rmovies in rmoviess.DefaultIfEmpty()
                                             join ramc in amcRuntimes on new { s.UserId, s.Year } equals new { ramc.UserId, ramc.Year } into ramcs
                                             from ramc in ramcs.DefaultIfEmpty()
                                             select new YearStatModel
                                             {
                                                 UserId = s.UserId,
                                                 Year = s.Year,
                                                 Name = s.Name,
                                                 TvCnt = s.TvCnt,
                                                 TvNotTrackedCnt = s.TvCnt - (rtv?.TvCnt ?? 0),
                                                 TvRuntime = rtv?.TvRuntime,
                                                 MoviesCnt = s.MoviesCnt,
                                                 MoviesNotTrackedCnt = s.MoviesCnt - (rmovies?.MoviesCnt ?? 0),
                                                 MoviesRuntime = rmovies?.MoviesRuntime,
                                                 AmcCnt = s.AmcCnt,
                                                 AmcNotTrackedCnt = s.AmcCnt - (ramc?.AmcCnt ?? 0),
                                                 AmcRuntime = ramc?.AmcRuntime,
                                                 AListMembership = t?.AListMembership ?? 0,
                                                 AListTickets = t?.AListTickets ?? 0,
                                                 AmcPurchases = t?.AmcPurchases ?? 0
                                             });

        return model;
    }
}
