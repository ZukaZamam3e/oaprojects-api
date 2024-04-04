using Azure.Core;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OAProjects.Models.OAIdentity;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using System;
using System.Net.Http;
using System.Security.Claims;
using System.Text.Json.Serialization;

namespace OAProjects.API.Controllers;

public class BaseController : ControllerBase
{
    private readonly ILogger<BaseController> _logger;
    protected readonly IUserStore _userStore;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly int UserId;

    //protected int _userId;
    public BaseController(ILogger<BaseController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory)
    {
        _logger = logger;
        _userStore = userStore;
        _httpClientFactory = httpClientFactory;
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

    protected async Task<int> GetUserId()
    {
        int userId = -1;

        string accessToken = Request.Headers[HeaderNames.Authorization];
        string exp = HttpContext.User.FindFirst("exp").Value;
        string iat = HttpContext.User.FindFirst("iat").Value;



        //DateTime origin = new DateTime(1970, 1, 1, 0, 0, 0, 0, DateTimeKind.);
        int expiryTime = int.Parse(exp);
        int issuedAtTime = int.Parse(iat);

        DateTime expiryDate = DateTimeOffset.FromUnixTimeSeconds(expiryTime).UtcDateTime;
        DateTime issuedAtDate = DateTimeOffset.FromUnixTimeSeconds(issuedAtTime).UtcDateTime;


        // Try to load user's token from the database.
        UserModel? user = _userStore.GetUserByToken(accessToken);

        if (user == null)
        {
            // if empty, get user information from Auth0
            HttpClient httpClient = _httpClientFactory.CreateClient("Auth0");

            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, accessToken);

            HttpResponseMessage response = await httpClient.GetAsync("/userinfo");

            if(response.IsSuccessStatusCode)
            {
                // Retrieved user infomation back
                // See if email exist in database
                // If not, add it
                // Then add token to database

                string responseText = await response.Content.ReadAsStringAsync();
                Auth0UserInfoModel? userInfo = JsonConvert.DeserializeObject<Auth0UserInfoModel>(responseText);

                if (userInfo != null)
                {
                    user = _userStore.GetUserByEmail(userInfo.Email);

                    if (user == null)
                    {
                        user = _userStore.AddUser(new UserModel
                        {
                            Email = userInfo.Email,
                            FirstName = userInfo.GivenName,
                            LastName = userInfo.FamilyName,
                            UserName = userInfo.NickName
                        });
                    }

                    _userStore.AddToken(new UserTokenModel
                    {
                        UserId = user.UserId,
                        Token = accessToken,
                        ExpiryTime = expiryTime,
                        IssuedAt = issuedAtTime,
                        ExpiryDateUtc = expiryDate,
                        IssuedAtDateUtc = issuedAtDate
                    });
                }

                userId = user.UserId;
            }
        }
        else
        {
            userId = user.UserId;
        }

        return userId;
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
