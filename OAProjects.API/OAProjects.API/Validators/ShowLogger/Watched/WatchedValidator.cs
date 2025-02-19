using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Watched;

namespace OAProjects.API.Validators.ShowLogger.Watched;

public class WatchedValidator : AbstractValidator<WatchedModel>
{
    public WatchedValidator()
    {
    }
}
