using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.Store.ShowLogger.Stores;
public class ShowStore : IShowStore
{
    private readonly ShowLoggerDbContext _context;

    public ShowStore(ShowLoggerDbContext context)
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

    public IEnumerable<ShowModel> GetShows(Expression<Func<ShowModel, bool>>? predicate = null)
    {
        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        IEnumerable<ShowModel> query = _context.SL_SHOW.Select(m => new ShowModel
        {
            ShowId = m.SHOW_ID,
            UserId = m.USER_ID,
            ShowName = m.SHOW_NAME,
            SeasonNumber = m.SEASON_NUMBER,
            EpisodeNumber = m.EPISODE_NUMBER,
            DateWatched = m.DATE_WATCHED,
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

        return id;
    }

    public int UpdateShow(int userId, ShowModel model)
    {
        SL_SHOW? entity = _context.SL_SHOW.FirstOrDefault(m => m.SHOW_ID == model.ShowId && m.USER_ID == userId);

        if (entity != null)
        {
            entity.SHOW_TYPE_ID = model.ShowTypeId;
            entity.DATE_WATCHED = model.DateWatched.Date;
            entity.EPISODE_NUMBER = model.EpisodeNumber;
            entity.SEASON_NUMBER = model.SeasonNumber;
            entity.SHOW_NAME = model.ShowName;
            entity.SHOW_NOTES = model.ShowNotes;

            return _context.SaveChanges();
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
}
