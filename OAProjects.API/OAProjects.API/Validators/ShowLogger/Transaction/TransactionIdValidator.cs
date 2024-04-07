using FluentValidation;
using OAProjects.API.Requests.Transaction;

namespace OAProjects.API.Validators.ShowLogger.Transaction;

public class TransactionIdValidator : AbstractValidator<TransactionIdRequest>
{
    public TransactionIdValidator()
    {
        RuleFor(m => m.TransactionId).GreaterThan(0);
    }
}
