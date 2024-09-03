using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Models.WhatsNext;
public class CreateSubscriptionModel
{
    public DateTime SubscribeDate { get; set; }

    public bool InculdeSpecials { get; set; }

    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Id { get; set; }
}
