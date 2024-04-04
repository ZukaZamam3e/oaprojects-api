using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.API.Validators.ShowLogger.Info;

public class InfoApiDownloadValidator : AbstractValidator<InfoApiDownloadModel>
{
    public InfoApiDownloadValidator()
    {
        RuleFor(m => (int)m.API).LessThanOrEqualTo((int)INFO_API.TMDB_API).WithMessage("API value not accepted.");

        RuleFor(m => (int)m.Type).LessThanOrEqualTo((int)INFO_TYPE.MOVIE).WithMessage("Type value not accepted.");

        RuleFor(m => m.Id).NotEmpty().WithMessage("Id must be filled.");
    }
}

