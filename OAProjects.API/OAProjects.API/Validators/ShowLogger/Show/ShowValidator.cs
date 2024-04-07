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

        RuleFor(x => x.RestartBinge).NotNull();

        RuleFor(m => m.Transactions).Must((m,t) =>
        {
            if(m.ShowTypeId == (int)CodeValueIds.TV || m.ShowTypeId == (int)CodeValueIds.MOVIE)
            {
                if(t == null)
                {
                    return false;
                }
                else if(t.Count(n => n.DeleteTransaction) != t.Count())
                {
                    return false;
                }
            }


            return true;
        }).WithMessage("TV or Movies cannot have transactions. If any exists, they must be deleted.");
    }
}