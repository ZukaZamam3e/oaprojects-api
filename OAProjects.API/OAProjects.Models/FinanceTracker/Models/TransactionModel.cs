namespace OAProjects.Models.FinanceTracker.Models;

public class TransactionModel
{
    public int TransactionId { get; set; }

    public int AccountId { get; set; }

    public int UserId { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public decimal Amount { get; set; }

    public string Name { get; set; }

    public int FrequencyTypeId { get; set; }

    public string? FrequencyTypeIdZ { get; set; }

    public string? TransactionNotes { get; set; }

    public string? TransactionUrl { get; set; }

    public int? Interval { get; set; }

    public DateTime? OffsetDate { get; set; }

    public decimal? OffsetAmount { get; set; }
}
