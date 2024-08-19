using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.ShowLogger.Responses.Info;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using FluentValidation;
using FluentValidation.Results;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using OAProjects.Models.Common.Responses;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("Info.ReadWrite")]
public class UnlinkedShowsController : BaseController
{
    private readonly ILogger<UnlinkedShowsController> _logger;
    private readonly IUnlinkedShowStore _unlinkedShowStore;

    public UnlinkedShowsController(ILogger<UnlinkedShowsController> logger,
        IUserStore userStore,
        IUnlinkedShowStore unlinkedShowStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _unlinkedShowStore = unlinkedShowStore;
    }

    [HttpGet("Load")]
    public IActionResult Load(int take = 10)
    {
        GetResponse<UnlinkedShowsLoadResponse> response = new GetResponse<UnlinkedShowsLoadResponse>();
        try
        {
            response.Model = new UnlinkedShowsLoadResponse();

            IEnumerable<UnlinkedShowModel> data = GetUnlinkedShows();

            response.Model.Count = data.Count();
            response.Model.UnlinkedShows = data.OrderBy(m => m.ShowName).Take(take).ToArray();
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
        GetResponse<UnlinkedShowsGetResponse> response = new GetResponse<UnlinkedShowsGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new UnlinkedShowsGetResponse();

            IEnumerable<UnlinkedShowModel> data = GetUnlinkedShows(search);

            response.Model.Count = data.Count();
            response.Model.UnlinkedShows = data.OrderBy(m => m.ShowName).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<UnlinkedShowModel> GetUnlinkedShows(string? search = null)
    {
        Expression<Func<UnlinkedShowModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.ShowName.ToLower().Contains(search.ToLower());
        }

        IEnumerable<UnlinkedShowModel> query = _unlinkedShowStore.GetUnlinkedShows(predicate);

        return query;
    }

    [HttpPost("UpdateShowNames")]
    public async Task<IActionResult> UpdateShowNames(UpdateUnlinkedNameModel model,
        [FromServices] IValidator<UpdateUnlinkedNameModel> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

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
                response.Model = _unlinkedShowStore.UpdateShowNames(model);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("LinkShows")]
    public async Task<IActionResult> LinkShows(LinkShowModel model,
        [FromServices] IValidator<LinkShowModel> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

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
                response.Model = _unlinkedShowStore.LinkShows(model);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
