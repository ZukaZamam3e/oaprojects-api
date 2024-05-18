using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.Transaction;

namespace OAProjects.API.Responses.ShowLogger.Transaction;

public class TransactionLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> TransactionTypeIds { get; set; }

    public IEnumerable<TransactionModel> Transactions { get; set; }

    public IEnumerable<TransactionItemModel> TransactionItems { get; set; }

    public int Count { get; set; }
}
