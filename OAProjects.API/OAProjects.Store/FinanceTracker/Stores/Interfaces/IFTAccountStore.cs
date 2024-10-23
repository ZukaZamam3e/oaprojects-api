using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.Stores.Interfaces;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IFTAccountStore : IStore
{
    IEnumerable<AccountModel> GetAccounts(int userId);

    int CreateAccount(int userId, AccountModel account);

    int UpdateAccount(int userId, AccountModel account);

    bool DeleteAccount(int userId, int accountId);

    int CloneAccount(int userId, int accountId);
}
