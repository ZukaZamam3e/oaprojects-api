namespace OAProjects.Models.FinanceTracker.Requests.Calendar;
public class DeleteTransactionRequest
{
    public int TransactionId { get; set; }

    public int AccountId { get; set; }
}
