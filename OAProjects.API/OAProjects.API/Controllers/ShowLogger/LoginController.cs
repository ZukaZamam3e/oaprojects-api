using Azure.Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Net.Http.Headers;
using Newtonsoft.Json;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.OAIdentity;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Login;
using OAProjects.Models.ShowLogger.Requests.Show;
using OAProjects.Models.ShowLogger.Responses.Auth;
using OAProjects.Models.ShowLogger.Responses.Login;
using OAProjects.Models.ShowLogger.Responses.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Net.Http;
using System.Text;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class LoginController : BaseController
{
    private readonly ILogger<LoginController> _logger;
    private readonly ILoginStore _loginStore;
    private readonly Auth0Config _auth0Config;

    public LoginController(ILogger<LoginController> logger,
        IUserStore userStore,
        ILoginStore loginStore,
        Auth0Config auth0Config,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _loginStore = loginStore;
        _auth0Config = auth0Config;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<LoginLoadResponse> response = new GetResponse<LoginLoadResponse>();

        try
        {
            int userId = await GetUserId();
            response.Model = new LoginLoadResponse();

            string[] roles = await GetRoles();

            response.Model.UserPref = _loginStore.GetUserPref(userId);
            response.Model.UserPref.HasAdminRole = roles.Any(m => m == "Admin");
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save(UserPrefModel model,
        [FromServices] IValidator<UserPrefModel> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                response.Model = _loginStore.UpdateUserPref(userId, model);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private async Task<string[]> GetRoles()
    {
        string[] roles = [];

        string accessToken = Request.Headers[HeaderNames.Authorization];

        UserModel? user = _userStore.GetUserByToken(accessToken);

        if (user != null)
        {
            string apiAccessToken = await GetAuthToken();
            // if empty, get user information from Auth0
            HttpClient httpClient = _httpClientFactory.CreateClient("Auth0");

            httpClient.DefaultRequestHeaders.Add(HeaderNames.Authorization, apiAccessToken);
            string id = HttpContext.User.FindFirst("http://schemas.xmlsoap.org/ws/2005/05/identity/claims/nameidentifier").Value;

            HttpResponseMessage response = await httpClient.GetAsync($"/api/v2/users/{id}/roles");

            if (response.IsSuccessStatusCode)
            {
                // Retrieved user infomation back
                // See if email exist in database
                // If not, add it
                // Then add token to database

                string responseText = await response.Content.ReadAsStringAsync();
                IEnumerable<GetRolesResponse> roleData = JsonConvert.DeserializeObject<IEnumerable<GetRolesResponse>>(responseText);
                roles = roleData.Select(m => m.Name).ToArray();
                int x = 0;
            }
        }

        return roles;
    }

    private async Task<string> GetAuthToken()
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("Auth0");

        using StringContent jsonContent = new(
            System.Text.Json.JsonSerializer.Serialize(new
            {
                client_id = _auth0Config.Auth0ClientId,
                client_secret = _auth0Config.Auth0ClientSecret,
                audience = _auth0Config.Auth0Audience,
                grant_type = _auth0Config.Auth0GrantType,
                scopes = _auth0Config.Auth0Scopes
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync($"oauth/token", jsonContent);

        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();

        AuthTokenResponse? authToken = JsonConvert.DeserializeObject<AuthTokenResponse>(result);

        string token = "";

        if (authToken != null)
        {
            token = $"{authToken.TokenType} {authToken.AccessToken}";
        }

        return token;
    }
}
