using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.WhatsNext;

namespace OAProjects.API.Validators.ShowLogger.WhatsNext;

public class WhatsNextSubIdValidator : AbstractValidator<WhatsNextSubIdRequest>
{
    public WhatsNextSubIdValidator()
    {
        RuleFor(m => m.WhatsNextSubId).GreaterThan(0);
    }
}