using FluentValidation;
using OAProjects.Models.CatanLogger.Models;

namespace OAProjects.API.Validators.CatanLogger.Game;

public class GameValidator : AbstractValidator<GameModel>
{
    public GameValidator()
    {
    }
}