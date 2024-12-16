using OAProjects.Models.CatanLogger.Models;

namespace OAProjects.Models.CatanLogger.Responses;
public class GameGetResponse
{
    public IEnumerable<GameModel> Games { get; set; }
    public int Count { get; set; }
}
