using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.FinanceTracker.Responses.Account;
using OAProjects.Models.FinanceTracker.Responses.Transaction;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using System.Linq.Expressions;
using TMDbLib.Objects.Search;

namespace OAProjects.API.Controllers.FinanceTracker;

[ApiController]
[Route("api/finance-tracker/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class FTTransactionController(
    ILogger<FTTransactionController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory,
        IFTTransactionStore _ftTransactionStore,
        IFTTransactionOffsetStore _ftTransactionOffsetStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("Load")]
    public async Task<IActionResult> Load(int accountId, int take = 10)
    {
        GetResponse<FTTransactionLoadResponse> response = new();

        try
        {
            int userId = await GetUserId();

            IEnumerable<FTTransactionModel> transactions = GetTransactions(userId, accountId);

            response.Model = new FTTransactionLoadResponse
            {
                Count = transactions.Count()
            };

            int[] sortLast =
            {
                (int)FT_CodeValueIds.HARDSET,
                (int)FT_CodeValueIds.SINGLE
            };

            transactions = transactions.OrderBy(m => sortLast.Contains(m.FrequencyTypeId)).ThenBy(m => m.StartDate.Date).Take(take);
            response.Model.Transactions = LoadOffsets(userId, accountId, transactions);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(int accountId, int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<FTTransactionGetResponse> response = new();

        try
        {
            int userId = await GetUserId();

            IEnumerable<FTTransactionModel> transactions = GetTransactions(userId, accountId, search);

            response.Model = new FTTransactionGetResponse
            {
                Count = transactions.Count()
            };

            int[] sortLast =
            {
                (int)FT_CodeValueIds.HARDSET,
                (int)FT_CodeValueIds.SINGLE
            };

            transactions = transactions.OrderBy(m => sortLast.Contains(m.FrequencyTypeId)).ThenBy(m => m.StartDate.Date).Skip(offset).Take(take);
            response.Model.Transactions = LoadOffsets(userId, accountId, transactions);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<FTTransactionViewModel> LoadOffsets(int userId, int accountId, IEnumerable<FTTransactionModel> transactions)
    {
        IEnumerable<TransactionOffsetModel> offsets = _ftTransactionOffsetStore.GetTransactionOffsets(userId, accountId, transactions.Select(m => m.TransactionId).ToArray());

        return transactions.Select(m => new FTTransactionViewModel
        {
            Transaction = m,
            Offsets = offsets.Where(m => m.TransactionId == m.TransactionId)
        });
    }

    private IEnumerable<FTTransactionModel> GetTransactions(int userId, int accountId, string? search = null)
    {
        IEnumerable<FTTransactionModel> query = _ftTransactionStore.GetTransactions(userId, accountId: accountId);

        if (!string.IsNullOrEmpty(search))
        {
            query = query.Where(m => m.Name.ToLower().Contains(search.ToLower())).AsEnumerable();
        }

        return query;
    }
}
