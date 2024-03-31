using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Responses;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.WatchList;
using FluentValidation.Results;
using OAProjects.API.Requests.WatchList;
using OAProjects.API.Responses.ShowLogger.WatchList;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class WatchListController : BaseController
{
    private readonly ILogger<WatchListController> _logger;
    private readonly IWatchListStore _watchListStore;
    private readonly ICodeValueStore _codeValueStore;
    public WatchListController(ILogger<WatchListController> logger,
        IUserStore userStore,
        IWatchListStore watchListStore,
        ICodeValueStore codeValueStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _watchListStore = watchListStore;
        _codeValueStore = codeValueStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<WatchListLoadResponse> response = new GetResponse<WatchListLoadResponse>();
        try
        {
            int take = 10;
            int userId = await GetUserId();
            response.Model = new WatchListLoadResponse();

            response.Model.ShowTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID).Select(m => new SLCodeValueSimpleModel { CodeValueId = m.CodeValueId, DecodeTxt = m.DecodeTxt });
            response.Model.WatchLists = _watchListStore.GetWatchLists(m => m.UserId == userId);
            response.Model.Count = response.Model.WatchLists.Count();
            response.Model.WatchLists = response.Model.WatchLists.OrderByDescending(m => m.DateAdded).ThenByDescending(m => m.WatchlistId).Take(take);
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
        GetResponse<WatchListGetResponse> response = new GetResponse<WatchListGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new WatchListGetResponse();
            if (!string.IsNullOrEmpty(search))
            {
                response.Model.WatchLists = _watchListStore.SearchWatchLists(userId, search);
            }
            else
            {
                response.Model.WatchLists = _watchListStore.GetWatchLists(m => m.UserId == userId);
            }

            response.Model.Count = response.Model.WatchLists.Count();
            response.Model.WatchLists = response.Model.WatchLists.OrderByDescending(m => m.DateAdded).ThenByDescending(m => m.WatchlistId).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save(WatchListModel model,
        [FromServices] IValidator<WatchListModel> validator)
    {
        PostResponse<WatchListModel> response = new PostResponse<WatchListModel>();

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
                int watchListId = model.WatchlistId;

                if (watchListId <= 0)
                {
                    watchListId = _watchListStore.CreateWatchList(userId, model);
                }
                else
                {
                    _watchListStore.UpdateWatchList(userId, model);
                }

                response.Model = _watchListStore.GetWatchLists(m => m.UserId == userId && m.WatchlistId == watchListId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(WatchListIdRequest request,
        [FromServices] IValidator<WatchListIdRequest> validator)
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

                bool successful = _watchListStore.DeleteWatchList(userId, request.WatchListId);

                if (successful == false)
                {
                    throw new Exception("Unable to delete watchlist.");
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
