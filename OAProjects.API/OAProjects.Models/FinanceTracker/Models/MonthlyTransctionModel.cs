namespace OAProjects.Models.FinanceTracker.Models;

public class MonthlyTransactionModel
{
    public int TransactionId { get; set; }

    public DateTime? TransactionDate { get; set; }

    public string TransactionName { get; set; }

    public string? FrequencyTypeIdZ { get; set; }

    public decimal? Income { get; set; }

    public decimal? Expenses { get; set; }

    public decimal? EndOfDayBalance { get; set; }

    public string? Url { get; set; }
}
