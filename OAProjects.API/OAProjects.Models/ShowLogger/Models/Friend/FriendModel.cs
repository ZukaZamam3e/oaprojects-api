namespace OAProjects.Models.ShowLogger.Models.Friend;
public class FriendModel
{
    public int Id { get; set; }

    public int FriendUserId { get; set; }

    public string FriendEmail { get; set; }

    public DateTime CreatedDate { get; set; }

    public bool IsPending { get; set; }

    public string IsPendingZ => IsPending ? "Pending" : "Added";
}
