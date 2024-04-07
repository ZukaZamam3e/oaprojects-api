using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Show;
public class ShowTransactionModel
{
    public int TransactionId { get; set; }

    public int TransactionTypeId { get; set; }

    public string? TransactionTypeIdZ { get; set; }

    public string Item { get; set; }

    public decimal CostAmt { get; set; }

    public int Quantity { get; set; }

    public string? TransactionNotes { get; set; }

    public bool DeleteTransaction { get; set; }
}
