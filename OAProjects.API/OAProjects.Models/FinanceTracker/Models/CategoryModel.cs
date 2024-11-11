namespace OAProjects.Models.FinanceTracker.Models;
public class CategoryModel
{
    public string Category { get; set; }

    public IEnumerable<FTTransactionModel> Transactions { get; set; } = new List<FTTransactionModel>();

    public decimal Total => Transactions.Sum(m => m.Amount);
}
