using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.ShowLogger.Requests.Friend;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Models.ShowLogger.Models.Friend;
using FluentValidation.Results;
using OAProjects.Models.ShowLogger.Responses.Friend;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.ShowLogger.Models.Stat;
using OAProjects.Store.ShowLogger.Stores;
using System.Linq.Expressions;
using System.Linq;
using OAProjects.Models.OAIdentity;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class FriendController : BaseController
{
    private readonly ILogger<FriendController> _logger;
    private readonly IFriendStore _friendStore;

    public FriendController(ILogger<FriendController> logger,
        IUserStore userStore,
        IFriendStore friendStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _friendStore = friendStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<FriendLoadResponse> response = new GetResponse<FriendLoadResponse>();

        try
        {
            int userId = await GetUserId();
            response.Model = new FriendLoadResponse();

            response.Model.Friends = GetData(userId);
            response.Model.Count = response.Model.Friends.Count();
            response.Model.Friends = response.Model.Friends.OrderByDescending(m => m.IsPending).ThenBy(m => m.FriendEmail).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<FriendGetResponse> response = new GetResponse<FriendGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new FriendGetResponse();

            response.Model.Friends = GetData(userId, search);
            response.Model.Count = response.Model.Friends.Count();
            response.Model.Friends = response.Model.Friends.OrderByDescending(m => m.IsPending).ThenBy(m => m.FriendName).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<FriendModel> GetData(int userId, string? search = null)
    {
        Dictionary<int, UserModel> userLookUps = _userStore.GetUserLookUps();
        IEnumerable<FriendModel> query = _friendStore.GetFriends(userId, userLookUps);

        Expression<Func<FriendModel, bool>>? predicate = null;
        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.FriendName.ToLower().Contains(search.ToLower())
                || m.FriendEmail.ToLower().Contains(search.ToLower());
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }

        return query;
    }

    [HttpPost("AddFriend")]
    public async Task<IActionResult> AddFriend(
        AddFriendModel request, 
        [FromServices] IValidator<AddFriendModel> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int userId = await GetUserId();

                int? friendId = _userStore.GetUserByEmail(request.Email)?.UserId;

                if (friendId == null)
                {
                    throw new Exception("Could not find user by email.");
                }

                bool successful = _friendStore.SendFriendRequest(userId, friendId.Value);

                if (successful == false)
                {
                    throw new Exception("Unable to send friend request.");
                }

                response.Model = successful;
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("AcceptFriendRequest")]
    public async Task<IActionResult> AcceptFriendRequest(
        FriendRequestIdRequest request, 
        [FromServices] IValidator<FriendRequestIdRequest> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int userId = await GetUserId();

                bool successful = _friendStore.AcceptFriendRequest(userId, request.FriendRequestId);

                if (successful == false)
                {
                    throw new Exception("Unable to accept friend request.");
                }

                response.Model = successful;
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("DenyFriendRequest")]
    public async Task<IActionResult> DenyFriendRequest(
        FriendRequestIdRequest request, 
        [FromServices] IValidator<FriendRequestIdRequest> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int userId = await GetUserId();

                bool successful = _friendStore.DenyFriendRequest(userId, request.FriendRequestId);

                if (successful == false)
                {
                    throw new Exception("Unable to deny friend request.");
                }

                response.Model = successful;
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> DeleteFriend(FriendIdRequest request,
        [FromServices] IValidator<FriendIdRequest> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int userId = await GetUserId();

                bool successful = _friendStore.DeleteFriend(userId, request.FriendId);

                if (successful == false)
                {
                    throw new Exception("Unable to delete friend.");
                }

                response.Model = successful;
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
