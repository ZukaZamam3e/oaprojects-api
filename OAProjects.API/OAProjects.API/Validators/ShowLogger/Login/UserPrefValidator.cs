using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Login;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.ShowLogger.Login;

public class UserPrefValidator : AbstractValidator<UserPrefModel>
{
    public UserPrefValidator()
    {
        
    }
}
