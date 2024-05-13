using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
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
    private readonly ApisConfig _apisConfig;

    public StatStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public IEnumerable<TvStatModel> GetTVStats(int userId)
    {
        IEnumerable<SL_SHOW> shows = _context.SL_SHOW.Where(m => m.SHOW_TYPE_ID == (int)CodeValueIds.TV && m.USER_ID == userId)
            .OrderBy(m => m.SHOW_NAME)
            .ThenBy(m => m.DATE_WATCHED)
            .ThenBy(m => m.SHOW_ID);

        int[] episodeTvInfoIds = shows.Where(m => m.INFO_ID != null).Select(m => m.INFO_ID.Value).ToArray();
        int[] tvInfoIds = _context.SL_TV_EPISODE_INFO.Where(m => episodeTvInfoIds.Contains(m.TV_EPISODE_INFO_ID)).Select(m => m.TV_INFO_ID).ToArray();
        SL_TV_INFO[] tvInfos = _context.SL_TV_INFO.Where(m => tvInfoIds.Contains(m.TV_INFO_ID)).ToArray();

        List<SL_TV_EPISODE_INFO> episodes = _context.SL_TV_EPISODE_INFO.Where(m => tvInfoIds.Contains(m.TV_INFO_ID))
                .OrderBy(m => m.TV_INFO_ID)
                .ThenBy(m => m.SEASON_NUMBER)
                .ThenBy(m => m.EPISODE_NUMBER)
                .ToList();

        List<SL_TV_EPISODE_ORDER> orders = _context.SL_TV_EPISODE_ORDER.Where(m => tvInfoIds.Contains(m.TV_INFO_ID)).ToList();

        TvStatModel model = new TvStatModel();
        int count = 0;
        SL_SHOW? previousShow = null;

        DateTime fourMonthsAgo = DateTime.Now.AddMonths(-4);


        List<TvStatModel> list = new List<TvStatModel>();

        foreach (SL_SHOW? show in shows)
        {
            if (model.ShowName != show.SHOW_NAME || show.RESTART_BINGE)
            {
                if (previousShow != null)
                {
                    count = UpdateLatest(model, previousShow, count);

                    if (model.InfoId != null)
                    {
                        SL_TV_EPISODE_INFO episodeInfo = episodes.First(m => m.TV_EPISODE_INFO_ID == model.InfoId);
                        SL_TV_INFO info = tvInfos.First(m => m.TV_INFO_ID == episodeInfo.TV_INFO_ID);
                        model.InfoBackdropUrl = GetImageUrl(info.API_TYPE, info.BACKDROP_URL);
                        model.InfoUrl = GetTvInfoUrl(info.API_TYPE, info.API_ID);
                    }

                    if (model.InfoId != null && model.LastWatched > fourMonthsAgo)
                    {
                        int episodesLeft;
                        SL_TV_EPISODE_INFO? nextEpisodeInfo = GetNextEpisode(episodes, orders, model.InfoId, out episodesLeft);

                        if (nextEpisodeInfo != null)
                        {
                            SL_TV_INFO info = tvInfos.First(m => m.TV_INFO_ID == nextEpisodeInfo.TV_INFO_ID);
                            UpdateNextEpisode(model, info, nextEpisodeInfo, episodesLeft);  
                        }
                    }

                    list.Add(model);
                }

                model = CreateFromShow(userId, show);

                count = 1;
            }
            else if (previousShow != null && previousShow.DATE_WATCHED.AddMonths(4) < show.DATE_WATCHED)
            {
                count = UpdateLatest(model, previousShow, count);

                if (model.InfoId != null)
                {
                    SL_TV_EPISODE_INFO episodeInfo = episodes.First(m => m.TV_EPISODE_INFO_ID == model.InfoId);
                    SL_TV_INFO info = tvInfos.First(m => m.TV_INFO_ID == episodeInfo.TV_INFO_ID);
                    model.InfoBackdropUrl = GetImageUrl(info.API_TYPE, info.BACKDROP_URL);
                    model.InfoUrl = GetTvInfoUrl(info.API_TYPE, info.API_ID);
                }

                if (model.InfoId != null && model.LastWatched > fourMonthsAgo)
                {
                    int episodesLeft;
                    SL_TV_EPISODE_INFO? nextEpisodeInfo = GetNextEpisode(episodes, orders, model.InfoId, out episodesLeft);

                    if (nextEpisodeInfo != null)
                    {
                        SL_TV_INFO info = tvInfos.First(m => m.TV_INFO_ID == nextEpisodeInfo.TV_INFO_ID);
                        UpdateNextEpisode(model, info, nextEpisodeInfo, episodesLeft);
                    }
                }

                list.Add(model);

                model = CreateFromShow(userId, show);

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

    private TvStatModel CreateFromShow(int userId, SL_SHOW show)
    {
        return new TvStatModel
        {
            UserId = userId,
            ShowName = show.SHOW_NAME,
            FirstWatched = show.DATE_WATCHED,
            StartingSeasonNumber = show.SEASON_NUMBER,
            StartingEpisodeNumber = show.EPISODE_NUMBER,
        };
    }

    private int UpdateLatest(TvStatModel model, SL_SHOW previousShow, int count)
    {
        model.LastWatched = previousShow.DATE_WATCHED;
        model.LatestSeasonNumber = previousShow.SEASON_NUMBER;
        model.LatestEpisodeNumber = previousShow.EPISODE_NUMBER;
        model.EpisodesWatched = (model.LatestSeasonNumber == previousShow.SEASON_NUMBER && model.LatestEpisodeNumber == previousShow.EPISODE_NUMBER ? count : ++count);
        model.ShowId = previousShow.SHOW_ID;
        model.InfoId = previousShow.INFO_ID;

        return count;
    }

    private void UpdateNextEpisode(TvStatModel model, SL_TV_INFO info, SL_TV_EPISODE_INFO nextEpisodeInfo, int episodesLeft)
    {
        model.NextSeasonNumber = nextEpisodeInfo.SEASON_NUMBER;
        model.NextEpisodeNumber = nextEpisodeInfo.EPISODE_NUMBER;
        model.NextEpisodeInfoId = nextEpisodeInfo.TV_EPISODE_INFO_ID;
        model.NextEpisodeName = nextEpisodeInfo.EPISODE_NAME;
        model.NextAirDate = nextEpisodeInfo.AIR_DATE;
        model.NextInfoUrl = GetTvEpisodeInfoUrl(info.API_TYPE, info.API_ID, nextEpisodeInfo.SEASON_NUMBER, nextEpisodeInfo.EPISODE_NUMBER);
        model.EpisodesLeft = episodesLeft;
    }

    private string GetTvEpisodeInfoUrl(int? apiType, string? apiId, int? seasonNumber, int? episodeNumber)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId)
            || seasonNumber == null
            || episodeNumber == null)
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{_apisConfig.TMDbURL}{TMDBApiPaths.TV}{$"{apiId}/season/{seasonNumber}/episode/{episodeNumber}"}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    private string GetTvInfoUrl(int? apiType, string? apiId)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId))
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{_apisConfig.TMDbURL}{TMDBApiPaths.TV}{$"{apiId}"}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    private string GetMovieInfoUrl(int? apiType, string? apiId)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId))
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{_apisConfig.TMDbURL}{TMDBApiPaths.Movie}{apiId}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    private string GetImageUrl(int? apiType, string? imageUrl)
    {
        if (apiType == null
            || string.IsNullOrEmpty(imageUrl))
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{_apisConfig.TMDbURL}{TMDBApiPaths.Image}{imageUrl}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    private SL_TV_EPISODE_INFO? GetNextEpisode(List<SL_TV_EPISODE_INFO> episodesList, List<SL_TV_EPISODE_ORDER> episodeOrders, int? episodeInfoId, out int episodesLeft)
    {
        SL_TV_EPISODE_INFO? nextEpisodeInfo = null;
        SL_TV_EPISODE_INFO? currentEpisode = episodesList.First(m => m.TV_EPISODE_INFO_ID == episodeInfoId);
        episodesLeft = 0;

        if (currentEpisode != null)
        {
            List<SL_TV_EPISODE_INFO> episodes = episodesList.Where(m => m.TV_INFO_ID == currentEpisode.TV_INFO_ID).ToList();
            List<SL_TV_EPISODE_ORDER> orders = episodeOrders.Where(m => m.TV_INFO_ID == currentEpisode.TV_INFO_ID).ToList();

            if (episodes != null)
            {
                if (orders.Count > 0)
                {
                    SL_TV_EPISODE_ORDER order = orders.First(m => m.TV_EPISODE_INFO_ID == episodeInfoId);
                    SL_TV_EPISODE_ORDER? next = orders.FirstOrDefault(m => m.EPISODE_ORDER == order.EPISODE_ORDER + 1);

                    if (next != null)
                    {
                        int episodeCount = orders.Max(m => m.EPISODE_ORDER);
                        episodesLeft = episodeCount - order.EPISODE_ORDER;
                        nextEpisodeInfo = episodes.First(m => m.TV_EPISODE_INFO_ID == next.TV_EPISODE_INFO_ID);
                    }
                }
                else
                {
                    int index = episodes.FindIndex(m => m.TV_EPISODE_INFO_ID == episodeInfoId);
                    if (index != -1 && index + 1 < episodes.Count - 1)
                    {
                        nextEpisodeInfo = episodes[index + 1];
                        episodesLeft = episodes.Count - (index + 1);
                    }
                }
            }
        }

        return nextEpisodeInfo;
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
                InfoId = m.INFO_ID,
            }).ToList();

        int[] movieInfoIds = query.Where(m => m.InfoId != null).Select(m => m.InfoId.Value).ToArray();   
        Dictionary<int, SL_MOVIE_INFO> dictMovieInfos = _context.SL_MOVIE_INFO.Where(m => movieInfoIds.Contains(m.MOVIE_INFO_ID)).ToDictionary(m => m.MOVIE_INFO_ID);

        query.ToList().ForEach(m =>
        {
            SL_MOVIE_INFO movieInfo;

            if (m.InfoId != null)
            {
                if (dictMovieInfos.TryGetValue(m.InfoId.Value, out movieInfo))
                {
                    m.InfoBackdropUrl = GetImageUrl(movieInfo.API_TYPE, movieInfo.BACKDROP_URL);
                    m.InfoUrl = GetMovieInfoUrl(movieInfo.API_TYPE, movieInfo.API_ID);
                }
            }
        });

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

    public IEnumerable<BookYearStatModel> GetBookYearStats(int userId, Dictionary<int, string> users)
    {
        int[] friends = _context.SL_FRIEND.Where(m => m.USER_ID == userId).Select(m => m.FRIEND_USER_ID)
            .Union(_context.SL_FRIEND.Where(m => m.FRIEND_USER_ID == userId).Select(m => m.USER_ID)).ToArray();

        SL_BOOK[] books = _context.SL_BOOK.ToArray();

        IEnumerable<BookYearStatModel> model = from x in books
                                                where x.END_DATE != null && x.START_DATE != null
                                                group new { x } by new { x.USER_ID, x.END_DATE.Value.Year } into g
                                                select new BookYearStatModel
                                                {
                                                    UserId = g.Key.USER_ID,
                                                    Name = users[g.Key.USER_ID],
                                                    Year = g.Key.Year,
                                                    BookCnt = g.Count(),
                                                    ChapterCnt = g.Sum(m => m.x.CHAPTERS) ?? 0,
                                                    PageCnt = g.Sum(m => m.x.PAGES) ?? 0,
                                                    TotalDays = (decimal)g.Sum(m => (m.x.END_DATE.Value - m.x.START_DATE.Value).TotalDays)
                                                };

        return model;
    }
}
