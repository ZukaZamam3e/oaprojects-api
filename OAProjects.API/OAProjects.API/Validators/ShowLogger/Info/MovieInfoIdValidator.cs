using FluentValidation;
using OAProjects.API.Requests.WatchList;
using OAProjects.API.Responses.ShowLogger.Info;

namespace OAProjects.API.Validators.ShowLogger.Info;

public class MovieInfoIdValidator : AbstractValidator<MovieInfoIdRequest>
{
    public MovieInfoIdValidator()
    {
        RuleFor(m => m.MovieInfoId).GreaterThan(0);
    }
}
