using OAProjects.Models.FinanceTracker.Models;

namespace OAProjects.Models.FinanceTracker.Responses.Calendar;
public class LoadResponse
{
    public int AccountId { get; set; }

    public int Month { get; set; }

    public string MonthYear { get; set; }

    public bool CanGoFurther { get; set; }

    public bool CanGoBack { get; set; }

    public decimal MonthIncome { get; set; }

    public decimal MonthExpenses { get; set; }

    public DayModel LowestDay { get; set; }

    public IEnumerable<DayModel> Days { get; set; }

    public IEnumerable<AccountModel> Accounts { get; set; }
}
