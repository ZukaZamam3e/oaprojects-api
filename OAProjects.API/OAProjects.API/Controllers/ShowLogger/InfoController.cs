using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.ShowLogger.Responses.Info;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using FluentValidation.Results;
using OAProjects.Models.Common.Responses;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class InfoController : BaseController
{
    private readonly ILogger<InfoController> _logger;
    private readonly IInfoStore _infoStore;

    public InfoController(ILogger<InfoController> logger,
        IUserStore userStore,
        IInfoStore infoStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _infoStore = infoStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<InfoLoadResponse> response = new GetResponse<InfoLoadResponse>();
        try
        {
            int userId = await GetUserId();
            response.Model = new InfoLoadResponse();

            response.Model.InfoApiIds = new SLCodeValueSimpleModel[]
            {
                new SLCodeValueSimpleModel
                {
                    CodeValueId = 0,
                    DecodeTxt = "TMDB"
                }
            };

            response.Model.InfoTypeIds = new SLCodeValueSimpleModel[]
            {
                new SLCodeValueSimpleModel
                {
                    CodeValueId = 0,
                    DecodeTxt = "TV"
                },
                new SLCodeValueSimpleModel
                {
                    CodeValueId = 1,
                    DecodeTxt = "Movie"
                }
            };
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("SearchApi")]
    public async Task<IActionResult> SearchApi(InfoApiSearchModel model, [FromServices] IValidator<InfoApiSearchModel> validator)
    {
        PostResponse<InfoSearchApiResponse> response = new PostResponse<InfoSearchApiResponse>();

        try
        {
            ValidationResult result = await validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int userId = await GetUserId();

                ApiResultModel<IEnumerable<ApiSearchResultModel>> query = await _infoStore.Search(userId, model);

                response.Model = new InfoSearchApiResponse();

                response.Model.SearchResults = query.ApiResultContents.OrderByDescending(m => m.AirDate);
                response.Model.Count = query.ApiResultContents.Count();
                response.Model.ResultMessage = query.Result;
            }

        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("DownloadInfo")]
    public async Task<IActionResult> DownloadInfo(InfoApiDownloadModel model, [FromServices] IValidator<InfoApiDownloadModel> validator)
    {
        PostResponse<DownloadInfoResponse> response = new PostResponse<DownloadInfoResponse>();

        try
        {
            ValidationResult result = await validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int userId = await GetUserId();

                DownloadResultModel downloadResult = await _infoStore.Download(userId, model);

                response.Model = new DownloadInfoResponse
                {
                    Result = downloadResult.Result,
                    ShowName = downloadResult.ShowName,
                    IsSuccessful = downloadResult.IsSuccessful,
                    Id = downloadResult.Id
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
