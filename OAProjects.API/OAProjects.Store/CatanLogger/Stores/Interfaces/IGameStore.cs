using OAProjects.Models.CatanLogger;

namespace OAProjects.Store.CatanLogger.Stores.Interfaces;
public interface IGameStore
{
    IEnumerable<GameModel> GetGames(int groupId);

    GameModel SaveGame(int groupId, GameModel model);

    bool DeleteGame(int groupId, int gameId);
}
