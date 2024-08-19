using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Responses.FriendHistory;
using OAProjects.Models.ShowLogger.Models.FriendHistory;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.Common.Responses;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class FriendHistoryController : BaseController
{
    private readonly ILogger<StatController> _logger;
    private readonly IFriendHistoryStore _friendHistoryStore;
    private readonly ICodeValueStore _codeValueStore;
    public FriendHistoryController(ILogger<StatController> logger,
        IUserStore userStore,
        IFriendHistoryStore friendHistoryStore,
        ICodeValueStore codeValueStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _friendHistoryStore = friendHistoryStore;
        _codeValueStore = codeValueStore;
    }

    [HttpGet("GetShows")]
    public async Task<IActionResult> GetShows(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<ShowFriendHistoryGetResponse> response = new GetResponse<ShowFriendHistoryGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new ShowFriendHistoryGetResponse();

            response.Model.ShowFriendHistory = GetShowData(userId, search);
            response.Model.Count = response.Model.ShowFriendHistory.Count();
            response.Model.ShowFriendHistory = response.Model.ShowFriendHistory.OrderByDescending(m => m.Show.ShowId).ThenByDescending(m => m.Show.DateWatched).ThenByDescending(m => m.Show.ShowName).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<ShowFriendHistoryModel> GetShowData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserNameLookUps();
        IEnumerable<ShowFriendHistoryModel> query = _friendHistoryStore.GetShowFriendHistory(userId, userLookUps);

        Expression<Func<ShowFriendHistoryModel, bool>>? predicate = null;

        DateTime dateSearch;

        Dictionary<string, int> showTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.DecodeTxt.ToLower(), m => m.CodeValueId);

        if (!string.IsNullOrEmpty(search))
        {
            if (DateTime.TryParse(search, out dateSearch))
            {
                predicate = m => m.Show.DateWatched.Date == dateSearch.Date;
            }
            else if (showTypeIds.ContainsKey(search.ToLower()))
            {
                predicate = m => m.Show.ShowTypeId == showTypeIds[search.ToLower()];
            }
            else
            {
                predicate = m => 
                    (m.Show.ShowName.ToLower().Contains(search.ToLower())
                     || m.Name.ToLower().Contains(search.ToLower()));
            }
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }

        return query;
    }

    [HttpGet("GetBooks")]
    public async Task<IActionResult> GetBooks(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<BookFriendHistoryResponse> response = new GetResponse<BookFriendHistoryResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new BookFriendHistoryResponse();

            response.Model.BookFriendHistory = GetBookData(userId, search);
            response.Model.Count = response.Model.BookFriendHistory.Count();
            response.Model.BookFriendHistory = response.Model.BookFriendHistory.OrderByDescending(m => m.Book.EndDate == null).ThenByDescending(m => m.Book.EndDate).ThenByDescending(m => m.Book.StartDate).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<BookFriendHistoryModel> GetBookData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserNameLookUps();
        IEnumerable<BookFriendHistoryModel> query = _friendHistoryStore.GetBookFriendHistory(userId, userLookUps);

        Expression<Func<BookFriendHistoryModel, bool>>? predicate = null;

        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            if (DateTime.TryParse(search, out dateSearch))
            {
                predicate = m => (m.Book.StartDate != null ? m.Book.StartDate.Value.Date == dateSearch.Date : true)
                    || (m.Book.EndDate != null ? m.Book.EndDate.Value.Date == dateSearch.Date : true);
            }
            else
            {
                predicate = m =>
                    (m.Book.BookName.ToLower().Contains(search.ToLower())
                     || m.Name.ToLower().Contains(search.ToLower()));
            }
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }

        return query;
    }
}
