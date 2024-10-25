using Azure;
using FluentValidation.Validators;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Identity.Client;
using OAProjects.API.Controllers.ShowLogger;
using OAProjects.Models.Common;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.FinanceTracker.Responses.Calendar;
using OAProjects.Models.ShowLogger.Responses.Show;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using System.Globalization;

namespace OAProjects.API.Controllers.FinanceTracker;

[ApiController]
[Route("api/finance-tracker/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class CalendarController(
        ILogger<CalendarController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory,
        IMemoryCache _memoryCache,
        IFTAccountStore _ftAccountStore,
        IFTTransactionStore _ftTransactionStore,
        IFTTransactionOffsetStore _ftTransactionOffsetStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("Load")]
    public async Task<IActionResult> Load(DateTime selectedDate)
    {
        GetResponse<GetFinancesResponse> response = new GetResponse<GetFinancesResponse>();

        try
        {
            int userId = await GetUserId();
            int accountId;
            IEnumerable<AccountModel> accounts = _ftAccountStore.GetAccounts(userId).ToList();
            if (!accounts.Any())
            {
                accountId = _ftAccountStore.CreateAccount(userId, new AccountModel
                {
                    AccountName = "Account #1",
                    DefaultIndc = true,
                });

                SetUpCalendar(userId, accountId);

                
            }
            else
            {
                AccountModel? defaultAccount = accounts.FirstOrDefault(m => m.DefaultIndc);

                accountId = defaultAccount != null ? defaultAccount.AccountId : accounts.First().AccountId;

                foreach (AccountModel account in accounts)
                {
                    SetUpCalendar(userId, account.AccountId);
                }
            }

            CalendarModel? calendar = GetCalendarFromCache(userId, accountId);

            if(calendar is not null)
            {
                DateTime beginningOfWeek = new DateTime(selectedDate.Year, selectedDate.Month, 1).StartOfWeek(DayOfWeek.Sunday);
                DateTime endDate = beginningOfWeek.AddDays(41);

                response.Model = new GetFinancesResponse
                {
                    AccountId = accountId,
                    CanGoBack = true,
                    CanGoFurther = true,
                    Days = calendar.Days.Where(m => m.Date >= beginningOfWeek && m.Date <= endDate).OrderBy(m => m.Date),
                    Month = selectedDate.Month,
                    MonthYear = selectedDate.ToString("MMM yyyy"),
                };

                IEnumerable<DayModel> monthDays = response.Model.Days.Where(m => m.Date.Month == selectedDate.Month).AsEnumerable();

                response.Model.LowestDay = monthDays.OrderBy(m => m.Total).First();
                response.Model.MonthIncome = monthDays.Sum(m => m.Income);
                response.Model.MonthExpenses = monthDays.Sum(m => m.Expenses);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("GetFinances")]
    public async Task<IActionResult> GetFinances(int accountId, DateTime selectedDate)
    {
        GetResponse<GetFinancesResponse> response = new GetResponse<GetFinancesResponse>();

        try
        {
            int userId = await GetUserId();
            CalendarModel? calendar = GetCalendarFromCache(userId, accountId);

            if (calendar is not null)
            {
                DateTime beginningOfWeek = new DateTime(selectedDate.Year, selectedDate.Month, 1).StartOfWeek(DayOfWeek.Sunday);
                DateTime endDate = beginningOfWeek.AddDays(41);
                bool updated = calendar.Calculate(beginningOfWeek, endDate);

                if (updated)
                {
                    CacheCalendar(calendar);
                }

                response.Model = new GetFinancesResponse
                {
                    AccountId = accountId,
                    CanGoBack = true, 
                    CanGoFurther = true, 
                    Days = calendar.Days.Where(m => m.Date >= beginningOfWeek && m.Date <= endDate).OrderBy(m => m.Date),
                    Month = selectedDate.Month,
                    MonthYear = selectedDate.ToString("MMM yyyy"),
                };

                IEnumerable<DayModel> monthDays = response.Model.Days.Where(m => m.Date.Month == selectedDate.Month).AsEnumerable();

                response.Model.LowestDay = monthDays.OrderBy(m => m.Total).First();
                response.Model.MonthIncome = monthDays.Sum(m => m.Income);
                response.Model.MonthExpenses = monthDays.Sum(m => m.Expenses);


            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private void SetUpCalendar(int userId, int accountId)
    {
        IEnumerable<TransactionModel> transactions = _ftTransactionStore.GetTransactions(userId, accountId: accountId).ToList();
        IEnumerable<TransactionOffsetModel> offsets = _ftTransactionOffsetStore.GetTransactionOffsets(userId, accountId: accountId).ToList();

        CalendarModel calendar = new CalendarModel
        (
            userId,
            accountId,
            transactions.Min(m => m.StartDate),
            transactions.ToList(),
            offsets.ToList()
        );

        calendar.Calculate(calendar.StartDate, DateTime.Now.AddMonths(2));

        CacheCalendar(calendar);
    }

    private void CacheCalendar(CalendarModel calendar)
    {
        _memoryCache.Set(calendar.CalendarId, calendar);
    }

    private CalendarModel? GetCalendarFromCache(int userId, int accountId)
    {
        return _memoryCache.Get($"{userId}_{accountId}") as CalendarModel;
    }
}
