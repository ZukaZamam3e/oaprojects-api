using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using OAProjects.Models.OAIdentity;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using System.Security.Claims;

namespace OAProjects.API.Controllers;

public class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;
    protected readonly IUserStore _userStore;
    //protected int _userId;
    public BaseController(ILogger<BaseController> logger,
        IUserStore userStore)
    {
        _logger = logger;
        _userStore = userStore;
    }

    protected bool IsAppMakingRequest()
    {
        // Add in the optional 'idtyp' claim to check if the access token is coming from an application or user.
        // See: https://docs.microsoft.com/en-us/azure/active-directory/develop/active-directory-optional-claims
        if (HttpContext.User.Claims.Any(c => c.Type == "idtyp"))
        {
            return HttpContext.User.Claims.Any(c => c.Type == "idtyp" && c.Value == "app");
        }
        else
        {
            // alternatively, if an AT contains the roles claim but no scp claim, that indicates it's an app token
            return HttpContext.User.Claims.Any(c => c.Type == "roles") && !HttpContext.User.Claims.Any(c => c.Type == "scp");
        }
    }

    protected Guid GetGuidUserId()
    {
        Guid userId;

        if (!Guid.TryParse(HttpContext.User.GetObjectId(), out userId))
        {
            throw new Exception("User ID is not valid.");
        }

        return userId;
    }

    protected int GetUserId()
    {
        Guid userId;

        string accessToken = Request.Headers[HeaderNames.Authorization];

        if (!Guid.TryParse(HttpContext.User.GetObjectId(), out userId))
        {
            throw new Exception("User ID is not valid.");
        }

        UserModel user = _userStore.GetUser(null, userId.ToString());

        if(user == null)
        {
            string userName = GetUserClaim("name");
            user = _userStore.AddUser(userId.ToString(), userName, "AZURE");
        }

        return user.UserId;
    }

    protected string GetUserClaim(string claimType)
    {
        Claim? claim = HttpContext.User.FindFirst(claimType);

        if (claim == null)
        {
            throw new Exception($"{claimType} is not accessible.");
        }

        return claim.Value;
    }

    protected bool RequestCanAccessToDo(Guid userId)
    {
        return IsAppMakingRequest() || (userId == GetGuidUserId());
    }
}
