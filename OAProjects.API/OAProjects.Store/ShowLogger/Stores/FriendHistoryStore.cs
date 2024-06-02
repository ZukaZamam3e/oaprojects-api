using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Book;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.FriendHistory;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores;
public class FriendHistoryStore : IFriendHistoryStore
{
    private readonly ShowLoggerDbContext _context;
    private readonly ApisConfig _apisConfig;

    public FriendHistoryStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
        _apisConfig = apisConfig;
    }

    public IEnumerable<ShowFriendHistoryModel> GetShowFriendHistory(int userId, Dictionary<int, string> users)
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

        IEnumerable<DetailedShowModel> showQuery = infoQuery.ToList().Select(m =>
        {
            m.ImageUrl = _apisConfig.GetImageUrl(m.InfoApiType, m.InfoImageUrl);
            m.InfoUrl = m.ShowTypeId == (int)CodeValueIds.TV ? _apisConfig.GetTvEpisodeInfoUrl(m.InfoApiType, m.InfoApiId, m.InfoSeasonNumber, m.InfoEpisodeNumber) : _apisConfig.GetMovieInfoUrl(m.InfoApiType, m.InfoApiId);
            return m;
        });

        List<ShowFriendHistoryModel> friendQuery = showQuery.ToList().Select(m => new ShowFriendHistoryModel
        {
            Show = m,
            Name = users[m.UserId]
        }).ToList();

        return friendQuery;
    }

    public IEnumerable<BookFriendHistoryModel> GetBookFriendHistory(int userId, Dictionary<int, string> users)
    {
        int[] friends = _context.SL_FRIEND.Where(m => m.USER_ID == userId).Select(m => m.FRIEND_USER_ID)
            .Union(_context.SL_FRIEND.Where(m => m.FRIEND_USER_ID == userId).Select(m => m.USER_ID)).ToArray();

        IEnumerable<BookFriendHistoryModel> query = _context.SL_BOOK.Where(m => friends.Contains(m.USER_ID)).ToList()
            .Select(m => new BookFriendHistoryModel
            {
                Book = new BookModel
                {
                    BookId = m.BOOK_ID,
                    UserId = m.USER_ID,
                    BookName = m.BOOK_NAME,
                    StartDate = m.START_DATE,
                    EndDate = m.END_DATE,
                    Chapters = m.CHAPTERS,
                    Pages = m.PAGES,
                    BookNotes = m.BOOK_NOTES,
                },
                Name = users[m.USER_ID]
            });

        return query;
    }

}
