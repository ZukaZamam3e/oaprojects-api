using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Microsoft.Extensions.Caching.Memory;
using OAProjects.Data.FinanceTracker.Entities;
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
            DateTime beginningOfWeek = new DateTime(selectedDate.Year, selectedDate.Month, 1).StartOfWeek(DayOfWeek.Sunday);
            DateTime endDate = beginningOfWeek.AddDays(41);

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

                SetUpCalendar(userId, accountId, null, endDate);

                accounts = _ftAccountStore.GetAccounts(userId).ToList();
            }
            else
            {
                AccountModel? defaultAccount = accounts.FirstOrDefault(m => m.DefaultIndc);

                accountId = defaultAccount != null ? defaultAccount.AccountId : accounts.First().AccountId;

                foreach (AccountModel account in accounts)
                {
                    SetUpCalendar(userId, account.AccountId, null, endDate);
                }
            }

            CalendarModel? calendar = GetCalendarFromCache(userId, accountId);

            if (calendar is not null)
            {
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
        [FromServices] IValidator<FTTransactionModel> validator)
    {
        PostResponse<FTTransactionModel> response = new PostResponse<FTTransactionModel>();

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

                DateTime? updateDate = null;

                if (transactionId <= 0)
                {
                    transactionId = _ftTransactionStore.CreateTransaction(userId, model.AccountId, model.Transaction);
                }
                else
                {
                    FTTransactionModel oldTransaction = _ftTransactionStore.GetTransactions(userId, transactionId: transactionId).First();

                    if (oldTransaction.Amount != model.Transaction.Amount)
                    {
                        updateDate = model.Transaction.StartDate;
                    }

                    _ftTransactionStore.UpdateTransaction(userId, model.AccountId, model.Transaction);
                }

                TransactionOffsetModel? offset = _ftTransactionOffsetStore.GetTransactionOffsets(userId, model.Transaction.TransactionId).Where(m => m.OffsetDate == model.SelectedDate).FirstOrDefault();

                if (offset != null)
                {
                    if (model.Transaction.OffsetAmount is not null && model.Transaction.OffsetDate is not null
                        && model.Transaction.OffsetAmount != offset.OffsetAmount && model.Transaction.OffsetAmount != 0)
                    {
                        offset.OffsetAmount = model.Transaction.OffsetAmount.Value;
                        _ftTransactionOffsetStore.UpdateTransactionOffset(userId, model.AccountId, offset);

                        updateDate ??= model.SelectedDate;
                    }
                    else
                    {
                        _ftTransactionOffsetStore.DeleteTransactionOffset(userId, model.AccountId, offset.TransactionOffsetId);
                    }

                }
                else if (model.Transaction.OffsetDate is not null && model.Transaction.OffsetAmount is not null && model.Transaction.OffsetAmount != 0)
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
                }

                response.Model = _ftTransactionStore.GetTransactions(userId, transactionId: transactionId).First();

                SetUpCalendar(userId, model.AccountId, updateDate);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("DeleteTransaction")]
    public async Task<IActionResult> DeleteTransaction(DeleteTransactionRequest request,
        [FromServices] IValidator<DeleteTransactionRequest> validator)
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
                FTTransactionModel oldTransaction = _ftTransactionStore.GetTransactions(userId, transactionId: request.TransactionId).First();
                response.Model = _ftTransactionStore.DeleteTransaction(userId, request.AccountId, request.TransactionId);
                SetUpCalendar(userId, request.AccountId, oldTransaction.StartDate);

            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("SaveHardset")]
    public async Task<IActionResult> SaveHardset(SaveHardsetRequest request,
        [FromServices] IValidator<SaveHardsetRequest> validator)
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
                int transactionId = 0;
                FTTransactionModel? hardset = _ftTransactionStore.GetTransactions(userId, accountId: request.AccountId).FirstOrDefault(m => m.FrequencyTypeId == (int)FT_CodeValueIds.HARDSET && m.StartDate == request.Date.Date);

                if (hardset is null)
                {
                    if (request.Amount != 0)
                    {
                        transactionId = _ftTransactionStore.CreateTransaction(userId, request.AccountId, new FTTransactionModel
                        {
                            Name = "Hardset",
                            StartDate = request.Date.Date,
                            AccountId = request.AccountId,
                            FrequencyTypeId = (int)FT_CodeValueIds.HARDSET,
                            Amount = request.Amount,
                        });
                    }
                }
                else
                {
                    if (request.Amount == 0)
                    {
                        _ftTransactionStore.DeleteTransaction(userId, request.AccountId, hardset.TransactionId);
                        transactionId = 1;
                    }
                    else
                    {
                        hardset.Amount = request.Amount;

                        _ftTransactionStore.UpdateTransaction(userId, request.AccountId, hardset);
                    }
                }

                response.Model = transactionId > 0;
                SetUpCalendar(userId, request.AccountId, request.Date);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private void SetUpCalendar(int userId, int accountId, DateTime? startDate = null, DateTime? endDate = null)
    {
        CalendarModel calendar = GetCalendarFromCache(userId, accountId);

        IEnumerable<FTTransactionModel> transactions = _ftTransactionStore.GetTransactions(userId, accountId: accountId).ToList() ?? [];
        IEnumerable<TransactionOffsetModel> offsets = _ftTransactionOffsetStore.GetTransactionOffsets(userId, accountId: accountId).ToList() ?? [];

        DateTime minDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).StartOfWeek(DayOfWeek.Sunday);

        if (transactions.Any())
        {
            minDate = transactions.Min(m => m.StartDate);
        }

        if (calendar is null)
        {
            calendar = new CalendarModel
            (
                userId,
                accountId,
                minDate,
                transactions.ToList(),
                offsets.ToList()
            );
        }
        else
        {
            calendar.Transactions = transactions.ToList();
            calendar.Offsets = offsets.ToList();
        }

        if (startDate == null)
        {
            startDate = calendar.StartDate;
        }

        if (endDate == null)
        {
            endDate = calendar.EndDate;
        }

        calendar.Calculate(startDate.Value, endDate.Value);

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
