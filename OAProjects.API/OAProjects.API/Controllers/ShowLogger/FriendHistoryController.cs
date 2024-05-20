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
[Route("api/[controller]")]
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

    [HttpGet("Get")]
    public async Task<IActionResult> Get(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<FriendHistoryGetResponse> response = new GetResponse<FriendHistoryGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new FriendHistoryGetResponse();

            response.Model.FriendHistory = GetData(userId, search);
            response.Model.Count = response.Model.FriendHistory.Count();
            response.Model.FriendHistory = response.Model.FriendHistory.OrderByDescending(m => m.Show.ShowId).ThenByDescending(m => m.Show.DateWatched).ThenByDescending(m => m.Show.ShowName).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<FriendHistoryModel> GetData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserLookUps();
        IEnumerable<FriendHistoryModel> query = _friendHistoryStore.GetFriendHistory(userId, userLookUps);

        Expression<Func<FriendHistoryModel, bool>>? predicate = null;

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
}
