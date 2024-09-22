using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Models.ShowLogger.Responses.WatchList;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Store.ShowLogger.Stores;
using Azure.Core;
using FluentValidation.Results;
using OAProjects.Models.ShowLogger.Requests.Batch;
using OAProjects.Models.ShowLogger.Models.Batch;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.ShowLogger.Responses.Batch;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("Batch.ReadWrite")]
public class BatchController : BaseController
{
    private readonly ILogger<BatchController> _logger;
    private readonly IInfoStore _infoStore;

    public BatchController(ILogger<BatchController> logger,
        IUserStore userStore,
        IInfoStore infoStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _infoStore = infoStore;
    }

    [HttpGet("GetReturningSeries")]
    public async Task<IActionResult> GetReturningSeries()
    {
        GetResponse<ReturningSeriesResponse> response = new GetResponse<ReturningSeriesResponse>();

        try
        {
            int userId = await GetUserId();

            string[] statuses = ["Returning Series", "In Production"];

            response.Model = new ReturningSeriesResponse
            {
                ReturningSeries = _infoStore.GetTvInfos(m => statuses.Contains(m.Status)).Select(m => new ReturningSeriesModel
                {
                    LastRefreshDate = m.LastDataRefresh,
                    SeriesName = m.ShowName,
                    TvInfoId = m.TvInfoId
                })
            };
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("RefreshTvSeries")]
    public async Task<IActionResult> RefreshTvSeries(RefreshTvSeriesRequest request)
    {
        PostResponse<RefreshTvSeriesResponse> response = new PostResponse<RefreshTvSeriesResponse>();

        try
        {
            int userId = await GetUserId();

            TvInfoModel? info = _infoStore.GetTvInfos(m => m.TvInfoId == request.TvInfoId).FirstOrDefault();

            if(info != null)
            {
                DownloadResultModel result = await _infoStore.RefreshInfo(userId, info.TvInfoId, INFO_TYPE.TV);

                response.Model = new RefreshTvSeriesResponse
                {
                    Successful = true,
                    UpdatedEpisodeCount = result.UpdatedEpisodeCount
                };
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
