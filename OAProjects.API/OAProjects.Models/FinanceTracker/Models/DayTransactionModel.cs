namespace OAProjects.Models.FinanceTracker.Models;
public class DayTransactionModel
{
    public FTTransactionModel Transaction { get; set; }

    public TransactionOffsetModel TransactionOffset { get; set; }
}
