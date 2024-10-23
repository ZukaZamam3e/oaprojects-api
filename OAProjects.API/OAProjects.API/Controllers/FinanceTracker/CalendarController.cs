using Azure;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Controllers.ShowLogger;
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
            AccountManager accountManager = new AccountManager();



            //accountManager.Calculate(1000, 1000, new DateTime(2024, 10, 01), new DateTime(2024, 10, 01))
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
