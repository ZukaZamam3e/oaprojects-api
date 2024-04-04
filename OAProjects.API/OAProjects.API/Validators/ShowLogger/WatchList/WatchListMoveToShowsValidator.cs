using FluentValidation;
using OAProjects.API.Requests.WatchList;

namespace OAProjects.API.Validators.ShowLogger.WatchList;

public class WatchListMoveToShowsValidator : AbstractValidator<WatchListMoveToShowsRequest>
{
    public WatchListMoveToShowsValidator()
    {
        RuleFor(m => m.WatchListId).GreaterThan(0);
        RuleFor(m => m.DateWatched).NotEmpty();
    }
}
