using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.API.Validators.ShowLogger.Info;

public class InfoApiSearchValidator : AbstractValidator<InfoApiSearchModel>
{
    public InfoApiSearchValidator()
    {
        RuleFor(m => m.Name).NotEmpty().WithMessage("Name must be populated.");
        RuleFor(m => m.Name).MinimumLength(3).WithMessage("Name must have at least 3 characters.").When(m => !string.IsNullOrEmpty(m.Name));

        RuleFor(m => (int)m.API).LessThanOrEqualTo((int)INFO_API.TMDB_API).WithMessage("API value not accepted.");

        RuleFor(m => (int)m.Type).LessThanOrEqualTo((int)INFO_TYPE.MOVIE).WithMessage("Type value not accepted.");
    }
}
