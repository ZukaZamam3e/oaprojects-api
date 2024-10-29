using OAProjects.Models.FinanceTracker.Models;

namespace OAProjects.Models.FinanceTracker.Responses.Account;
public class AccountLoadResponse
{
    public IEnumerable<AccountModel> Accounts { get; set; }
    public int Count { get; set; }
}
