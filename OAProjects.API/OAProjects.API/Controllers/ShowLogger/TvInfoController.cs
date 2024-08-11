using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Models.ShowLogger.Models.Info;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Responses.Info;
using FluentValidation;
using FluentValidation.Results;
using OAProjects.Models.Common.Responses;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class TvInfoController : BaseController
{
    private readonly ILogger<TvInfoController> _logger;
    private readonly IInfoStore _infoStore;

    public TvInfoController(ILogger<TvInfoController> logger,
        IUserStore userStore,
        IInfoStore infoStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _infoStore = infoStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<TvInfoLoadResponse> response = new GetResponse<TvInfoLoadResponse>();
        try
        {
            int userId = await GetUserId();

            response.Model = new TvInfoLoadResponse();

            IEnumerable<TvInfoModel> data = GetTvInfo();

            response.Model.Count = data.Count();
            response.Model.TvInfos = data.OrderBy(m => m.ShowName).Take(take).ToArray();

            foreach (TvInfoModel tvInfo in response.Model.TvInfos)
            {
                tvInfo.Seasons = _infoStore.GetTvInfoSeasons(tvInfo.TvInfoId);
            }
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
        GetResponse<TvInfoGetResponse> response = new GetResponse<TvInfoGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new TvInfoGetResponse();
            
            IEnumerable<TvInfoModel> data = GetTvInfo(search);

            response.Model.Count = data.Count();
            response.Model.TvInfos = data.OrderBy(m => m.ShowName).Skip(offset).Take(take).ToArray();

            foreach(TvInfoModel tvInfo in response.Model.TvInfos)
            {
                tvInfo.Seasons = _infoStore.GetTvInfoSeasons(tvInfo.TvInfoId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<TvInfoModel> GetTvInfo(string? search = null)
    {
        Expression<Func<TvInfoModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.ShowName.ToLower().Contains(search.ToLower());
        }

        IEnumerable<TvInfoModel> query = _infoStore.GetTvInfos(predicate);

        return query;
    }

    [HttpGet("GetEpisodes")]
    public async Task<IActionResult> GetEpisodes(int tvInfoId, int seasonNumber, int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<TvInfoGetEpisodesResponse> response = new GetResponse<TvInfoGetEpisodesResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new TvInfoGetEpisodesResponse();

            IEnumerable<TvEpisodeInfoModel> data = GetEpisodeInfos(tvInfoId, seasonNumber, search);

            response.Model.Count = data.Count();
            response.Model.TvEpisodeInfos = data.OrderBy(m => m.EpisodeNumber).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<TvEpisodeInfoModel> GetEpisodeInfos(int tvInfoId, int seasonNumber, string? search = null)
    {
        Expression<Func<TvEpisodeInfoModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            int episodeSearch;

            if(int.TryParse(search, out episodeSearch))
            {
                predicate = m => m.EpisodeNumber == episodeSearch 
                    && m.TvInfoId == tvInfoId
                    && m.SeasonNumber == seasonNumber;
            }
        }
        else
        {
            predicate = m => m.TvInfoId == tvInfoId
                && m.SeasonNumber == seasonNumber;
        }

        IEnumerable<TvEpisodeInfoModel> query = _infoStore.GetTvEpisodeInfos(predicate);

        return query;
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(TvInfoIdRequest request,
        [FromServices] IValidator<TvInfoIdRequest> validator)
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

                bool successful = _infoStore.DeleteTvInfo(userId, request.TvInfoId);

                if (successful == false)
                {
                    throw new Exception("Unable to delete tv info.");
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

    [HttpPost("Refresh")]
    public async Task<IActionResult> Refresh(TvInfoIdRequest request,
        [FromServices] IValidator<TvInfoIdRequest> validator)
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

                await _infoStore.RefreshInfo(userId, request.TvInfoId, INFO_TYPE.TV);

                response.Model = true;
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
