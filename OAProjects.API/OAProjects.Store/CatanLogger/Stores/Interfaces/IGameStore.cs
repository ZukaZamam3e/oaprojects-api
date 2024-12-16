using OAProjects.Models.CatanLogger.Models;
using System.Linq.Expressions;

namespace OAProjects.Store.CatanLogger.Stores.Interfaces;
public interface IGameStore
{
    IEnumerable<GameModel> GetGames(int groupId, Expression<Func<GameModel, bool>>? predicate = null);

    GameModel SaveGame(int groupId, int userId, GameModel model);

    bool DeleteGame(int groupId, int userId, int gameId);
}
