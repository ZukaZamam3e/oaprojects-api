using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.Transaction;
using OAProjects.Models.ShowLogger.Responses.Transaction;
using FluentValidation.Results;
using OAProjects.Models.ShowLogger.Requests.Transaction;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.Common.Responses;

namespace OAProjects.API.Controllers.ShowLogger;
[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class TransactionController : BaseController
{
    private readonly ILogger<TransactionController> _logger;
    private ITransactionStore _transactionStore;
    private readonly ICodeValueStore _codeValueStore;

    public TransactionController(ILogger<TransactionController> logger,
        IUserStore userStore,
        ITransactionStore transactionStore,
        ICodeValueStore codeValueStore,
        IHttpClientFactory httpClientFactory)
    : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _transactionStore = transactionStore;
        _codeValueStore = codeValueStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<TransactionLoadResponse> response = new GetResponse<TransactionLoadResponse>();
        try
        {
            int userId = await GetUserId();

            response.Model = new TransactionLoadResponse();

            IEnumerable<TransactionModel> data = GetTransactions(false, userId);
            IEnumerable<TransactionModel> movieData = GetTransactions(true, userId);

            int[] ignore = new int[] 
            {                
                (int)CodeValueIds.BENEFITS,
                (int)CodeValueIds.REWARDS,
                (int)CodeValueIds.TAX 
            };

            response.Model.TransactionTypeIds = _codeValueStore.GetCodeValues(m => m.CodeTableId == (int)CodeTableIds.TRANSACTION_TYPE_ID && !ignore.Contains(m.CodeValueId))
                .Select(m => new SLCodeValueSimpleModel { CodeValueId = m.CodeValueId, DecodeTxt = m.DecodeTxt });
            
            response.Model.Count = data.Count();
            response.Model.Transactions = data.OrderByDescending(m => m.TransactionDate).Take(take).ToArray();
            response.Model.TransactionItems = _transactionStore.GetTransactionItems(userId);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(bool movieTransactions, int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<TransactionGetResponse> response = new GetResponse<TransactionGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new TransactionGetResponse();

            IEnumerable<TransactionModel> data = GetTransactions(movieTransactions, userId, search);

            response.Model.Count = data.Count();
            response.Model.Transactions = data.OrderByDescending(m => m.TransactionDate).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<TransactionModel> GetTransactions(bool movieTransactions, int userId, string? search = null)
    {
        Expression<Func<TransactionModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.Item.ToLower().Contains(search.ToLower())
                && ((movieTransactions && m.ShowId != null)
                || (!movieTransactions && m.ShowId == null));
        }
        else
        {
            predicate = m => (movieTransactions && m.ShowId != null)
                || (!movieTransactions && m.ShowId == null);
        }

        IEnumerable<TransactionModel> query = _transactionStore.GetTransactions(userId, predicate);

        return query;
    }

    [HttpPost("Save")]
    public async Task<IActionResult> Save(TransactionModel model,
        [FromServices] IValidator<TransactionModel> validator)
    {
        PostResponse<TransactionModel> response = new PostResponse<TransactionModel>();

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
                int transacitonId = model.TransactionId;

                if (transacitonId <= 0)
                {
                    transacitonId = _transactionStore.CreateTransaction(userId, model);
                }
                else
                {
                    _transactionStore.UpdateTransaction(userId, model);
                }

                response.Model = _transactionStore.GetTransactions(userId, m => m.TransactionId == transacitonId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(TransactionIdRequest request,
        [FromServices] IValidator<TransactionIdRequest> validator)
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

                bool successful = _transactionStore.DeleteTransaction(userId, request.TransactionId);

                if (successful == false)
                {
                    throw new Exception("Unable to delete transaction.");
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

}
