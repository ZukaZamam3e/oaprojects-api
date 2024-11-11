namespace OAProjects.Models.FinanceTracker.Requests.Calendar;
public class SaveHardsetRequest
{
    public DateTime Date { get; set; }

    public int AccountId { get; set; }

    public decimal Amount { get; set; }
}
