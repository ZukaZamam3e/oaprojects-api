using Microsoft.EntityFrameworkCore.Query.Internal;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace OAProjects.Store.ShowLogger.Stores;
public class ShowStore : IShowStore
{
    private readonly ShowLoggerDbContext _context;
    private readonly ApisConfig _apisConfig;

    public ShowStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public IEnumerable<SLCodeValueModel> GetCodeValues(Expression<Func<SLCodeValueModel, bool>>? predicate = null)
    {
        IEnumerable<SLCodeValueModel> query = _context.SL_CODE_VALUE.Select(m => new SLCodeValueModel
        {
            CodeTableId = m.CODE_TABLE_ID,
            CodeValueId = m.CODE_VALUE_ID,
            DecodeTxt = m.DECODE_TXT
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public IEnumerable<DetailedShowModel> GetShows(Expression<Func<ShowInfoModel, bool>>? predicate = null)
    {
        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        IEnumerable<ShowInfoModel> infoQuery = from s in _context.SL_SHOW
                                               join t in _context.SL_TV_EPISODE_INFO on new { Id = s.INFO_ID ?? -1, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = t.TV_EPISODE_INFO_ID, Type = INFO_TYPE.TV } into ts
                                               from t in ts.DefaultIfEmpty()
                                               join ti in _context.SL_TV_INFO on new { Id = t.TV_INFO_ID, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = ti.TV_INFO_ID, Type = INFO_TYPE.TV } into tis
                                               from ti in tis.DefaultIfEmpty()
                                               join m in _context.SL_MOVIE_INFO on new { Id = s.INFO_ID ?? -1, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = m.MOVIE_INFO_ID, Type = INFO_TYPE.MOVIE } into ms
                                               from m in ms.DefaultIfEmpty()
                                               select new ShowInfoModel
                                               {
                                                   ShowId = s.SHOW_ID,
                                                   UserId = s.USER_ID,
                                                   ShowName = s.SHOW_NAME,
                                                   SeasonNumber = s.SEASON_NUMBER,
                                                   EpisodeNumber = s.EPISODE_NUMBER,
                                                   DateWatched = s.DATE_WATCHED,
                                                   ShowTypeId = s.SHOW_TYPE_ID,
                                                   ShowTypeIdZ = showTypeIds[s.SHOW_TYPE_ID],
                                                   ShowNotes = s.SHOW_NOTES,
                                                   RestartBinge = s.RESTART_BINGE,
                                                   Runtime = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.RUNTIME : null) : (m != null ? m.RUNTIME : null),
                                                   EpisodeName = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.EPISODE_NAME : null) : "",
                                                   InfoId = s.INFO_ID,
                                                   HasMidCreditsScene = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? 
                                                    (ti != null && !string.IsNullOrEmpty(ti.KEYWORDS) && ti.KEYWORDS.Contains("duringcreditsstinger")) : 
                                                    (m != null && !string.IsNullOrEmpty(m.KEYWORDS) && m.KEYWORDS.Contains("duringcreditsstinger")),
                                                   HasEndCreditsScene = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? 
                                                    (ti != null && !string.IsNullOrEmpty(ti.KEYWORDS) && ti.KEYWORDS.Contains("aftercreditsstinger")) : 
                                                    (m != null && !string.IsNullOrEmpty(m.KEYWORDS) && m.KEYWORDS.Contains("aftercreditsstinger")),
                                                   InfoApiType = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.API_TYPE : null) : (m != null ? m.API_TYPE : null),
                                                   InfoApiId = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? ti.API_ID : null) : (m != null ? m.API_ID : null),
                                                   InfoSeasonNumber = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.SEASON_NUMBER : null) : null,
                                                   InfoEpisodeNumber = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.EPISODE_NUMBER : null) : null,
                                                   InfoImageUrl = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.IMAGE_URL : null) : (m != null ? m.BACKDROP_URL : null),
                                               };

        if (predicate != null)
        {
            infoQuery = infoQuery.AsQueryable().Where(predicate);
        }

        IEnumerable<DetailedShowModel> query = infoQuery.ToList().Select(m =>
        {
            m.ImageUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoImageUrl);
            m.InfoUrl = m.ShowTypeId == (int)CodeValueIds.TV ? _apisConfig.GetTvEpisodeInfoUrl(m.InfoApiType, m.InfoApiId, m.InfoSeasonNumber, m.InfoEpisodeNumber) : _apisConfig.GetMovieInfoUrl(m.InfoApiType, m.InfoApiId);
            return m;
        });

        return query;
    }

    public int CreateShow(int userId, ShowModel model, int? infoId = null)
    {
        SL_SHOW entity = new SL_SHOW
        {
            SHOW_TYPE_ID = model.ShowTypeId,
            DATE_WATCHED = model.DateWatched.Date,
            EPISODE_NUMBER = model.ShowTypeId == (int)CodeValueIds.TV ? model.EpisodeNumber : null,
            SEASON_NUMBER = model.ShowTypeId == (int)CodeValueIds.TV ? model.SeasonNumber : null,
            SHOW_NAME = model.ShowName,
            SHOW_NOTES = model.ShowNotes,
            USER_ID = userId,
            RESTART_BINGE = model.RestartBinge,
            INFO_ID = infoId
        };

        _context.SL_SHOW.Add(entity);
        _context.SaveChanges();
        int id = entity.SHOW_ID;

        if(model.Transactions != null)
        {
            SaveShowTransactions(userId, id, model.DateWatched.Date, model.Transactions);
        }

        return id;
    }

    public int UpdateShow(int userId, ShowModel model)
    {
        int result = 0;
        SL_SHOW? entity = _context.SL_SHOW.FirstOrDefault(m => m.SHOW_ID == model.ShowId && m.USER_ID == userId);

        if (entity != null)
        {
            bool deleteTransactions = false;

            // Delete all transactions if AMC -> TV or Movie
            if (entity.SHOW_TYPE_ID == (int)CodeValueIds.AMC
                && (model.ShowTypeId == (int)CodeValueIds.TV || model.ShowTypeId == (int)CodeValueIds.MOVIE))
            {
                deleteTransactions = true;
            }

            if (entity.INFO_ID != null 
                && (entity.EPISODE_NUMBER != model.EpisodeNumber
                || entity.SEASON_NUMBER != model.SeasonNumber))
            {
                entity.INFO_ID = GetTvEpisodeInfoId(entity.INFO_ID, model.SeasonNumber, model.EpisodeNumber);
            }

            entity.SHOW_TYPE_ID = model.ShowTypeId;
            entity.DATE_WATCHED = model.DateWatched.Date;
            entity.EPISODE_NUMBER = model.EpisodeNumber;
            entity.SEASON_NUMBER = model.SeasonNumber;
            entity.SHOW_NAME = model.ShowName;
            entity.SHOW_NOTES = model.ShowNotes;
            entity.RESTART_BINGE = model.RestartBinge;

            result = _context.SaveChanges();

            if (deleteTransactions)
            {
                result += DeleteAllShowTransactions(userId, entity.SHOW_ID);
            }
            else
            {
                if (model.Transactions != null)
                {
                    result += SaveShowTransactions(userId, entity.SHOW_ID, model.DateWatched.Date, model.Transactions);
                }
            }
        }

        return result;
    }

    public int AddNextEpisode(int userId, int showId, DateTime dateWatched)
    {
        int newShowId = -1;
        SL_SHOW? entity = _context.SL_SHOW.FirstOrDefault(m => m.SHOW_ID == showId && m.USER_ID == userId && m.SHOW_TYPE_ID == (int)CodeValueIds.TV);

        if (entity != null)
        {
            SL_SHOW nextEpisode = new SL_SHOW
            {
                SHOW_NAME = entity.SHOW_NAME,
                SHOW_TYPE_ID = entity.SHOW_TYPE_ID,
                USER_ID = userId,
                SEASON_NUMBER = entity.SEASON_NUMBER,
                EPISODE_NUMBER = entity.EPISODE_NUMBER + 1,
                DATE_WATCHED = dateWatched.Date,
            };

            if (entity.INFO_ID != null)
            {
                SL_TV_EPISODE_INFO info = _context.SL_TV_EPISODE_INFO.First(m => m.TV_EPISODE_INFO_ID == entity.INFO_ID);

                List<SL_TV_EPISODE_INFO> episodes = GetEpisodes(entity.INFO_ID);
                List<SL_TV_EPISODE_ORDER> episodeOrder = _context.SL_TV_EPISODE_ORDER.Where(m => m.TV_INFO_ID == info.TV_INFO_ID).ToList();

                int episodesLeft;
                SL_TV_EPISODE_INFO? nextEpisodeInfo = GetNextEpisode(episodes, episodeOrder, info.TV_EPISODE_INFO_ID, out episodesLeft);

                if (nextEpisodeInfo != null)
                {
                    nextEpisode.SEASON_NUMBER = nextEpisodeInfo.SEASON_NUMBER;
                    nextEpisode.EPISODE_NUMBER = nextEpisodeInfo.EPISODE_NUMBER;
                    nextEpisode.INFO_ID = nextEpisodeInfo.TV_EPISODE_INFO_ID;
                }
            }

            _context.SL_SHOW.Add(nextEpisode);

            _context.SaveChanges();

            newShowId = nextEpisode.SHOW_ID;
        }

        return newShowId;
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
                    if (index != -1 && index + 1 < episodes.Count)
                    {
                        nextEpisodeInfo = episodes[index + 1];
                        episodesLeft = episodes.Count - (index + 2);
                    }
                }
            }
        }

        return nextEpisodeInfo;
    }

    public bool DeleteShow(int userId, int showId)
    {
        bool result = false;
        SL_SHOW? entity = _context.SL_SHOW.FirstOrDefault(m => m.SHOW_ID == showId && m.USER_ID == userId);

        if (entity != null)
        {
            _context.SL_SHOW.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }

    public bool AddOneDay(int userId, int showId)
    {
        bool result = false;
        SL_SHOW? entity = _context.SL_SHOW.FirstOrDefault(m => m.SHOW_ID == showId && m.USER_ID == userId);

        if (entity != null)
        {
            entity.DATE_WATCHED = entity.DATE_WATCHED.AddDays(1);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }

    public bool SubtractOneDay(int userId, int showId)
    {
        bool result = false;
        SL_SHOW? entity = _context.SL_SHOW.FirstOrDefault(m => m.SHOW_ID == showId && m.USER_ID == userId);

        if (entity != null)
        {
            entity.DATE_WATCHED = entity.DATE_WATCHED.AddDays(-1);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }

    public bool AddRange(int userId, AddRangeModel model)
    {
        for (int i = model.StartEpisode; i <= model.EndEpisode; i++)
        {
            int? nextInfoId = GetTvEpisodeInfoId(model.ShowName, model.SeasonNumber, i);

            SL_SHOW nextEpisode = new SL_SHOW
            {
                SHOW_NAME = model.ShowName,
                SHOW_TYPE_ID = (int)CodeValueIds.TV,
                USER_ID = userId,
                SEASON_NUMBER = model.SeasonNumber,
                EPISODE_NUMBER = i,
                INFO_ID = nextInfoId,
                DATE_WATCHED = model.DateWatched.Date,
            };

            _context.SL_SHOW.Add(nextEpisode);
        }

        _context.SaveChanges();

        bool successful = true;
        return successful;
    }

    public IEnumerable<ShowTransactionModel> GetShowTransactions(int userId, int showId)
    {
        Dictionary<int, string> transactionTypeIds = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.TRANSACTION_TYPE_ID).ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        IEnumerable<ShowTransactionModel> query = _context.SL_TRANSACTION
            .Where(m => m.USER_ID == userId && m.SHOW_ID == showId)
            .Select(m => new ShowTransactionModel
        {
            TransactionId = m.TRANSACTION_ID,
            TransactionTypeId = m.TRANSACTION_TYPE_ID,
            TransactionTypeIdZ = transactionTypeIds[m.TRANSACTION_TYPE_ID],
            Item = m.ITEM,
            CostAmt = m.COST_AMT,
            Quantity = m.QUANTITY,
            TransactionNotes = m.TRANSACTION_NOTES,
        });

        return query;
    }

    public int DeleteAllShowTransactions(int userId, int showId)
    {
        int result = 0;
        IEnumerable<SL_TRANSACTION> entity = _context.SL_TRANSACTION.Where(m => m.SHOW_ID == showId && m.USER_ID == userId);

        if (entity != null)
        {
            _context.SL_TRANSACTION.RemoveRange(entity);

            _context.SaveChanges();

            result = _context.SaveChanges();
        }

        return result;
    }

    public int SaveShowTransactions(int userId, int showId, DateTime dateWatched, IEnumerable<ShowTransactionModel>? transactions)
    {
        int result = 0;

        foreach (ShowTransactionModel transaction in transactions)
        {
            SL_TRANSACTION? entity;

            if(transaction.TransactionId < 0)
            {
                entity = new SL_TRANSACTION();
                _context.SL_TRANSACTION.Add(entity);
            }
            else
            {
                entity = _context.SL_TRANSACTION.First(m => m.TRANSACTION_ID == transaction.TransactionId && m.USER_ID == userId);
            }

            if(transaction.DeleteTransaction)
            {
                _context.SL_TRANSACTION.Remove(entity);
            }
            else
            {
                entity.USER_ID = userId;
                entity.TRANSACTION_TYPE_ID = transaction.TransactionTypeId;
                entity.SHOW_ID = showId;
                entity.ITEM = transaction.Item;
                entity.COST_AMT = transaction.CostAmt;
                entity.QUANTITY = transaction.Quantity;
                entity.TRANSACTION_NOTES = transaction.TransactionNotes;
                entity.TRANSACTION_DATE = dateWatched;
            }
        }

        result = _context.SaveChanges();

        return result;
    }

    public IEnumerable<FriendWatchHistoryModel> GetFriendWatchHistory(int userId, Dictionary<int, string> users)
    {
        int[] friends = _context.SL_FRIEND.Where(m => m.USER_ID == userId).Select(m => m.FRIEND_USER_ID)
            .Union(_context.SL_FRIEND.Where(m => m.FRIEND_USER_ID == userId).Select(m => m.USER_ID)).ToArray();

        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID).ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        IEnumerable<ShowInfoModel> infoQuery = from s in _context.SL_SHOW
                                               join t in _context.SL_TV_EPISODE_INFO on new { Id = s.INFO_ID ?? -1, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = t.TV_EPISODE_INFO_ID, Type = INFO_TYPE.TV } into ts
                                               from t in ts.DefaultIfEmpty()
                                               join ti in _context.SL_TV_INFO on new { Id = t.TV_INFO_ID, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = ti.TV_INFO_ID, Type = INFO_TYPE.TV } into tis
                                               from ti in tis.DefaultIfEmpty()
                                               join m in _context.SL_MOVIE_INFO on new { Id = s.INFO_ID ?? -1, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = m.MOVIE_INFO_ID, Type = INFO_TYPE.MOVIE } into ms
                                               from m in ms.DefaultIfEmpty()
                                               where friends.Contains(s.USER_ID)
                                               select new ShowInfoModel
                                               {
                                                   ShowId = s.SHOW_ID,
                                                   UserId = s.USER_ID,
                                                   ShowName = s.SHOW_NAME,
                                                   SeasonNumber = s.SEASON_NUMBER,
                                                   EpisodeNumber = s.EPISODE_NUMBER,
                                                   DateWatched = s.DATE_WATCHED,
                                                   ShowTypeId = s.SHOW_TYPE_ID,
                                                   ShowTypeIdZ = showTypeIds[s.SHOW_TYPE_ID],
                                                   ShowNotes = s.SHOW_NOTES,
                                                   RestartBinge = s.RESTART_BINGE,
                                                   Runtime = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.RUNTIME : null) : (m != null ? m.RUNTIME : null),
                                                   EpisodeName = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.EPISODE_NAME : null) : "",
                                                   InfoId = s.INFO_ID,

                                                   InfoApiType = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.API_TYPE : null) : (m != null ? m.API_TYPE : null),
                                                   InfoApiId = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? ti.API_ID : null) : (m != null ? m.API_ID : null),
                                                   InfoSeasonNumber = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.SEASON_NUMBER : null) : null,
                                                   InfoEpisodeNumber = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.EPISODE_NUMBER : null) : null,
                                                   InfoImageUrl = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.IMAGE_URL : null) : (m != null ? m.BACKDROP_URL : null),
                                               };

        IEnumerable<FriendWatchHistoryModel> query = infoQuery.ToList().Select(m => new FriendWatchHistoryModel
        {
            ShowId = m.ShowId,
            UserId = m.UserId,
            Name = users[m.UserId],
            ShowName = m.ShowName,
            SeasonNumber = m.SeasonNumber,
            EpisodeNumber = m.EpisodeNumber,
            DateWatched = m.DateWatched,
            ShowTypeId = m.ShowTypeId,
            ShowTypeIdZ = m.ShowTypeIdZ,
            ShowNotes = m.ShowNotes,
            RestartBinge = m.RestartBinge,
            Runtime = m.Runtime,
            EpisodeName = m.EpisodeName,
            InfoId = m.InfoId,
            ImageUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoImageUrl),
            InfoUrl = (m.ShowTypeId == (int)CodeValueIds.TV ? _apisConfig.GetTvEpisodeInfoUrl(m.InfoApiType, m.InfoApiId, m.InfoSeasonNumber, m.InfoEpisodeNumber) : _apisConfig.GetMovieInfoUrl(m.InfoApiType, m.InfoApiId))
        });

        return query;
    }

   

    private List<SL_TV_EPISODE_INFO> GetEpisodes(int? tvEpisodeInfoId)
    {
        SL_TV_EPISODE_INFO? currentInfo = _context.SL_TV_EPISODE_INFO.FirstOrDefault(m => m.TV_EPISODE_INFO_ID == tvEpisodeInfoId);

        if (currentInfo == null)
            return null;

        return _context.SL_TV_EPISODE_INFO.Where(m => m.TV_INFO_ID == currentInfo.TV_INFO_ID)
                .OrderByDescending(m => m.SEASON_NUMBER > 0)
                .ThenBy(m => m.SEASON_NUMBER)
                .ThenBy(m => m.EPISODE_NUMBER)
                .ToList();
    }

    private int? GetTvEpisodeInfoId(int? infoId, int? seasonNumber, int? episodeNumber)
    {
        SL_TV_EPISODE_INFO? currentInfo = _context.SL_TV_EPISODE_INFO.FirstOrDefault(m => m.TV_EPISODE_INFO_ID == infoId);
        SL_TV_EPISODE_INFO? nextInfo = null;
        if (currentInfo != null)
        {
            nextInfo = _context.SL_TV_EPISODE_INFO.FirstOrDefault(m => m.SEASON_NUMBER == seasonNumber && m.EPISODE_NUMBER == episodeNumber && m.TV_INFO_ID == currentInfo.TV_INFO_ID);
        }

        return nextInfo?.TV_EPISODE_INFO_ID;
    }

    private int? GetTvEpisodeInfoId(string name, int? seasonNumber, int? episodeNumber)
    {
        int? infoId = null;
        Dictionary<string, SL_TV_INFO> lookUp = new();

        SL_TV_INFO[] shows = _context.SL_TV_INFO.ToArray();

        foreach (SL_TV_INFO info in shows)
        {
            lookUp.Add(info.SHOW_NAME.ToLower(), info);
        }

        //SL_TV_INFO? show = _context.SL_TV_INFO.FirstOrDefault(m => m.SHOW_NAME.ToLower() == name.ToLower() || m.)

        SL_TV_INFO found;

        if (lookUp.TryGetValue(name.ToLower(), out found))
        {
            SL_TV_EPISODE_INFO? episodeInfo = _context.SL_TV_EPISODE_INFO.FirstOrDefault(m => m.TV_INFO_ID == found.TV_INFO_ID && m.SEASON_NUMBER == seasonNumber && m.EPISODE_NUMBER == episodeNumber);

            if (episodeInfo == null && episodeNumber != null)
            {
                // Somes shows I track are by episode and ignoring season. 
                // So get all episodes and then find the row that matches the episode
                episodeInfo = _context.SL_TV_EPISODE_INFO.Where(m => m.TV_INFO_ID == found.TV_INFO_ID && m.SEASON_NUMBER > 0)
                    .OrderBy(m => m.SEASON_NUMBER).ThenBy(m => m.EPISODE_NUMBER)
                    .Skip(episodeNumber.Value - 1).FirstOrDefault();
            }

            if (episodeInfo != null)
            {
                infoId = episodeInfo.TV_EPISODE_INFO_ID;
            }
        }

        return infoId;
    }

    public IEnumerable<TransactionItemModel> GetTransactionItems(int userId)
    {
        IEnumerable<TransactionItemModel> query = _context.SL_TRANSACTION
            .Where(m => m.USER_ID == userId && m.TRANSACTION_TYPE_ID == (int)CodeValueIds.PURCHASE)
            .OrderByDescending(m => m.TRANSACTION_DATE)
            .GroupBy(m => new { m.ITEM, m.QUANTITY })
            .Select(m => new TransactionItemModel
            {
                Item = m.Key.ITEM,
                Quantity = m.Key.QUANTITY,
                LastTransactionDate = m.Max(n => n.TRANSACTION_DATE),
                CostAmt = m.OrderByDescending(n => n.TRANSACTION_DATE).First().COST_AMT
            });

        return query;
    }
}
