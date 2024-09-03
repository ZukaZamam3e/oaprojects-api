using OAProjects.Models.ShowLogger.Models.WhatsNext;

namespace OAProjects.Models.ShowLogger.Responses.WhatsNext;
public class WhatsNextLoadSubscriptionsResponse
{
    public IEnumerable<WhatsNextWatchEpisodeModel> Subscriptions { get; set; }

    public int SubscriptionCount { get; set; }
}
