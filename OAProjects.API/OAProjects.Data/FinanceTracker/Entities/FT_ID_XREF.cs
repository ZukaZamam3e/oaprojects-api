using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.FinanceTracker.Entities;
public enum FTTableIds
{
    FT_CODE_VALUE = 1,
    FT_ACCOUNT,
    FT_TRANSACTION,
    FT_TRANSACTION_OFFSET
}

public class FT_ID_XREF
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID_XREF_ID { get; set; }

    public int TABLE_ID { get; set; }

    public int OLD_ID { get; set; }

    public int NEW_ID { get; set; }
}
