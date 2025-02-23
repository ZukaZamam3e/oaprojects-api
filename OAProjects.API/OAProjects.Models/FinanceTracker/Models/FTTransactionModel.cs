using OAProjects.Models.ShowLogger.Models.Transaction;

namespace OAProjects.Models.FinanceTracker.Models;

public class FTTransactionModel
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

    public string? Categories { get; set; }

    public int? Conditional { get; set; }

    public string? ConditionalZ { get; set; }

    public decimal? ConditionalAmount { get; set; }


    public FTTransactionModel Clone(DateTime date)
    {
        return new FTTransactionModel
        {
            AccountId = AccountId,
            UserId = UserId,
            TransactionId = TransactionId,
            StartDate = date,
            EndDate = EndDate,
            Amount = Amount,
            Name = Name,
            FrequencyTypeId = FrequencyTypeId,
            FrequencyTypeIdZ = FrequencyTypeIdZ,
            TransactionNotes = TransactionNotes,
            TransactionUrl = TransactionUrl,
            Interval = Interval,
            Categories = Categories,
            Conditional = Conditional,
            ConditionalZ = ConditionalZ,
            ConditionalAmount = ConditionalAmount,
        };
    }
}
