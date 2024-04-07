using FluentValidation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Responses.ShowLogger.Info;
using OAProjects.API.Responses;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.Transaction;
using OAProjects.API.Responses.ShowLogger.Transaction;

namespace OAProjects.API.Controllers.ShowLogger;
[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class TransactionController : BaseController
{
    private readonly ILogger<TransactionController> _logger;
    private ITransactionStore _transactionStore;

    public TransactionController(ILogger<TransactionController> logger,
    IUserStore userStore,
    ITransactionStore transactionStore,
    IHttpClientFactory httpClientFactory)
    : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _transactionStore = transactionStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<TransactionLoadResponse> response = new GetResponse<TransactionLoadResponse>();
        try
        {
            int userId = await GetUserId();


            int take = 10;

            response.Model = new TransactionLoadResponse();

            IEnumerable<TransactionModel> data = GetTransactions(userId);

            response.Model.Count = data.Count();
            response.Model.Transactions = data.OrderByDescending(m => m.TransactionDate).Take(take).ToArray();
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
        GetResponse<TransactionGetResponse> response = new GetResponse<TransactionGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new TransactionGetResponse();

            IEnumerable<TransactionModel> data = GetTransactions(userId, search);

            response.Model.Count = data.Count();
            response.Model.Transactions = data.OrderByDescending(m => m.TransactionDate).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<TransactionModel> GetTransactions(int userId, string? search = null)
    {
        Expression<Func<TransactionModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.Item.ToLower().Contains(search.ToLower());
        }

        IEnumerable<TransactionModel> query = _transactionStore.GetTransactions(userId, predicate);

        return query;
    }

}
