using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.Stores.Interfaces;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IFTTransactionStore : IStore
{
    IEnumerable<TransactionModel> GetTransactions(int userId, int? transactionId = null, int? accountId = null);

    int CreateTransaction(int userId, int accountId, TransactionModel transaction);

    int UpdateTransaction(int userId, int accountId, TransactionModel transaction);

    bool DeleteTransaction(int userId, int accountId, int transactionId);
}
