using FluentValidation;
using OAProjects.Models.ShowLogger.Responses.Info;

namespace OAProjects.API.Validators.ShowLogger.Info;

public class TvInfoIdValidator : AbstractValidator<TvInfoIdRequest>
{
    public TvInfoIdValidator()
    {
        RuleFor(m => m.TvInfoId).GreaterThan(0);
    }
}
