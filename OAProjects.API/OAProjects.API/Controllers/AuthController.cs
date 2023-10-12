using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using OAProjects.API.Responses;
using OAProjects.Store.OAIdentity.Stores.Interfaces;

namespace OAProjects.API.Controllers;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
public class AuthController : BaseController
{
    private readonly ILogger<AuthController> _logger;

    public AuthController(ILogger<AuthController> logger,
        IUserStore userStore)
        : base(logger, userStore)
    {
        _logger = logger;
    }

    [HttpGet("Login")]
    [RequiredScopeOrAppPermission(
         RequiredScopesConfigurationKey = "AzureAD:Scopes:User.ReadWrite",
         RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:User.ReadWrite"
     )]
    public async Task<IActionResult> Login()
    {
        GetResponse<bool> response = new GetResponse<bool>();

        try
        {
            int userId = GetUserId();
            response.Model = userId > 0;
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }


}
