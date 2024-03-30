using FluentValidation;
using OAProjects.API.Requests.Friend;
using OAProjects.API.Requests.WatchList;

namespace OAProjects.API.Validators.ShowLogger.WatchList;

public class WatchListIdValidator : AbstractValidator<WatchListIdRequest>
{
    public WatchListIdValidator()
    {
        RuleFor(m => m.WatchListId).GreaterThan(0);
    }
}

