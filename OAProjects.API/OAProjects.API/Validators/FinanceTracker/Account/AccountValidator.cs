using FluentValidation;
using OAProjects.Models.FinanceTracker.Models;

namespace OAProjects.API.Validators.FinanceTracker.Account;

public class AccountValidator : AbstractValidator<AccountModel>
{
    public AccountValidator()
    {
        RuleFor(m => m.AccountName).NotEmpty();
    }
}
