using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Web.Resource;
using OAProjects.API.Requests.Show;
using OAProjects.API.Responses;
using OAProjects.API.Responses.Show;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class ShowController : BaseController
{
    private readonly ILogger<ShowController> _logger;
    private readonly IShowStore _showStore;
    private readonly IValidator<ShowModel> _validator;

    public ShowController(ILogger<ShowController> logger,
        IUserStore userStore,
        IShowStore showStore,
        IHttpClientFactory httpClientFactory,
        IValidator<ShowModel> validator)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _showStore = showStore;
        _validator = validator;
    }

    [HttpGet("Load")]
    //[RequiredScopeOrAppPermission(
    //    RequiredScopesConfigurationKey = "AzureAD:Scopes:User.ReadWrite",
    //    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:User.ReadWrite"
    //)]
    public async Task<IActionResult> Load()
    {
        GetResponse<ShowLoadResponse> response = new GetResponse<ShowLoadResponse>();

        try
        {
            int take = 10;
            int userId = await GetUserId();
            response.Model = new ShowLoadResponse();

            response.Model.ShowTypeIds = _showStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID).Select(m => new SLCodeValueSimpleModel { CodeValueId = m.CodeValueId, DecodeTxt = m.DecodeTxt });
            response.Model.Shows = _showStore.GetShows(m => m.UserId == userId);
            response.Model.Count = response.Model.Shows.Count();
            response.Model.Shows = response.Model.Shows.OrderByDescending(m => m.DateWatched).ThenByDescending(m => m.ShowId).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    //[RequiredScopeOrAppPermission(
    //    RequiredScopesConfigurationKey = "AzureAD:Scopes:User.ReadWrite",
    //    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:User.ReadWrite"
    //)]
    public async Task<IActionResult> Get(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<ShowGetResponse> response = new GetResponse<ShowGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new ShowGetResponse();
            if(!string.IsNullOrEmpty(search))
            {
                response.Model.Shows = _showStore.SearchShows(userId, search);
            }
            else
            {
                response.Model.Shows = _showStore.GetShows(m => m.UserId == userId);
            }

            response.Model.Count = response.Model.Shows.Count();
            response.Model.Shows = response.Model.Shows.OrderByDescending(m => m.DateWatched).ThenByDescending(m => m.ShowId).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Save")]
    //[RequiredScopeOrAppPermission(
    //    RequiredScopesConfigurationKey = "AzureAD:Scopes:User.ReadWrite",
    //    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:User.ReadWrite"
    //)]
    public async Task<IActionResult> SaveShow(ShowModel model)
    {
        PostResponse<ShowModel> response = new PostResponse<ShowModel>();
        
        try
        {
            int userId = await GetUserId();
            ValidationResult result = await _validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int showId = model.ShowId;

                if (showId <= 0)
                {
                    showId = _showStore.CreateShow(userId, model);
                }
                else
                {
                    _showStore.UpdateShow(userId, model);
                }

                response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == showId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("AddNextEpisode")]
    //[RequiredScopeOrAppPermission(
    //    RequiredScopesConfigurationKey = "AzureAD:Scopes:User.ReadWrite",
    //    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:User.ReadWrite"
    //)]
    public async Task<IActionResult> AddNextEpisode(ShowIdRequest request)
    {
        PostResponse<ShowModel> response = new PostResponse<ShowModel>();

        try
        {
            int userId = await GetUserId();

            int newShowId = _showStore.AddNextEpisode(userId, request.ShowId);

            if(newShowId > -1)
            {
                response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == newShowId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    //[RequiredScopeOrAppPermission(
    //    RequiredScopesConfigurationKey = "AzureAD:Scopes:User.ReadWrite",
    //    RequiredAppPermissionsConfigurationKey = "AzureAD:AppPermissions:User.ReadWrite"
    //)]
    public async Task<IActionResult> Delete(ShowIdRequest request)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            int userId = await GetUserId();

            response.Model = _showStore.DeleteShow(userId, request.ShowId);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
