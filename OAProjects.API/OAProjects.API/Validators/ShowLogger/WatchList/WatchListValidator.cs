using FluentValidation;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.API.Validators.ShowLogger.WatchList;

public class WatchListValidator : AbstractValidator<WatchListModel>
{
    public WatchListValidator()
    {
        RuleFor(x => x.ShowName).NotNull();
        RuleFor(x => x.ShowTypeId).GreaterThan(0);

        // TV Show
        RuleFor(x => x.SeasonNumber).NotNull().When(m => m.ShowTypeId == (int)CodeValueIds.TV);
        RuleFor(x => x.EpisodeNumber).NotNull().When(m => m.ShowTypeId == (int)CodeValueIds.TV);

        // Movies or AMC
        RuleFor(x => x.SeasonNumber).Null().When(m => m.ShowTypeId != (int)CodeValueIds.TV);
        RuleFor(x => x.EpisodeNumber).Null().When(m => m.ShowTypeId != (int)CodeValueIds.TV);
    }
}
