using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Watched;
using OAProjects.Models.ShowLogger.Models.WhatsNext;
using OAProjects.Models.ShowLogger.Requests.Watched;
using OAProjects.Models.ShowLogger.Requests.WhatsNext;
using OAProjects.Models.ShowLogger.Responses.Generic;
using OAProjects.Models.ShowLogger.Responses.WhatsNext;
using OAProjects.Store.OAIdentity.Stores;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class WatchedController(
    ILogger<WatchedController> logger,
    IUserStore userStore,
    IWatchedStore watchedStore,
    IInfoStore infoStore,
    ICodeValueStore codeValueStore,
    IHttpClientFactory httpClientFactory)
     : BaseController(logger, userStore, httpClientFactory)
{
    private readonly ILogger<WatchedController> _logger = logger;
    private readonly IWatchedStore _watchedStore = watchedStore;
    private readonly IInfoStore _infoStore = infoStore;

    [HttpGet("LoadTV")]
    public async Task<IActionResult> LoadTV(int take = 10)
    {
        GetResponse<ItemResponse<WatchedModel>> response = new GetResponse<ItemResponse<WatchedModel>>();
        try
        {
            int userId = await GetUserId();
            response.Model = new ItemResponse<WatchedModel>();

            response.Model.Items = GetWatchedTV(userId);
            response.Model.Count = response.Model.Items.Count();
            response.Model.Items = response.Model.Items.OrderByDescending(m => m.DateWatched != null).ThenByDescending(m => m.DateWatched).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("GetTV")]
    public async Task<IActionResult> GetTV(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<ItemResponse<WatchedModel>> response = new GetResponse<ItemResponse<WatchedModel>>();

        try
        {
            int userId = await GetUserId();

            response.Model = new ItemResponse<WatchedModel>();
            response.Model.Items = GetWatchedTV(userId, search);
            response.Model.Count = response.Model.Items.Count();
            response.Model.Items = response.Model.Items.OrderByDescending(m => m.DateWatched != null).ThenByDescending(m => m.DateWatched).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<WatchedModel> GetWatchedTV(int userId, string? search = null)
    {
        Expression<Func<WatchedModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        IEnumerable<WatchedModel> query = _watchedStore.GetWatchedTV(userId, predicate);

        return query;
    }

    [HttpGet("LoadMovies")]
    public async Task<IActionResult> LoadMovies(int take = 10)
    {
        GetResponse<ItemResponse<WatchedModel>> response = new GetResponse<ItemResponse<WatchedModel>>();
        try
        {
            int userId = await GetUserId();
            response.Model = new ItemResponse<WatchedModel>();

            response.Model.Items = GetWatchedMovies(userId);
            response.Model.Count = response.Model.Items.Count();
            response.Model.Items = response.Model.Items.OrderByDescending(m => m.DateWatched != null).ThenByDescending(m => m.DateWatched).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("GetMovies")]
    public async Task<IActionResult> GetMovies(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<ItemResponse<WatchedModel>> response = new GetResponse<ItemResponse<WatchedModel>>();

        try
        {
            int userId = await GetUserId();

            response.Model = new ItemResponse<WatchedModel>();
            response.Model.Items = GetWatchedMovies(userId, search);
            response.Model.Count = response.Model.Items.Count();
            response.Model.Items = response.Model.Items.OrderByDescending(m => m.DateWatched != null).ThenByDescending(m => m.DateWatched).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<WatchedModel> GetWatchedMovies(int userId, string? search = null)
    {
        Expression<Func<WatchedModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.Name.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        IEnumerable<WatchedModel> query = _watchedStore.GetWatchedMovies(userId, predicate);

        return query;
    }

    [HttpPost("CreateWatched")]
    public async Task<IActionResult> CreateWatched(CreateWatchedModel model,
        [FromServices] IValidator<CreateWatchedModel> validator)
    {
        PostResponse<WatchedModel> response = new PostResponse<WatchedModel>();
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
                DownloadResultModel info = await _infoStore.Download(userId, new InfoApiDownloadModel
                {
                    API = model.API,
                    Type = model.Type,
                    Id = model.Id
                });

                int watchedId = _watchedStore.CreateWatched(userId, new WatchedModel
                {
                    InfoId = (int)info.Id,
                    InfoType = (int)info.Type,
                    DateWatched = model.DateWatched,
                });

                response.Model = _watchedStore.GetWatched(m => m.UserId == userId && m.WatchedId == watchedId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save(WatchedModel model,
        [FromServices] IValidator<WatchedModel> validator)
    {
        PostResponse<WatchedModel> response = new PostResponse<WatchedModel>();
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
                int watchedId = model.WatchedId;

                if (watchedId <= 0)
                {
                    watchedId = _watchedStore.CreateWatched(userId, model);
                }
                else
                {
                    _watchedStore.UpdateWatched(userId, model);
                }

                response.Model = _watchedStore.GetWatched(m => m.UserId == userId && m.WatchedId == watchedId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(WatchedIdRequest request,
        [FromServices] IValidator<WatchedIdRequest> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                response.Model = _watchedStore.DeleteWatched(userId, request.WatchedId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
