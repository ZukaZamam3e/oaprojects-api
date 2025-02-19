using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.Watched;

namespace OAProjects.API.Validators.ShowLogger.Watched;

public class WatchedIdValidator : AbstractValidator<WatchedIdRequest>
{
    public WatchedIdValidator()
    {
        RuleFor(m => m.WatchedId).GreaterThan(0);
    }
}