using FluentValidation;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.FinanceTracker.Requests.Calendar;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.FinanceTracker.Calendar;

public class DeleteTransactionValidator : AbstractValidator<DeleteTransactionRequest>
{
    public DeleteTransactionValidator()
    {
        RuleFor(m => m.TransactionId).NotEqual(0);
        RuleFor(m => m.AccountId).NotEqual(0);
    }
}
