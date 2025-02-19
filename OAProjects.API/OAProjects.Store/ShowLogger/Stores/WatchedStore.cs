using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Watched;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace OAProjects.Store.ShowLogger.Stores;
public class WatchedStore : IWatchedStore
{
    private readonly ShowLoggerDbContext _context;
    private readonly ApisConfig _apisConfig;

    public WatchedStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public IEnumerable<WatchedModel> GetWatchedTV(int userId, Expression<Func<WatchedModel, bool>>? predicate = null)
    {
        SL_SHOW[] data = [.. _context.SL_SHOW.Where(m => m.USER_ID == userId && m.SHOW_TYPE_ID == (int)CodeValueIds.TV)];

        IEnumerable<WatchedInfoModel> shows = from s in data
                                              join e in _context.SL_TV_EPISODE_INFO on s.INFO_ID equals e.TV_EPISODE_INFO_ID
                                              join t in _context.SL_TV_INFO on e.TV_INFO_ID equals t.TV_INFO_ID
                                              group s by new { t, s.USER_ID } into grp
                                              select new WatchedInfoModel
                                              {
                                                  WatchedId = grp.Max(m => m.SHOW_ID) * -1,
                                                  UserId = grp.Key.USER_ID,
                                                  Name = grp.Key.t.SHOW_NAME,
                                                  DateWatched = grp.Max(m => m.DATE_WATCHED),
                                                  InfoId = grp.Key.t.TV_INFO_ID,
                                                  InfoType = (int)INFO_TYPE.TV,
                                                  InfoTypeIdZ = "TV",
                                                  InfoApiType = grp.Key.t.API_TYPE,
                                                  InfoApiId = grp.Key.t.API_ID,
                                                  InfoBackdropUrl = grp.Key.t.BACKDROP_URL,
                                              };


        IEnumerable<WatchedInfoModel> watched = from s in _context.SL_WATCHED
                                                join t in _context.SL_TV_INFO on new { Id = s.INFO_ID, Type = s.INFO_TYPE } equals new { Id = t.TV_INFO_ID, Type = (int)INFO_TYPE.TV } into ts
                                                from t in ts.DefaultIfEmpty()
                                                where s.USER_ID == userId
                                                && s.INFO_TYPE == (int)INFO_TYPE.TV
                                                select new WatchedInfoModel
                                                {
                                                    WatchedId = s.WATCHED_ID,
                                                    UserId = s.USER_ID,
                                                    Name = t.SHOW_NAME,
                                                    DateWatched = s.DATE_WATCHED,
                                                    InfoId = s.INFO_ID,
                                                    InfoType = (int)INFO_TYPE.TV,
                                                    InfoTypeIdZ = "TV",
                                                    InfoApiType = t.API_TYPE,
                                                    InfoApiId = t.API_ID,
                                                    InfoBackdropUrl = t.BACKDROP_URL,
                                                };

        IEnumerable<WatchedModel> query = shows.Distinct().Union(watched).ToList().Select(m =>
        {
            m.BackdropUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoBackdropUrl);
            m.InfoUrl = _apisConfig.GetTvInfoUrl(m.InfoApiType, m.InfoApiId);
            return m;
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public IEnumerable<WatchedModel> GetWatchedMovies(int userId, Expression<Func<WatchedModel, bool>>? predicate = null)
    {
        int[] codeValues = [(int)CodeValueIds.MOVIE, (int)CodeValueIds.AMC];
        SL_SHOW[] data = [.. _context.SL_SHOW.Where(m => m.USER_ID == userId && codeValues.Contains(m.SHOW_TYPE_ID))];


        IEnumerable<WatchedInfoModel> movies = from s in data
                                               join m in _context.SL_MOVIE_INFO on s.INFO_ID equals m.MOVIE_INFO_ID
                                               where s.USER_ID == userId
                                               group s by new { m, s.USER_ID } into grp
                                               select new WatchedInfoModel
                                               {
                                                   WatchedId = grp.Max(m => m.SHOW_ID) * -1,
                                                   UserId = grp.Key.USER_ID,
                                                   Name = grp.Key.m.MOVIE_NAME,
                                                   DateWatched = grp.Max(n => n.DATE_WATCHED),
                                                   InfoId = grp.Key.m.MOVIE_INFO_ID,
                                                   InfoType = (int)INFO_TYPE.MOVIE,
                                                   InfoTypeIdZ = "MOVIE",
                                                   InfoApiType = grp.Key.m.API_TYPE,
                                                   InfoApiId = grp.Key.m.API_ID,
                                                   InfoBackdropUrl = grp.Key.m.BACKDROP_URL,
                                               };

        IEnumerable<WatchedInfoModel> watched = from s in _context.SL_WATCHED
                                          join m in _context.SL_MOVIE_INFO on new { Id = s.INFO_ID, Type = s.INFO_TYPE } equals new { Id = m.MOVIE_INFO_ID, Type = (int)INFO_TYPE.MOVIE } into ms
                                          from m in ms.DefaultIfEmpty()
                                          where s.USER_ID == userId
                                          && s.INFO_TYPE == (int)INFO_TYPE.MOVIE
                                          select new WatchedInfoModel
                                          {
                                              WatchedId = s.WATCHED_ID,
                                              UserId = s.USER_ID,
                                              Name = m.MOVIE_NAME,
                                              DateWatched = s.DATE_WATCHED,
                                              InfoId = s.INFO_ID,
                                              InfoType = s.INFO_TYPE,
                                              InfoTypeIdZ = "Movie",
                                              InfoApiType = m.API_TYPE,
                                              InfoApiId = m.API_ID,
                                              InfoBackdropUrl = m.BACKDROP_URL,
                                          };

        IEnumerable<WatchedModel> query = movies.Union(watched).ToList().Select(m =>
        {
            m.BackdropUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoBackdropUrl);
            m.InfoUrl = _apisConfig.GetMovieInfoUrl(m.InfoApiType, m.InfoApiId);
            return m;
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public IEnumerable<WatchedModel> GetWatched(Expression<Func<WatchedModel, bool>>? predicate = null)
    {
        IEnumerable<WatchedInfoModel> data = from s in _context.SL_WATCHED
                                                join t in _context.SL_TV_INFO on new { Id = s.INFO_ID, Type = s.INFO_TYPE } equals new { Id = t.TV_INFO_ID, Type = (int)INFO_TYPE.TV } into ts
                                                from t in ts.DefaultIfEmpty()
                                                join m in _context.SL_MOVIE_INFO on new { Id = s.INFO_ID, Type = s.INFO_TYPE } equals new { Id = m.MOVIE_INFO_ID, Type = (int)INFO_TYPE.MOVIE } into ms
                                                from m in ms.DefaultIfEmpty()
                                                select new WatchedInfoModel
                                                {
                                                    WatchedId = s.WATCHED_ID,
                                                    UserId = s.USER_ID,
                                                    Name = s.INFO_TYPE == (int)INFO_TYPE.TV ? t.SHOW_NAME : m.MOVIE_NAME,
                                                    DateWatched = s.DATE_WATCHED,
                                                    InfoId = s.INFO_ID,
                                                    InfoType = s.INFO_TYPE,
                                                    InfoTypeIdZ = s.INFO_TYPE == (int)INFO_TYPE.TV ? "TV" : "MOVIE",
                                                    InfoApiType = s.INFO_TYPE == (int)INFO_TYPE.TV ? t.API_TYPE : m.API_TYPE,
                                                    InfoApiId = s.INFO_TYPE == (int)INFO_TYPE.TV ? t.API_ID : m.API_ID,
                                                    InfoBackdropUrl = s.INFO_TYPE == (int)INFO_TYPE.TV ? t.POSTER_URL : m.POSTER_URL,
                                                };

        IEnumerable<WatchedModel> query = data.ToList().Select(m =>
        {
            m.BackdropUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoBackdropUrl);
            m.InfoUrl = _apisConfig.GetMovieInfoUrl(m.InfoApiType, m.InfoApiId);
            return m;
        });

        if (predicate != null)
        {
            query = data.AsQueryable().Where(predicate);
        }

        return query;
    }

    public int CreateWatched(int userId, WatchedModel model, int? infoId = null)
    {
        SL_WATCHED entity = new SL_WATCHED
        {
            USER_ID = userId,
            INFO_ID = model.InfoId,
            INFO_TYPE = model.InfoType,
            DATE_WATCHED = model.DateWatched,
        };

        _context.SL_WATCHED.Add(entity);
        _context.SaveChanges();
        int id = entity.WATCHED_ID;

        return id;
    }

    public int UpdateWatched(int userId, WatchedModel model)
    {
        SL_WATCHED? entity = _context.SL_WATCHED.FirstOrDefault(m => m.WATCHED_ID == model.WatchedId && m.USER_ID == userId);

        int updated = 0;

        if (entity != null)
        {
            entity.DATE_WATCHED = model.DateWatched;

            updated = _context.SaveChanges();
        }

        return updated;
    }

    public bool DeleteWatched(int userId, int watchedId)
    {
        bool result = false;
        SL_WATCHED? entity = _context.SL_WATCHED.FirstOrDefault(m => m.WATCHED_ID == watchedId && m.USER_ID == userId);

        if (entity != null)
        {
            _context.SL_WATCHED.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }
}
