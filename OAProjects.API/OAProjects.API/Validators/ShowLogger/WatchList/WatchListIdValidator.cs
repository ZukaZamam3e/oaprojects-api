using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.Friend;
using OAProjects.Models.ShowLogger.Requests.WatchList;

namespace OAProjects.API.Validators.ShowLogger.WatchList;

public class WatchListIdValidator : AbstractValidator<WatchListIdRequest>
{
    public WatchListIdValidator()
    {
        RuleFor(m => m.WatchListId).GreaterThan(0);
    }
}

