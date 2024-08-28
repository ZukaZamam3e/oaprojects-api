namespace OAProjects.Models.ShowLogger.Models.WhatsNext;
public class WhatsNextSubscriptionModel
{
    public int UserId { get; set; }

    public int TvInfoId { get; set; }

    public DateTime SubscribeDate { get; set; }

    public bool IncludeSpecials { get; set; }
}
