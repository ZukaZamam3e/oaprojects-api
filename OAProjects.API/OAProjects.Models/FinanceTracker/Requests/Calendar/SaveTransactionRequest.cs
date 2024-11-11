using OAProjects.Models.FinanceTracker.Models;

namespace OAProjects.Models.FinanceTracker.Requests.Calendar;
public class SaveTransactionRequest
{
    public int AccountId { get; set; }

    public DateTime SelectedDate { get; set; }

    public FTTransactionModel Transaction { get; set; }
}
