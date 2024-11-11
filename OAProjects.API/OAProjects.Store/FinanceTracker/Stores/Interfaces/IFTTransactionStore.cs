using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.Stores.Interfaces;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IFTTransactionStore : IStore
{
    IEnumerable<FTTransactionModel> GetTransactions(int userId, int? transactionId = null, int? accountId = null);

    int CreateTransaction(int userId, int accountId, FTTransactionModel transaction);

    int UpdateTransaction(int userId, int accountId, FTTransactionModel transaction);

    bool DeleteTransaction(int userId, int accountId, int transactionId);
}
