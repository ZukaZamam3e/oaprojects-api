using OAProjects.Models.ShowLogger.Models.Transaction;

namespace OAProjects.API.Responses.ShowLogger.Transaction;

public class TransactionGetResponse
{
    public IEnumerable<TransactionModel> Transactions { get; set; }

    public int Count { get; set; }
}
