using FluentValidation;
using OAProjects.Models.FinanceTracker.Requests.Account;

namespace OAProjects.API.Validators.FinanceTracker.Account;

public class AccountIdValidator: AbstractValidator<AccountIdRequest>
{
    public AccountIdValidator()
    {
        RuleFor(m => m.AccountId).GreaterThan(0);
    }
}
