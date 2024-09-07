using OAProjects.Models.ShowLogger.Models.WhatsNext;

namespace OAProjects.Models.ShowLogger.Responses.WhatsNext;
public class WhatsNextLoadResponse
{
    public IEnumerable<WhatsNextShowModel> WhatsNext { get; set; }

    public int Count { get; set; }
}
