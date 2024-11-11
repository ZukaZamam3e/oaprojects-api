using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IFTAccountStore : IStore
{
    IEnumerable<AccountModel> GetAccounts(int userId);

    IEnumerable<AccountModel> GetAccounts(Expression<Func<AccountModel, bool>>? predicate = null);

    int CreateAccount(int userId, AccountModel account);

    int UpdateAccount(int userId, AccountModel account);

    bool DeleteAccount(int userId, int accountId);

    int CloneAccount(int userId, int accountId);
}
