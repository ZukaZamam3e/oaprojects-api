using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Friend;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.ShowLogger.Friend;

public class AddFriendValidator : AbstractValidator<AddFriendModel>
{
    public AddFriendValidator()
    {
        RuleFor(m => m.Email).NotEmpty().EmailAddress();
    }
}
