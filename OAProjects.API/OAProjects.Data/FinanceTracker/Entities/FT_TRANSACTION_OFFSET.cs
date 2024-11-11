using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.FinanceTracker.Entities;
public class FT_TRANSACTION_OFFSET
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TRANSACTION_OFFSET_ID { get; set; }

    public int ACCOUNT_ID { get; set; }

    public int USER_ID { get; set; }

    public int TRANSACTION_ID { get; set; }

    public DateTime OFFSET_DATE { get; set; }

    public decimal OFFSET_AMOUNT { get; set; }
}
