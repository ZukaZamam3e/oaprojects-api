using FluentValidation;
using OAProjects.API.Requests.Friend;

namespace OAProjects.API.Validators.ShowLogger.Friend;

public class FriendRequestIdValidator : AbstractValidator<FriendRequestIdRequest>
{
    public FriendRequestIdValidator()
    {
        RuleFor(m => m.FriendRequestId).GreaterThan(0);
    }
}
