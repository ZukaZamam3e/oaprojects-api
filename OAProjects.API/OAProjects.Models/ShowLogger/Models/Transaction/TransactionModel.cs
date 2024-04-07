namespace OAProjects.Models.ShowLogger.Models.Transaction;
public class TransactionModel
{
    public int UserId { get; set; }

    public int TransactionId { get; set; }

    public int TransactionTypeId { get; set; }

    public string? TransactionTypeIdZ { get; set; }

    public int? ShowId { get; set; }

    public string? ShowIdZ { get; set; }

    public string Item { get; set; }

    public decimal CostAmt { get; set; }

    public int Quantity { get; set; }

    public string? TransactionNotes { get; set; }

    public DateTime TransactionDate { get; set; }
}
