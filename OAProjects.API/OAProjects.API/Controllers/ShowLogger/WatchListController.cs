using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.WatchList;
using FluentValidation.Results;
using OAProjects.Models.ShowLogger.Requests.WatchList;
using OAProjects.Models.ShowLogger.Responses.WatchList;
using OAProjects.Models.ShowLogger.Models.Info;
using System.Linq.Expressions;
using OAProjects.Models.Common.Responses;

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
    private readonly IInfoStore _infoStore;
    public WatchListController(ILogger<WatchListController> logger,
        IUserStore userStore,
        IWatchListStore watchListStore,
        IInfoStore infoStore,
        ICodeValueStore codeValueStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _watchListStore = watchListStore;
        _codeValueStore = codeValueStore;
        _infoStore = infoStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<WatchListLoadResponse> response = new GetResponse<WatchListLoadResponse>();
        try
        {
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
            response.Model.WatchLists = GetWatchLists(userId, search);
            response.Model.Count = response.Model.WatchLists.Count();
            response.Model.WatchLists = response.Model.WatchLists.OrderByDescending(m => m.DateAdded).ThenByDescending(m => m.WatchlistId).Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<DetailedWatchListModel> GetWatchLists(int userId, string? search = null)
    {
        Expression<Func<WatchListInfoModel, bool>>? predicate = null;

        DateTime dateSearch;

        Dictionary<string, int> showTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.DecodeTxt.ToLower(), m => m.CodeValueId);

        if (!string.IsNullOrEmpty(search))
        {
            if (DateTime.TryParse(search, out dateSearch))
            {
                predicate = m => m.DateAdded.Date == dateSearch.Date
                    && m.UserId == userId;
            }
            else if (showTypeIds.ContainsKey(search.ToLower()))
            {
                predicate = m => m.ShowTypeId == showTypeIds[search.ToLower()]
                    && m.UserId == userId;
            }
            else
            {
                predicate = m => m.ShowName.ToLower().Contains(search.ToLower())
                    && m.UserId == userId;
            }
        }
        else
        {
            predicate = m => m.UserId == userId;
        }

        IEnumerable<DetailedWatchListModel> query = _watchListStore.GetWatchLists(predicate);

        return query;
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

    [HttpPost("MoveToShows")]
    public async Task<IActionResult> MoveToShows(WatchListMoveToShowsRequest request,
        [FromServices] IValidator<WatchListMoveToShowsRequest> validator)
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

                bool successful = _watchListStore.MoveToShows(userId, request.WatchListId, request.DateWatched);

                if (successful == false)
                {
                    throw new Exception("Unable to move to shows.");
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

    [HttpPost("AddWatchFromSearch")]
    public async Task<IActionResult> AddWatchFromSearch(AddWatchListFromSearchModel model,
       [FromServices] IValidator<AddWatchListFromSearchModel> validator)
    {
        PostResponse<DetailedWatchListModel> response = new PostResponse<DetailedWatchListModel>();

        try
        {
            int userId = await GetUserId();
            int? infoId = null;
            ValidationResult result = await validator.ValidateAsync(model);


            if (result.Errors.Count > 0)
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

                if (info.Type == INFO_TYPE.TV)
                {
                    TvEpisodeInfoModel? episode = _infoStore.GetTvEpisodeInfos(m => m.TvInfoId == info.Id)
                        .FirstOrDefault(m => m.SeasonNumber == model.SeasonNumber && m.EpisodeNumber == model.EpisodeNumber);

                    if (episode != null)
                    {
                        infoId = episode.TvEpisodeInfoId;
                    }
                }
                else
                {
                    infoId = (int)info.Id;
                }

                int watchListId = _watchListStore.CreateWatchList(userId, new WatchListModel
                {
                    ShowName = model.ShowName,
                    ShowTypeId = model.ShowTypeId,
                    DateAdded = model.DateAdded,
                    SeasonNumber = model.SeasonNumber,
                    EpisodeNumber = model.EpisodeNumber,
                    ShowNotes = model.ShowNotes,
                }, infoId);

                if (watchListId > 0)
                {
                    response.Model = _watchListStore.GetWatchLists(m => m.UserId == userId && m.WatchlistId == watchListId).First();
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
