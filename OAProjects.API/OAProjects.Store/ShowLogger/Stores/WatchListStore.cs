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

namespace OAProjects.Store.ShowLogger.Stores;
public class WatchListStore : IWatchListStore
{
    private readonly ShowLoggerDbContext _context;

    public WatchListStore(ShowLoggerDbContext context)
    {
        _context = context;
    }

    public IEnumerable<WatchListModel> GetWatchLists(Expression<Func<WatchListModel, bool>>? predicate = null)
    {
        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID).ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        IEnumerable<WatchListModel> query = _context.SL_WATCHLIST.Select(m => new WatchListModel
        {
            WatchlistId = m.WATCHLIST_ID,
            UserId = m.USER_ID,
            ShowName = m.SHOW_NAME,
            SeasonNumber = m.SEASON_NUMBER,
            EpisodeNumber = m.EPISODE_NUMBER,
            DateAdded = m.DATE_ADDED,
            ShowTypeId = m.SHOW_TYPE_ID,
            ShowTypeIdZ = showTypeIds[m.SHOW_TYPE_ID],
            ShowNotes = m.SHOW_NOTES,
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

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

    public int CreateWatchList(int userId, WatchListModel model)
    {
        SL_WATCHLIST entity = new SL_WATCHLIST
        {
            SHOW_TYPE_ID = model.ShowTypeId,
            DATE_ADDED = DateTime.Now.Date,
            EPISODE_NUMBER = model.ShowTypeId == (int)CodeValueIds.TV ? model.EpisodeNumber : null,
            SEASON_NUMBER = model.ShowTypeId == (int)CodeValueIds.TV ? model.SeasonNumber : null,
            SHOW_NAME = model.ShowName,
            SHOW_NOTES = model.ShowNotes,
            USER_ID = userId
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
            entity.SHOW_TYPE_ID = model.ShowTypeId;
            entity.EPISODE_NUMBER = model.EpisodeNumber;
            entity.SEASON_NUMBER = model.SeasonNumber;
            entity.SHOW_NAME = model.ShowName;
            entity.SHOW_NOTES = model.ShowNotes;

            return _context.SaveChanges();
        }

        return 0;
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
}
