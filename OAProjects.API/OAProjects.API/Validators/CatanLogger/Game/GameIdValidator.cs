using FluentValidation;
using OAProjects.Models.CatanLogger.Requests;

namespace OAProjects.API.Validators.CatanLogger.Game;

public class GameIdValidator : AbstractValidator<GameIdRequest>
{
    public GameIdValidator()
    {
        RuleFor(m => m.GameId).NotEqual(0);
        RuleFor(m => m.GroupId).NotEqual(0);
    }
}
