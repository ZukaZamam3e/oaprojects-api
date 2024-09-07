using OAProjects.Models.ShowLogger.Models.WhatsNext;

namespace OAProjects.Models.ShowLogger.Responses.WhatsNext;
public class WhatsNextGetSubscriptionsResponse
{
    public IEnumerable<WhatsNextWatchEpisodeModel> Subscriptions { get; set; }

    public int Count { get; set; }
}
