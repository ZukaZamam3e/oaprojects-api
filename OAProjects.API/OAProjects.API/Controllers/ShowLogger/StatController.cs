using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Responses.ShowLogger.Show;
using OAProjects.API.Responses;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.API.Responses.ShowLogger.Stat;
using OAProjects.Store.ShowLogger.Stores;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class StatController : BaseController
{
    private readonly ILogger<StatController> _logger;
    private readonly IStatStore _statStore;
    public StatController(ILogger<StatController> logger,
        IUserStore userStore,
        IStatStore statStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _statStore = statStore;
    }

    [HttpGet("GetTvStats")]
    public async Task<IActionResult> GetTvStats(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<StatTvStatResponse> response = new GetResponse<StatTvStatResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new StatTvStatResponse();

            response.Model.TvStats = GetTvStatsData(userId, search);
            response.Model.Count = response.Model.TvStats.Count();
            response.Model.TvStats = response.Model.TvStats.OrderByDescending(m => m.ShowId).ThenByDescending(m => m.LastWatched).ThenByDescending(m => m.ShowName).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<TvStatModel> GetTvStatsData(int userId, string? search = null)
    {
        IEnumerable<TvStatModel> query = _statStore.GetTVStats(userId);

        Expression<Func<TvStatModel, bool>>? predicate = null;
        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.ShowName.ToLower().Contains(search.ToLower());
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }


        return query;
    }

    [HttpGet("GetMovieStats")]
    public async Task<IActionResult> GetMovieStats(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<StatMovieStatResponse> response = new GetResponse<StatMovieStatResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new StatMovieStatResponse();

            response.Model.MovieStats = GetMovieStatsData(userId, search);
            response.Model.Count = response.Model.MovieStats.Count();
            response.Model.MovieStats = response.Model.MovieStats.OrderByDescending(m => m.DateWatched).ThenByDescending(m => m.MovieName).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<MovieStatModel> GetMovieStatsData(int userId, string? search = null)
    {
        IEnumerable<MovieStatModel> query = _statStore.GetMovieStats(userId);

        Expression<Func<MovieStatModel, bool>>? predicate = null;
        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.MovieName.ToLower().Contains(search.ToLower());
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }


        return query;
    }

    [HttpGet("GetYearStats")]
    public async Task<IActionResult> GetYearStats(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<StatYearStatResponse> response = new GetResponse<StatYearStatResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new StatYearStatResponse();

            response.Model.YearStats = GetYearStatsData(userId, search);
            response.Model.Count = response.Model.YearStats.Count();
            response.Model.YearStats = response.Model.YearStats.OrderByDescending(m => m.Year).ThenBy(m => m.UserId == userId ? 0 : 1).ThenBy(m => m.Name).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<YearStatModel> GetYearStatsData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserLookUps();
        IEnumerable<YearStatModel> query = _statStore.GetYearStats(userId, userLookUps);

        Expression<Func<YearStatModel, bool>>? predicate = null;
        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.Name.ToLower().Contains(search.ToLower());
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }


        return query;
    }

    [HttpGet("GetBookYearStats")]
    public async Task<IActionResult> GetBookYearStats(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<StatBookYearStatResponse> response = new GetResponse<StatBookYearStatResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new StatBookYearStatResponse();

            response.Model.BookYearStats = GetBookYearStatsData(userId, search);
            response.Model.Count = response.Model.BookYearStats.Count();
            response.Model.BookYearStats = response.Model.BookYearStats.OrderByDescending(m => m.Year).ThenByDescending(m => m.Name).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<BookYearStatModel> GetBookYearStatsData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserLookUps();
        IEnumerable<BookYearStatModel> query = _statStore.GetBookYearStats(userId, userLookUps);

        Expression<Func<BookYearStatModel, bool>>? predicate = null;
        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.Name.ToLower().Contains(search.ToLower());
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).AsEnumerable();
        }


        return query;
    }
}
