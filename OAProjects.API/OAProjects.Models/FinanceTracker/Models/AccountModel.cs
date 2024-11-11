namespace OAProjects.Models.FinanceTracker.Models;
public class AccountModel
{
    public int AccountId { get; set; }

    public int UserId { get; set; }

    public string AccountName { get; set; }

    public bool DefaultIndc { get; set; }
}
