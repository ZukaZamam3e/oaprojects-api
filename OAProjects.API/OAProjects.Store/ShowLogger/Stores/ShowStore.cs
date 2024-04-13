using Microsoft.EntityFrameworkCore.Query.Internal;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
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

                                                   InfoApiType = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.API_TYPE : null) : (m != null ? m.API_TYPE : null),
                                                   InfoApiId = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? ti.API_ID : null) : (m != null ? m.API_ID : null),
                                                   InfoSeasonNumber = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.SEASON_NUMBER : null) : null,
                                                   InfoEpisodeNumber = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.EPISODE_NUMBER : null) : null,
                                                   InfoImageUrl = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.IMAGE_URL : null) : (m != null ? m.IMAGE_URL : null),
                                               };

        if (predicate != null)
        {
            infoQuery = infoQuery.AsQueryable().Where(predicate);
        }

        IEnumerable<DetailedShowModel> query = infoQuery.ToList().Select(m =>
        {
            m.ImageUrl = GetImageUrl(m.InfoApiType, m.InfoImageUrl);
            m.InfoUrl = m.ShowTypeId == (int)CodeValueIds.TV ? GetTvEpisodeInfoUrl(m.InfoApiType, m.InfoApiId, m.InfoSeasonNumber, m.InfoEpisodeNumber) : GetMovieInfoUrl(m.InfoApiType, m.InfoApiId);
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

            entity.SHOW_TYPE_ID = model.ShowTypeId;
            entity.DATE_WATCHED = model.DateWatched.Date;
            entity.EPISODE_NUMBER = model.EpisodeNumber;
            entity.SEASON_NUMBER = model.SeasonNumber;
            entity.SHOW_NAME = model.ShowName;
            entity.SHOW_NOTES = model.ShowNotes;

            int result = _context.SaveChanges();

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

        return 0;
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

            _context.SL_SHOW.Add(nextEpisode);

            _context.SaveChanges();

            newShowId = nextEpisode.SHOW_ID;
        }

        return newShowId;
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
            //int? nextInfoId = GetTvEpisodeInfoId(model.ShowName, model.SeasonNumber, i);

            SL_SHOW nextEpisode = new SL_SHOW
            {
                SHOW_NAME = model.ShowName,
                SHOW_TYPE_ID = (int)CodeValueIds.TV,
                USER_ID = userId,
                SEASON_NUMBER = model.SeasonNumber,
                EPISODE_NUMBER = i,
                //INFO_ID = nextInfoId,
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

            if(transaction.TransactionId == 0)
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
                                                   InfoImageUrl = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.IMAGE_URL : null) : (m != null ? m.IMAGE_URL : null),
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
            ImageUrl = GetImageUrl(m.InfoApiType, m.InfoImageUrl),
            InfoUrl = (m.ShowTypeId == (int)CodeValueIds.TV ? GetTvEpisodeInfoUrl(m.InfoApiType, m.InfoApiId, m.InfoSeasonNumber, m.InfoEpisodeNumber) : GetMovieInfoUrl(m.InfoApiType, m.InfoApiId))
        });

        return query;
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
            INFO_API.TMDB_API => $"{_apisConfig.TMDbURL}{TMDBApiPaths.TV}{$"/{apiId}/season/{seasonNumber}/episode/{episodeNumber}"}",
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
}
