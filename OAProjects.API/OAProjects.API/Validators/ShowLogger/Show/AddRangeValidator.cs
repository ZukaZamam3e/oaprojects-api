using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.ShowLogger.Show;

public class AddRangeValidator : AbstractValidator<AddRangeModel>
{
    public AddRangeValidator()
    {
        RuleFor(m => m.ShowName).NotEmpty();
        RuleFor(m => m.DateWatched).NotEmpty();

        RuleFor(m => m.SeasonNumber).NotEmpty();
        RuleFor(m => m.StartEpisode).LessThan(m => m.EndEpisode).WithMessage("Start episode must be less than End episode.");
    }
}
