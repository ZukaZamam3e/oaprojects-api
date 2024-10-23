using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.Stores.Interfaces;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IFTTransactionOffsetStore : IStore
{
    IEnumerable<TransactionOffsetModel> GetTransactionOffsets(int? transactionId = null, int? accountId = null);

    int CreateTransactionOffset(int userId, int accountId, TransactionOffsetModel offset);

    int UpdateTransactionOffset(int userId, int accountId, TransactionOffsetModel offset);

    bool DeleteTransactionOffset(int userId, int accountId, int offsetId);
}
