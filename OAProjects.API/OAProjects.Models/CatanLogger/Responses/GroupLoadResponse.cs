using OAProjects.Models.CatanLogger.Models;

namespace OAProjects.Models.CatanLogger.Responses;
public class GroupLoadResponse
{
    public IEnumerable<GroupModel> Groups { get; set; }
    public int Count { get; set; }
}
