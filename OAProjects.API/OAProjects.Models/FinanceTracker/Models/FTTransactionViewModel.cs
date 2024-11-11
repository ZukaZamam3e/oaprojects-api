namespace OAProjects.Models.FinanceTracker.Models;
public class FTTransactionViewModel
{
    public FTTransactionModel Transaction { get; set; }

    public IEnumerable<TransactionOffsetModel> Offsets { get; set; }
}
