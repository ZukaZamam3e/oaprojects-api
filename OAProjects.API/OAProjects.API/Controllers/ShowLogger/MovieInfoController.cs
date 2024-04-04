using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Responses.ShowLogger.Info;
using OAProjects.API.Responses;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using TMDbLib.Objects.Search;
using FluentValidation;
using OAProjects.API.Requests.WatchList;
using OAProjects.Store.ShowLogger.Stores;
using FluentValidation.Results;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class MovieInfoController : BaseController
{
    private readonly ILogger<MovieInfoController> _logger;
    private readonly IInfoStore _infoStore;

    public MovieInfoController(ILogger<MovieInfoController> logger,
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
        GetResponse<MovieInfoLoadResponse> response = new GetResponse<MovieInfoLoadResponse>();
        try
        {
            int take = 10;

            int userId = await GetUserId();

            response.Model = new MovieInfoLoadResponse();

            IEnumerable<MovieInfoModel> data = GetMovieInfo();

            response.Model.Count = data.Count();
            response.Model.MovieInfos = data.OrderBy(m => m.MovieName).Take(take).ToArray();
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
        GetResponse<MovieInfoGetResponse> response = new GetResponse<MovieInfoGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new MovieInfoGetResponse();

            IEnumerable<MovieInfoModel> data = GetMovieInfo(search);

            response.Model.Count = data.Count();
            response.Model.MovieInfos = data.OrderBy(m => m.MovieName).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<MovieInfoModel> GetMovieInfo(string? search = null)
    {
        Expression<Func<MovieInfoModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.MovieName.ToLower().Contains(search.ToLower());
        }

        IEnumerable<MovieInfoModel> query = _infoStore.GetMovieInfos(predicate);

        return query;
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(MovieInfoIdRequest request,
        [FromServices] IValidator<MovieInfoIdRequest> validator)
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

                bool successful = _infoStore.DeleteMovieInfo(userId, request.MovieInfoId);

                if (successful == false)
                {
                    throw new Exception("Unable to delete movie info.");
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
    public async Task<IActionResult> Refresh(MovieInfoIdRequest request,
        [FromServices] IValidator<MovieInfoIdRequest> validator)
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

                await _infoStore.RefreshInfo(userId, request.MovieInfoId, INFO_TYPE.MOVIE);

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
