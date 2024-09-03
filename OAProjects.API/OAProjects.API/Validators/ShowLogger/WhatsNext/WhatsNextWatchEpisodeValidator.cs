using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.WhatsNext;

namespace OAProjects.API.Validators.ShowLogger.WhatsNext;

public class WhatsNextWatchEpisodeValidator : AbstractValidator<WhatsNextWatchEpisodeRequest>
{
    public WhatsNextWatchEpisodeValidator()
    {
        RuleFor(x => x.TvEpisodeInfoId).GreaterThan(0);
    }
}
