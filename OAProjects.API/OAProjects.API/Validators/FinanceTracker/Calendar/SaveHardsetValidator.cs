using FluentValidation;
using OAProjects.Models.FinanceTracker.Requests.Calendar;

namespace OAProjects.API.Validators.FinanceTracker.Calendar;

public class SaveHardsetValidator : AbstractValidator<SaveHardsetRequest>
{
    public SaveHardsetValidator()
    {
        RuleFor(m => m.AccountId).NotEqual(0);
        RuleFor(m => m.Date).GreaterThan(DateTime.MinValue);
    }
}
