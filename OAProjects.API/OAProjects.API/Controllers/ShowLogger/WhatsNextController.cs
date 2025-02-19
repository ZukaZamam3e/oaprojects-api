using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.WhatsNext;
using OAProjects.Models.ShowLogger.Requests.WhatsNext;
using OAProjects.Models.ShowLogger.Responses.WhatsNext;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class WhatsNextController(ILogger<WhatsNextController> logger,
    IUserStore userStore,
    IWhatsNextStore whatsNextStore,
    IInfoStore infoStore,
    ICodeValueStore codeValueStore,
    IHttpClientFactory httpClientFactory) : BaseController(logger, userStore, httpClientFactory)
{
    private readonly ILogger<WhatsNextController> _logger = logger;
    private readonly IWhatsNextStore _whatsNextStore = whatsNextStore;
    private readonly ICodeValueStore _codeValueStore = codeValueStore;
    private readonly IInfoStore _infoStore = infoStore;

    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<WhatsNextLoadResponse> response = new GetResponse<WhatsNextLoadResponse>();
        try
        {
            int userId = await GetUserId();
            response.Model = new WhatsNextLoadResponse();

            response.Model.WhatsNext = GetWhatsNext(userId);
            response.Model.Count = response.Model.WhatsNext.Count();
            response.Model.WhatsNext = response.Model.WhatsNext.OrderBy(m => m.StartDate).Take(take);
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
        GetResponse<WhatsNextGetResponse> response = new GetResponse<WhatsNextGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new WhatsNextGetResponse();
            response.Model.WhatsNext = GetWhatsNext(userId, search);
            response.Model.Count = response.Model.WhatsNext.Count();
            response.Model.WhatsNext = response.Model.WhatsNext.OrderBy(m => m.StartDate).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<WhatsNextShowModel> GetWhatsNext(int userId, string? search = null)
    {
        Expression<Func<WhatsNextShowModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.ShowName.Contains(search, StringComparison.CurrentCultureIgnoreCase);
        }

        IEnumerable<WhatsNextShowModel> query = _whatsNextStore.GetWhatsNext(userId, predicate);

        return query;
    }

    [HttpGet("LoadSubscriptions")]
    public async Task<IActionResult> LoadSubscriptions(int take = 10)
    {
        GetResponse<WhatsNextLoadSubscriptionsResponse> response = new GetResponse<WhatsNextLoadSubscriptionsResponse>();
        try
        {
            int userId = await GetUserId();
            response.Model = new WhatsNextLoadSubscriptionsResponse();

            response.Model.Subscriptions = GetWhatsNextSubscriptions(userId);
            response.Model.SubscriptionCount = response.Model.Subscriptions.Count();
            response.Model.Subscriptions = response.Model.Subscriptions.OrderBy(m => m.SubscribeDate).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("GetSubscriptions")]
    public async Task<IActionResult> GetSubscriptions(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<WhatsNextGetSubscriptionsResponse> response = new GetResponse<WhatsNextGetSubscriptionsResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new WhatsNextGetSubscriptionsResponse();
            response.Model.Subscriptions = GetWhatsNextSubscriptions(userId, search);
            response.Model.Count = response.Model.Subscriptions.Count();
            response.Model.Subscriptions = response.Model.Subscriptions.OrderBy(m => m.SubscribeDate).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<WhatsNextWatchEpisodeModel> GetWhatsNextSubscriptions(int userId, string? search = null)
    {
        Expression<Func<WhatsNextWatchEpisodeModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.ShowName.ToLower().Contains(search.ToLower())
                && m.UserId == userId;
        }
        else
        {
            predicate = m => m.UserId == userId;
        }

        IEnumerable<WhatsNextWatchEpisodeModel> query = _whatsNextStore.GetWhatsNextSubs(predicate);

        return query;
    }

    [HttpPost("CreateSubscription")]
    public async Task<IActionResult> CreateSubscription(CreateSubscriptionModel model,
        [FromServices] IValidator<CreateSubscriptionModel> validator)
    {
        PostResponse<WhatsNextWatchEpisodeModel> response = new PostResponse<WhatsNextWatchEpisodeModel>();
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

                int whatsNextSubId = _whatsNextStore.CreateWhatsNextSub(userId, new WhatsNextWatchEpisodeModel
                {
                    TvInfoId = (int)info.Id,
                    SubscribeDate = model.SubscribeDate,
                    IncludeSpecials = model.InculdeSpecials,
                });

                response.Model = _whatsNextStore.GetWhatsNextSubs(m => m.UserId == userId && m.WhatsNextSubId == whatsNextSubId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save(WhatsNextWatchEpisodeModel model,
        [FromServices] IValidator<WhatsNextWatchEpisodeModel> validator)
    {
        PostResponse<WhatsNextShowModel> response = new PostResponse<WhatsNextShowModel>();
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
                int whatsNextSubId = model.WhatsNextSubId;

                if (whatsNextSubId <= 0)
                {
                    whatsNextSubId = _whatsNextStore.CreateWhatsNextSub(userId, model);
                }
                else
                {
                    _whatsNextStore.UpdateWhatsNextSub(userId, model);
                }

                response.Model = _whatsNextStore.GetWhatsNext(userId, m => m.WhatsNextSubId == whatsNextSubId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(WhatsNextSubIdRequest request,
        [FromServices] IValidator<WhatsNextSubIdRequest> validator)
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
                response.Model = _whatsNextStore.DeleteWhatsNextSub(userId, request.WhatsNextSubId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("WatchEpisode")]
    public async Task<IActionResult> WatchEpisode(WhatsNextWatchEpisodeRequest request,
        [FromServices] IValidator<WhatsNextWatchEpisodeRequest> validator)
    {
        PostResponse<int> response = new PostResponse<int>();

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
                response.Model = _whatsNextStore.WatchEpisode(userId, request.TvEpisodeInfoId, request.DateWatched);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
