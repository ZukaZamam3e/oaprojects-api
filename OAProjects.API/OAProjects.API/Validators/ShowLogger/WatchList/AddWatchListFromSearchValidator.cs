using FluentValidation;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.API.Validators.ShowLogger.Show;

public class AddWatchListFromSearchValidator : AbstractValidator<AddWatchListFromSearchModel>
{
    public AddWatchListFromSearchValidator()
    {
        RuleFor(m => (int)m.API).LessThanOrEqualTo((int)INFO_API.TMDB_API).WithMessage("API value not accepted.");
        RuleFor(m => (int)m.Type).LessThanOrEqualTo((int)INFO_TYPE.MOVIE).WithMessage("Type value not accepted.");
        RuleFor(m => m.Id).NotEmpty().WithMessage("Id must be filled.");

        RuleFor(x => x.ShowName).NotNull();
        RuleFor(x => x.ShowTypeId).GreaterThan(0);
        RuleFor(x => x.DateAdded).GreaterThan(DateTime.MinValue);

        // TV Show
        RuleFor(x => x.SeasonNumber).NotNull().When(m => m.ShowTypeId == (int)CodeValueIds.TV);
        RuleFor(x => x.EpisodeNumber).NotNull().When(m => m.ShowTypeId == (int)CodeValueIds.TV);

        // Movies or AMC
        RuleFor(x => x.SeasonNumber).Null().When(m => m.ShowTypeId != (int)CodeValueIds.TV);
        RuleFor(x => x.EpisodeNumber).Null().When(m => m.ShowTypeId != (int)CodeValueIds.TV);
    }
}
