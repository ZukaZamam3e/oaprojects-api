using FluentValidation;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.ShowLogger.Show;

public class ShowTransactionValidator : AbstractValidator<ShowTransactionModel>
{
    public ShowTransactionValidator()
    {
        RuleFor(m => m.Item).NotEmpty();
        RuleFor(m => m.CostAmt).GreaterThanOrEqualTo(0).WithMessage(m => $"{m.Item} cannot have a negative amount.");
        RuleFor(m => m.Quantity).GreaterThanOrEqualTo(1).WithMessage(m => $"{m.Item} cannot have a quantity less than 0.");
        RuleFor(m => m.TransactionTypeId).GreaterThanOrEqualTo((int)CodeValueIds.ALIST_TICKET).WithMessage(m => $"{m.Item} transaction type not valid.");
        RuleFor(m => m.TransactionTypeId).LessThanOrEqualTo((int)CodeValueIds.TAX).WithMessage(m => $"{m.Item} transaction type not valid.");
    }
}
