using FluentValidation;
using OAProjects.Models.ShowLogger.Models.WatchList;
using OAProjects.Models.ShowLogger.Models.WhatsNext;

namespace OAProjects.API.Validators.ShowLogger.WhatsNext;

public class WhatsNextSubValidator : AbstractValidator<WhatsNextWatchEpisodeModel>
{
    public WhatsNextSubValidator()
    {
        RuleFor(x => x.TvInfoId).GreaterThan(0);
    }
}
