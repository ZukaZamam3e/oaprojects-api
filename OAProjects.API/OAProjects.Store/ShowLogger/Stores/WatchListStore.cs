using OAProjects.Models.ShowLogger.Models.WatchList;
using OAProjects.Models.ShowLogger;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Config;

namespace OAProjects.Store.ShowLogger.Stores;
public class WatchListStore : IWatchListStore
{
    private readonly ShowLoggerDbContext _context;
    private readonly ApisConfig _apisConfig;

    public WatchListStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public IEnumerable<DetailedWatchListModel> GetWatchLists(Expression<Func<WatchListInfoModel, bool>>? predicate = null)
    {

        //IEnumerable<WatchListModel> query = _context.SL_WATCHLIST.Select(m => new WatchListModel
        //{
        //    WatchlistId = m.WATCHLIST_ID,
        //    UserId = m.USER_ID,
        //    ShowName = m.SHOW_NAME,
        //    SeasonNumber = m.SEASON_NUMBER,
        //    EpisodeNumber = m.EPISODE_NUMBER,
        //    DateAdded = m.DATE_ADDED,
        //    ShowTypeId = m.SHOW_TYPE_ID,
        //    ShowTypeIdZ = showTypeIds[m.SHOW_TYPE_ID],
        //    ShowNotes = m.SHOW_NOTES,
        //});

        //if (predicate != null)
        //{
        //    query = query.AsQueryable().Where(predicate);
        //}

        //return query;

        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        IEnumerable<WatchListInfoModel> infoQuery = from s in _context.SL_WATCHLIST
                                                    join t in _context.SL_TV_EPISODE_INFO on new { Id = s.INFO_ID ?? -1, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = t.TV_EPISODE_INFO_ID, Type = INFO_TYPE.TV } into ts
                                               from t in ts.DefaultIfEmpty()
                                               join ti in _context.SL_TV_INFO on new { Id = t.TV_INFO_ID, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = ti.TV_INFO_ID, Type = INFO_TYPE.TV } into tis
                                               from ti in tis.DefaultIfEmpty()
                                               join m in _context.SL_MOVIE_INFO on new { Id = s.INFO_ID ?? -1, Type = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? INFO_TYPE.TV : INFO_TYPE.MOVIE } equals new { Id = m.MOVIE_INFO_ID, Type = INFO_TYPE.MOVIE } into ms
                                               from m in ms.DefaultIfEmpty()
                                               select new WatchListInfoModel
                                               {
                                                   WatchlistId = s.WATCHLIST_ID,
                                                   UserId = s.USER_ID,
                                                   ShowName = s.SHOW_NAME,
                                                   SeasonNumber = s.SEASON_NUMBER,
                                                   EpisodeNumber = s.EPISODE_NUMBER,
                                                   DateAdded = s.DATE_ADDED,
                                                   ShowTypeId = s.SHOW_TYPE_ID,
                                                   ShowTypeIdZ = showTypeIds[s.SHOW_TYPE_ID],
                                                   ShowNotes = s.SHOW_NOTES,
                                                   Runtime = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.RUNTIME : null) : (m != null ? m.RUNTIME : null),
                                                   EpisodeName = s.SHOW_TYPE_ID == (int)CodeValueIds.TV ? (t != null ? t.EPISODE_NAME : null) : "",
                                                   InfoId = s.INFO_ID,

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

        IEnumerable<DetailedWatchListModel> query = infoQuery.ToList().Select(m =>
        {
            m.ImageUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoImageUrl);
            m.InfoUrl = m.ShowTypeId == (int)CodeValueIds.TV ? _apisConfig.GetTvEpisodeInfoUrl(m.InfoApiType, m.InfoApiId, m.InfoSeasonNumber, m.InfoEpisodeNumber) : _apisConfig.GetMovieInfoUrl(m.InfoApiType, m.InfoApiId);
            return m;
        });

