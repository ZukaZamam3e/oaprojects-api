using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Controllers.ShowLogger;
using OAProjects.Models.Common;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.FinanceTracker.Responses.Calendar;
using OAProjects.Models.ShowLogger.Responses.Show;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using OAProjects.Store.OAIdentity.Stores.Interfaces;

namespace OAProjects.API.Controllers.FinanceTracker;

[ApiController]
[Route("api/finance-tracker/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class CalendarController(
        ILogger<CalendarController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory,
        IFTTransactionStore _ftTransactionStore,
        IFTTransactionOffsetStore _ftTransactionOffsetStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("GetFinances")]
    public async Task<IActionResult> GetFinances(int accountId, DateTime selectedDate)
    {
        GetResponse<GetFinancesResponse> response = new GetResponse<GetFinancesResponse>();

        try
        {
            int userId = await GetUserId();
            AccountManager accountManager = new AccountManager();

            IEnumerable<TransactionModel> transactions = _ftTransactionStore.GetTransactions(accountId: accountId);
            IEnumerable<TransactionOffsetModel> offsets = _ftTransactionOffsetStore.GetTransactionOffsets(accountId: accountId);
            DateTime beginningOfWeek = new DateTime(selectedDate.Year, selectedDate.Month, 1).StartOfWeek(DayOfWeek.Sunday);
            DateTime endDate = beginningOfWeek.AddDays(41);

            accountManager.Calculate(userId, accountId, beginningOfWeek, endDate, transactions, offsets);

            //accountManager.Calculate(1000, 1000, new DateTime(2024, 10, 01), new DateTime(2024, 10, 01))
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
