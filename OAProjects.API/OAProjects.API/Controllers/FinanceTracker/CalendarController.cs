using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using OAProjects.Models.Common;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.FinanceTracker.Requests.Calendar;
using OAProjects.Models.FinanceTracker.Responses.Calendar;
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
        IMemoryCache _memoryCache,
        IFTAccountStore _ftAccountStore,
        IFTTransactionStore _ftTransactionStore,
        IFTTransactionOffsetStore _ftTransactionOffsetStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("Load")]
    public async Task<IActionResult> Load(DateTime selectedDate)
    {
        GetResponse<LoadResponse> response = new GetResponse<LoadResponse>();

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

                accounts = _ftAccountStore.GetAccounts(userId).ToList();
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

            if (calendar is not null)
            {
                DateTime beginningOfWeek = new DateTime(selectedDate.Year, selectedDate.Month, 1).StartOfWeek(DayOfWeek.Sunday);
                DateTime endDate = beginningOfWeek.AddDays(41);

                response.Model = new LoadResponse
                {
                    AccountId = accountId,
                    CanGoBack = true,
                    CanGoFurther = true,
                    Days = calendar.Days.Where(m => m.Date >= beginningOfWeek && m.Date <= endDate).OrderBy(m => m.Date),
                    Month = selectedDate.Month,
                    MonthYear = selectedDate.ToString("MMM yyyy"),
                    Accounts = accounts
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

    [HttpGet("GetFinancesOnDay")]
    public async Task<IActionResult> GetFinancesOnDay(int accountId, DateTime selectedDate)
    {
        GetResponse<DayModel> response = new GetResponse<DayModel>();

        try
        {
            int userId = await GetUserId();
            CalendarModel calendar = GetCalendarFromCache(userId, accountId);

            response.Model = calendar.Days.First(m => m.Date == selectedDate.Date);

        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("SaveTransaction")]
    public async Task<IActionResult> SaveTransaction(SaveTransactionRequest model,
        [FromServices] IValidator<TransactionModel> validator)
    {
        PostResponse<TransactionModel> response = new PostResponse<TransactionModel>();

        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(model.Transaction);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int transactionId = model.Transaction.TransactionId;

                if (transactionId <= 0)
                {
                    transactionId = _ftTransactionStore.CreateTransaction(userId, model.AccountId, model.Transaction);
                }
                else
                {
                    _ftTransactionStore.UpdateTransaction(userId, model.AccountId, model.Transaction);
                }

                TransactionOffsetModel? offset = _ftTransactionOffsetStore.GetTransactionOffsets(userId, model.Transaction.TransactionId).Where(m => m.OffsetDate == model.SelectedDate).FirstOrDefault();

                if (offset != null)
                {
                    if (model.Transaction.OffsetAmount is not null && model.Transaction.OffsetDate is not null)
                    {
                        offset.OffsetAmount = model.Transaction.OffsetAmount.Value;
                        _ftTransactionOffsetStore.UpdateTransactionOffset(userId, model.AccountId, offset);
                    }
                    else
                    {
                        _ftTransactionOffsetStore.DeleteTransactionOffset(userId, model.AccountId, offset.TransactionOffsetId);
                    }

                }
                else if (model.Transaction.OffsetDate is not null && model.Transaction.OffsetAmount is not null)
                {
                    offset = new TransactionOffsetModel
                    {
                        AccountId = model.AccountId,
                        OffsetAmount = model.Transaction.OffsetAmount.Value,
                        OffsetDate = model.Transaction.OffsetDate.Value,
                        TransactionId = transactionId,
                        UserId = userId
                    };

                    _ftTransactionOffsetStore.CreateTransactionOffset(userId, model.AccountId, offset);


                    response.Model = _ftTransactionStore.GetTransactions(userId, transactionId: transactionId).First();
                }
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

    private CalendarModel GetCalendarFromCache(int userId, int accountId)
    {
        return _memoryCache.Get($"{userId}_{accountId}") as CalendarModel;
    }
}
