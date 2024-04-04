using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;

namespace OAProjects.API.Validators.ShowLogger.UnlinkedShows;

public class UpdateUnlinkedNameValidator : AbstractValidator<UpdateUnlinkedNameModel>
{
    public UpdateUnlinkedNameValidator()
    {
        RuleFor(m => m.ShowName).NotEmpty();
        RuleFor(m => m.NewShowName).NotEmpty();
        RuleFor(m => m.ShowTypeId).GreaterThanOrEqualTo(1000);
    }
}
