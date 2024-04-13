using Azure.Core;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using OAProjects.API.Requests.Show;
using OAProjects.API.Responses;
using OAProjects.API.Responses.ShowLogger.Show;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using static System.Net.Mime.MediaTypeNames;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class ShowController : BaseController
{
    private readonly ILogger<ShowController> _logger;
    private readonly IShowStore _showStore;
    private readonly IInfoStore _infoStore;
    private readonly ICodeValueStore _codeValueStore;

    public ShowController(ILogger<ShowController> logger,
        IUserStore userStore,
        IShowStore showStore,
        IInfoStore infoStore,
        ICodeValueStore codeValueStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _showStore = showStore;
        _infoStore = infoStore;
        _codeValueStore = codeValueStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<ShowLoadResponse> response = new GetResponse<ShowLoadResponse>();

        try
        {
            int take = 10;
            int userId = await GetUserId();
            response.Model = new ShowLoadResponse();

            response.Model.ShowTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID).Select(m => new SLCodeValueSimpleModel { CodeValueId = m.CodeValueId, DecodeTxt = m.DecodeTxt });
            response.Model.TransactionTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.TRANSACTION_TYPE_ID).Select(m => new SLCodeValueSimpleModel { CodeValueId = m.CodeValueId, DecodeTxt = m.DecodeTxt });
            response.Model.Shows = _showStore.GetShows(m => m.UserId == userId);
            response.Model.Count = response.Model.Shows.Count();
            response.Model.Shows = response.Model.Shows.OrderByDescending(m => m.DateWatched).ThenByDescending(m => m.ShowId).Take(take).ToArray();

            foreach (ShowModel show in response.Model.Shows)
            {
                if (show.ShowTypeId == (int)CodeValueIds.AMC)
                {
                    show.Transactions = GetShowTransactions(userId, show.ShowId);
                }
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
        GetResponse<ShowGetResponse> response = new GetResponse<ShowGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new ShowGetResponse();
            
            response.Model.Shows = GetShows(userId, search);
            response.Model.Count = response.Model.Shows.Count();
            response.Model.Shows = response.Model.Shows.OrderByDescending(m => m.DateWatched).ThenByDescending(m => m.ShowId).Skip(offset).Take(take).ToArray();

            foreach (ShowModel show in response.Model.Shows)
            {
                if(show.ShowTypeId == (int)CodeValueIds.AMC)
                {
                    show.Transactions = GetShowTransactions(userId, show.ShowId);
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<DetailedShowModel> GetShows(int userId, string? search = null)
    {
        Expression<Func<ShowInfoModel, bool>>? predicate = null;

        DateTime dateSearch;

        Dictionary<string, int> showTypeIds = _showStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.SHOW_TYPE_ID)
            .ToDictionary(m => m.DecodeTxt.ToLower(), m => m.CodeValueId);

        if (!string.IsNullOrEmpty(search))
        {
            if (DateTime.TryParse(search, out dateSearch))
            {
                predicate = m => m.DateWatched.Date == dateSearch.Date
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

        IEnumerable<DetailedShowModel> query = _showStore.GetShows(predicate);

        return query;
    }

    [HttpPost("Save")]
    public async Task<IActionResult> SaveShow(ShowModel model,
        [FromServices] IValidator<ShowModel> showValidator,
        [FromServices] IValidator<ShowTransactionModel> transactionValidator)
    {
        PostResponse<DetailedShowModel> response = new PostResponse<DetailedShowModel>();
        
        try
        {
            int userId = await GetUserId();
            ValidationResult showResult = await showValidator.ValidateAsync(model);
            List<ValidationResult> transactionResults = new List<ValidationResult>();

            if(model.Transactions != null)
            {
                foreach(ShowTransactionModel transaction in model.Transactions)
                {
                    ValidationResult transactionResult = await transactionValidator.ValidateAsync(transaction);
                    transactionResults.Add(transactionResult);
                }
            }

            bool showErrors = !showResult.IsValid;
            bool transactionErrors = transactionResults.Any(m => !m.IsValid);

            if (showErrors || transactionErrors)
            {
                List<string> errors = new List<string>();

                if(showErrors)
                {
                    errors.AddRange(showResult.Errors.Select(m => m.ErrorMessage));
                }

                if(transactionErrors)
                {
                    foreach(ValidationResult result in transactionResults) 
                    {
                        errors.AddRange(result.Errors.Select(m => m.ErrorMessage));
                    }
                }

                response.Errors = errors;
            }
            else
            {
                int showId = model.ShowId;

                if (showId <= 0)
                {
                    showId = _showStore.CreateShow(userId, model);
                }
                else
                {
                    _showStore.UpdateShow(userId, model);
                }

                response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == showId).First();

                if (response.Model.ShowTypeId == (int)CodeValueIds.AMC)
                {
                    response.Model.Transactions = GetShowTransactions(userId, response.Model.ShowId);
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(ShowIdRequest request,
        [FromServices] IValidator<ShowIdRequest> validator)
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
                response.Model = _showStore.DeleteShow(userId, request.ShowId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("AddNextEpisode")]
    public async Task<IActionResult> AddNextEpisode(ShowAddNextEpisodeRequest request,
        [FromServices] IValidator<ShowAddNextEpisodeRequest> validator)
    {
        PostResponse<DetailedShowModel> response = new PostResponse<DetailedShowModel>();

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
                int newShowId = _showStore.AddNextEpisode(userId, request.ShowId, request.DateWatched);

                if (newShowId > -1)
                {
                    response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == newShowId).First();
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("AddOneDay")]
    public async Task<IActionResult> AddOneDay(ShowIdRequest request,
        [FromServices] IValidator<ShowIdRequest> validator)
    {
        PostResponse<DetailedShowModel> response = new PostResponse<DetailedShowModel>();

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
                bool successful = _showStore.AddOneDay(userId, request.ShowId);

                if(successful)
                {
                    response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == request.ShowId).First();
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("SubtractOneDay")]
    public async Task<IActionResult> SubtractOneDay(ShowIdRequest request,
        [FromServices] IValidator<ShowIdRequest> validator)
    {
        PostResponse<DetailedShowModel> response = new PostResponse<DetailedShowModel>();

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
                bool successful = _showStore.SubtractOneDay(userId, request.ShowId);

                if (successful)
                {
                    response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == request.ShowId).First();
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("AddRange")]
    public async Task<IActionResult> AddRange(AddRangeModel model,
        [FromServices] IValidator<AddRangeModel> validator)
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
                response.Model = _showStore.AddRange(userId, model);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("AddWatchFromSearch")]
    public async Task<IActionResult> AddWatchFromSearch(AddWatchFromSearchModel model,
        [FromServices] IValidator<AddWatchFromSearchModel> showValidator,
        [FromServices] IValidator<ShowTransactionModel> transacationValidator)
    {
        PostResponse<DetailedShowModel> response = new PostResponse<DetailedShowModel>();

        try
        {
            int userId = await GetUserId();
            int? infoId = null;
            ValidationResult showResult = await showValidator.ValidateAsync(model);
            List<ValidationResult> transactionResults = new List<ValidationResult>();

            if (model.Transactions != null)
            {
                foreach (ShowTransactionModel transaction in model.Transactions)
                {
                    ValidationResult transactionResult = await transacationValidator.ValidateAsync(transaction);
                    transactionResults.Add(transactionResult);
                }
            }

            bool showErrors = !showResult.IsValid;
            bool transactionErrors = transactionResults.Any(m => !m.IsValid);

            if (showErrors || transactionErrors)
            {
                List<string> errors = new List<string>();

                if (showErrors)
                {
                    errors.AddRange(showResult.Errors.Select(m => m.ErrorMessage));
                }

                if (transactionErrors)
                {
                    foreach (ValidationResult result in transactionResults)
                    {
                        errors.AddRange(result.Errors.Select(m => m.ErrorMessage));
                    }
                }

                response.Errors = errors;
            }
            else
            {
                DownloadResultModel info = await _infoStore.Download(userId, new InfoApiDownloadModel
                {
                    API = model.API,
                    Type = model.Type,
                    Id = model.Id
                });

                if(info.Type == INFO_TYPE.TV)
                {
                    TvEpisodeInfoModel? episode = _infoStore.GetTvEpisodeInfos(m => m.TvInfoId == info.Id)
                        .FirstOrDefault(m => m.SeasonNumber == model.SeasonNumber && m.EpisodeNumber == model.EpisodeNumber);

                    if(episode != null)
                    {
                        infoId = episode.TvEpisodeInfoId;
                    }
                }
                else
                {
                    infoId = (int)info.Id;
                }

                int showId = _showStore.CreateShow(userId, new ShowModel
                {
                    ShowName = model.ShowName,
                    ShowTypeId = model.ShowTypeId,
                    DateWatched = model.DateWatched,
                    SeasonNumber = model.SeasonNumber,
                    EpisodeNumber = model.EpisodeNumber,
                    ShowNotes = model.ShowNotes,
                    RestartBinge = model.RestartBinge,
                    Transactions = model.Transactions
                }, infoId);

                if (showId > 0)
                {
                    response.Model = _showStore.GetShows(m => m.UserId == userId && m.ShowId == showId).First();

                    if (response.Model.ShowTypeId == (int)CodeValueIds.AMC)
                    {
                        response.Model.Transactions = GetShowTransactions(userId, response.Model.ShowId);
                    }
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<ShowTransactionModel> GetShowTransactions(int userId, int showId)
    {
        IEnumerable<ShowTransactionModel> query = _showStore.GetShowTransactions(userId, showId);

        return query;
    }
}
