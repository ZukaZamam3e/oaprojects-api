namespace OAProjects.Models.FinanceTracker.Models;
public class TransactionOffsetModel
{
    public int TransactionOffsetId { get; set; }

    public int AccountId { get; set; }

    public int UserId { get; set; }

    public int TransactionId { get; set; }

    public DateTime OffsetDate { get; set; }

    public decimal OffsetAmount { get; set; }
}
