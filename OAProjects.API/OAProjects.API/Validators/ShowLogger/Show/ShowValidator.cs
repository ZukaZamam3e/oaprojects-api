using FluentValidation;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.ShowLogger.Show;
public class ShowValidator : AbstractValidator<ShowModel>
{
    public ShowValidator()
    {
        RuleFor(x => x.ShowName).NotNull();
        RuleFor(x => x.ShowTypeId).GreaterThan(0);
        RuleFor(x => x.DateWatched).GreaterThan(DateTime.MinValue);

        // TV Show
        RuleFor(x => x.SeasonNumber).NotNull().When(m => m.ShowTypeId == (int)CodeValueIds.TV);
        RuleFor(x => x.EpisodeNumber).NotNull().When(m => m.ShowTypeId == (int)CodeValueIds.TV);

        // Movies or AMC
        RuleFor(x => x.SeasonNumber).Null().When(m => m.ShowTypeId != (int)CodeValueIds.TV);
        RuleFor(x => x.EpisodeNumber).Null().When(m => m.ShowTypeId != (int)CodeValueIds.TV);
    }
}