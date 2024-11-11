namespace OAProjects.Models.FinanceTracker.Models;
public class DayModel
{
    public DateTime Date { get; set; }

    public string DateZ => Date.ToString("MM/dd/yyyy");

    public decimal Total { get; set; }

    public decimal Expenses { get; set; }

    public decimal Income { get; set; }

    public List<FTTransactionModel> Transactions { get; set; }

    public List<TransactionOffsetModel> Offsets { get; set; }
}