        return query;
    }

    public IEnumerable<WatchListModel> SearchWatchLists(int userId, string text)
    {
        DateTime dateSearch;
        IEnumerable<WatchListModel> query;
        Dictionary<string, int> showTypeIds = _context.SL_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.DECODE_TXT.ToLower(), m => m.CODE_VALUE_ID);

        string[] showTypes = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID).Select(m => m.DECODE_TXT.ToLower()).ToArray();

        if (DateTime.TryParse(text, out dateSearch))
        {
            query = GetWatchLists(m => m.DateAdded.Date == dateSearch.Date);
        }
        else if (showTypeIds.ContainsKey(text.ToLower()))
        {
            query = GetWatchLists(m => m.ShowTypeId == showTypeIds[text.ToLower()]);
        }
        else
        {
            query = GetWatchLists(m => m.ShowName.ToLower().Contains(text.ToLower()));
        }

        return query;
    }

    public int CreateWatchList(int userId, WatchListModel model, int? infoId = null)
    {
        SL_WATCHLIST entity = new SL_WATCHLIST
        {
            SHOW_TYPE_ID = model.ShowTypeId,
            DATE_ADDED = model.DateAdded.Date,
            EPISODE_NUMBER = model.ShowTypeId == (int)CodeValueIds.TV ? model.EpisodeNumber : null,
            SEASON_NUMBER = model.ShowTypeId == (int)CodeValueIds.TV ? model.SeasonNumber : null,
            SHOW_NAME = model.ShowName,
            SHOW_NOTES = model.ShowNotes,
            USER_ID = userId,
            INFO_ID = infoId,
        };

        _context.SL_WATCHLIST.Add(entity);
        _context.SaveChanges();
        int id = entity.WATCHLIST_ID;

        return id;
    }

    public int UpdateWatchList(int userId, WatchListModel model)
    {
        SL_WATCHLIST? entity = _context.SL_WATCHLIST.FirstOrDefault(m => m.WATCHLIST_ID == model.WatchlistId && m.USER_ID == userId);

        if (entity != null)
        {
            if (entity.INFO_ID != null
                && (entity.EPISODE_NUMBER != model.EpisodeNumber
                || entity.SEASON_NUMBER != model.SeasonNumber))
            {
                entity.INFO_ID = GetTvEpisodeInfoId(entity.INFO_ID, model.SeasonNumber, model.EpisodeNumber);
            }

            entity.SHOW_TYPE_ID = model.ShowTypeId;
            entity.EPISODE_NUMBER = model.EpisodeNumber;
            entity.SEASON_NUMBER = model.SeasonNumber;
            entity.SHOW_NAME = model.ShowName;
            entity.SHOW_NOTES = model.ShowNotes;

            return _context.SaveChanges();
        }

        return 0;
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

    public bool DeleteWatchList(int userId, int watchListId)
    {
        bool result = false;
        SL_WATCHLIST? entity = _context.SL_WATCHLIST.FirstOrDefault(m => m.WATCHLIST_ID == watchListId && m.USER_ID == userId);

        if (entity != null)
        {
            _context.SL_WATCHLIST.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }

    public bool MoveToShows(int userId, int watchListId, DateTime dateWatched)
    {
        bool result = false;
        SL_WATCHLIST? entity = _context.SL_WATCHLIST.FirstOrDefault(m => m.WATCHLIST_ID == watchListId && m.USER_ID == userId);

        if (entity != null)
        {
            SL_SHOW newShowEntity = new SL_SHOW
            {
                SHOW_TYPE_ID = entity.SHOW_TYPE_ID,
                DATE_WATCHED = dateWatched.Date,
                EPISODE_NUMBER = entity.SHOW_TYPE_ID == (int)CodeValueIds.TV ? entity.EPISODE_NUMBER : null,
                SEASON_NUMBER = entity.SHOW_TYPE_ID == (int)CodeValueIds.TV ? entity.SEASON_NUMBER : null,
                SHOW_NAME = entity.SHOW_NAME,
                SHOW_NOTES = entity.SHOW_NOTES,
                USER_ID = userId,
                INFO_ID = entity.INFO_ID,
            };

            _context.SL_SHOW.Add(newShowEntity);

            _context.SaveChanges();
            
            if(newShowEntity.SHOW_ID > 0)
            {
                result = DeleteWatchList(userId, watchListId);
            }
        }

        return result;
    }
}
