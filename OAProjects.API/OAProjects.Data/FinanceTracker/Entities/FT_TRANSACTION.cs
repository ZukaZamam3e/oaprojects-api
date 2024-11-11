using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.FinanceTracker.Entities;
public class FT_TRANSACTION
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TRANSACTION_ID { get; set; }

    public int ACCOUNT_ID { get; set; }

    public int USER_ID { get; set; }

    public string TRANSACTION_NAME { get; set; }

    public DateTime START_DATE { get; set; }

    public DateTime? END_DATE { get; set; }

    public decimal TRANSACTION_AMOUNT { get; set; }

    public int FREQUENCY_TYPE_ID { get; set; }

    public int? INTERVAL { get; set; }

    public string? TRANSACTION_NOTES { get; set; }

    public string? TRANSACTION_URL { get; set; }

    public string? CATEGORIES { get; set; }
}