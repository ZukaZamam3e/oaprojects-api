using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using OAProjects.Models.ShowLogger.Responses.Stat;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.Stat;
using OAProjects.Models.Common.Responses;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Data.ShowLogger.Entities;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/show-logger/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class StatController : BaseController
{
    private readonly ILogger<StatController> _logger;
    private readonly IStatStore _statStore;
    private readonly ICodeValueStore _codeValueStore;

    public StatController(ILogger<StatController> logger,
        IUserStore userStore,
        IStatStore statStore,
        ICodeValueStore codeValueStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _statStore = statStore;
        _codeValueStore = codeValueStore;
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

        Dictionary<string, int> showTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID)
                .ToDictionary(m => m.DecodeTxt.ToLower(), m => m.CodeValueId);

        Expression<Func<MovieStatModel, bool>>? predicate = null;
        DateTime dateSearch;

        if (!string.IsNullOrEmpty(search))
        {
            if (showTypeIds.ContainsKey(search.ToLower()))
            {
                predicate = m => m.ShowTypeId == showTypeIds[search.ToLower()]
                    && m.UserId == userId;
            }
            else
            {
                predicate = m => m.MovieName.ToLower().Contains(search.ToLower());
            }
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

            YearStatDataParameters[] parameters = response.Model.YearStats.Select(m => new YearStatDataParameters
            {
                UserId = m.UserId,
                Year = m.Year
            }).ToArray();

            IEnumerable<YearStatDataModel> data = _statStore.GetYearStatData(parameters).ToArray();

            foreach (YearStatModel year in response.Model.YearStats)
            {
                year.Data = data.Where(m => m.UserId == year.UserId && m.Year == year.Year).OrderByDescending(m => m.TotalRuntime);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<YearStatModel> GetYearStatsData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserNameLookUps();
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
            response.Model.BookYearStats = response.Model.BookYearStats.OrderByDescending(m => m.Year).ThenBy(m => m.Name).Skip(offset).Take(take).ToArray();

            YearBookStatDataParameters[] parameters = response.Model.BookYearStats.Select(m => new YearBookStatDataParameters
            {
                UserId = m.UserId,
                Year = m.Year
            }).ToArray();

            IEnumerable<BookYearStatDataModel> data = _statStore.GetBookYearStatData(parameters).ToArray();

            foreach (BookYearStatModel year in response.Model.BookYearStats)
            {
                year.Data = data.Where(m => m.UserId == year.UserId && m.Year == year.Year).OrderBy(m => m.Month);

                if (year.Data.Any())
                {
                    year.MonthAvg = year.Data.Count() / year.Data.Select(m => m.StartDate.Month).Distinct().Count();
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<BookYearStatModel> GetBookYearStatsData(int userId, string? search = null)
    {
        Dictionary<int, string> userLookUps = _userStore.GetUserNameLookUps();
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
