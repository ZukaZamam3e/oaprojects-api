using FluentValidation;
using OAProjects.API.Requests.WatchList;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;

namespace OAProjects.API.Validators.ShowLogger.UnlinkedShows;

public class LinkShowValidator : AbstractValidator<LinkShowModel>
{
    public LinkShowValidator()
    {
        RuleFor(m => m.ShowName).NotEmpty();
        RuleFor(m => m.ShowTypeId).GreaterThanOrEqualTo(1000);
        RuleFor(m => m.InfoId).GreaterThan(0);
    }
}
