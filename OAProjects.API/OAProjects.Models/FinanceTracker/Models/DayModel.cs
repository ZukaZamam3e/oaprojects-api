namespace OAProjects.Models.FinanceTracker.Models;
public class DayModel
{
    public DateTime Date { get; set; }

    public decimal Total { get; set; }

    public decimal Expenses { get; set; }

    public decimal Income { get; set; }

    public List<TransactionModel> Transactions { get; set; }

    public List<TransactionOffsetModel> Offsets { get; set; }

    public override string ToString()
    {
        return Date.ToString("MM/dd/yyyy");
    }
}
