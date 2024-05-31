using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.WatchList;
using OAProjects.Models.ShowLogger.Responses.Info;

namespace OAProjects.API.Validators.ShowLogger.Info;

public class MovieInfoIdValidator : AbstractValidator<MovieInfoIdRequest>
{
    public MovieInfoIdValidator()
    {
        RuleFor(m => m.MovieInfoId).GreaterThan(0);
    }
}
