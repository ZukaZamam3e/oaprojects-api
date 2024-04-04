using FluentValidation;
using OAProjects.API.Responses.ShowLogger.Info;

namespace OAProjects.API.Validators.ShowLogger.Info;

public class TvInfoIdValidator : AbstractValidator<TvInfoIdRequest>
{
    public TvInfoIdValidator()
    {
        RuleFor(m => m.TvInfoId).GreaterThan(0);
    }
}
