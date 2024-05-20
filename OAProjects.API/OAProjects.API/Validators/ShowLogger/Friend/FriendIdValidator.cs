using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.Friend;

namespace OAProjects.API.Validators.ShowLogger.Friend;

public class FriendIdValidator : AbstractValidator<FriendIdRequest>
{
    public FriendIdValidator()
    {
        RuleFor(m => m.FriendId).GreaterThan(0);
    }
}
