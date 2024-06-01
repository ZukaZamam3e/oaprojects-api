using Azure.Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.ShowLogger.Models.Login;
using OAProjects.Models.ShowLogger.Requests.Show;
using OAProjects.Models.ShowLogger.Responses.Login;
using OAProjects.Models.ShowLogger.Responses.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class LoginController : BaseController
{
    private readonly ILogger<LoginController> _logger;
    private readonly ILoginStore _loginStore;

    public LoginController(ILogger<LoginController> logger,
        IUserStore userStore,
        ILoginStore loginStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _loginStore = loginStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<LoginLoadResponse> response = new GetResponse<LoginLoadResponse>();

        try
        {
            int userId = await GetUserId();
            response.Model = new LoginLoadResponse();

            response.Model.UserPref = _loginStore.GetUserPref(userId);
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
}
