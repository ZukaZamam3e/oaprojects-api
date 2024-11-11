using FluentValidation;
using FluentValidation.Results;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.FinanceTracker.Requests.Account;
using OAProjects.Models.FinanceTracker.Responses.Account;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.API.Controllers.FinanceTracker;

[ApiController]
[Route("api/finance-tracker/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class AccountController(
        ILogger<AccountController> logger,
        IUserStore userStore,
        IHttpClientFactory httpClientFactory,
        IFTAccountStore _ftAccountStore
    ) : BaseController(logger, userStore, httpClientFactory)
{
    [HttpGet("Load")]
    public async Task<IActionResult> Load(int take = 10)
    {
        GetResponse<AccountLoadResponse> response = new();

        try
        {
            int userId = await GetUserId();

            response.Model = new AccountLoadResponse
            {
                Accounts = GetAccounts(userId)
            };
            response.Model.Count = response.Model.Accounts.Count();
            response.Model.Accounts = response.Model.Accounts.Take(take);
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
        GetResponse<AccountGetResponse> response = new();

        try
        {
            int userId = await GetUserId();

            response.Model = new AccountGetResponse
            {
                Accounts = GetAccounts(userId, search)
            };
            response.Model.Count = response.Model.Accounts.Count();
            response.Model.Accounts = response.Model.Accounts.Skip(offset).Take(take);
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    private IEnumerable<AccountModel> GetAccounts(int userId, string? search = null)
    {
        Expression<Func<AccountModel, bool>>? predicate = null;

        if(!string.IsNullOrEmpty(search))
        {
            predicate = m => m.AccountName.ToLower().Contains(search.ToLower())
                    && m.UserId == userId;
        }
        else
        {
            predicate = m => m.UserId == userId;
        }

        IEnumerable<AccountModel> query = _ftAccountStore.GetAccounts(predicate);

        return query;

    }

    [HttpPost("SaveAccount")]
    public async Task<IActionResult> SaveAccount(AccountModel model,
        [FromServices] IValidator<AccountModel> validator)
    {
        PostResponse<AccountModel> response = new();

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
                int accountId = model.AccountId;

                if (accountId <= 0)
                {
                    accountId = _ftAccountStore.CreateAccount(userId, model);
                }
                else
                {
                    _ftAccountStore.UpdateAccount(userId, model);
                }

                response.Model = _ftAccountStore.GetAccounts(m => m.UserId == userId && m.AccountId == accountId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("DeleteAccount")]
    public async Task<IActionResult> DeleteAccount(AccountIdRequest request,
        [FromServices] IValidator<AccountIdRequest> validator)
    {
        PostResponse<bool> response = new();

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
                response.Model = _ftAccountStore.DeleteAccount(userId, request.AccountId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("CloneAccount")]
    public async Task<IActionResult> CloneAccount(AccountIdRequest request,
        [FromServices] IValidator<AccountIdRequest> validator)
    {
        PostResponse<AccountModel> response = new();

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
                int newAccountId = _ftAccountStore.CloneAccount(userId, request.AccountId);

                if (newAccountId > -1)
                {
                    response.Model = _ftAccountStore.GetAccounts(userId).First(m => m.AccountId == newAccountId);
                }
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
