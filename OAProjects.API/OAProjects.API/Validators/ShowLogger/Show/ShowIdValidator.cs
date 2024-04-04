using FluentValidation;
using OAProjects.API.Requests.Show;

namespace OAProjects.API.Validators.ShowLogger.Show;

public class ShowIdValidator : AbstractValidator<ShowIdRequest>
{
    public ShowIdValidator()
    {
        RuleFor(x => x.ShowId).GreaterThan(0);
    }
}
