using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_TRANSACTION
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int TRANSACTION_ID { get; set; }

    public int USER_ID { get; set; }

    public int TRANSACTION_TYPE_ID { get; set; }

    public int? SHOW_ID { get; set; }

    public string ITEM { get; set; }

    public decimal COST_AMT { get; set; }

    public int QUANTITY { get; set; }

    public string? TRANSACTION_NOTES { get; set; }

    public DateTime TRANSACTION_DATE { get; set; }
}
