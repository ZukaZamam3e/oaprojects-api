using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.ShowLogger.Models.Show;
using System.Linq.Expressions;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IAccountStore
{
    IEnumerable<AccountModel> GetAccounts(Expression<Func<ShowInfoModel, bool>>? predicate = null);

    int CreateAccount(AccountModel account);

    int UpdateAccount(AccountModel account);
    bool DeleteAccount(AccountModel account);
}
