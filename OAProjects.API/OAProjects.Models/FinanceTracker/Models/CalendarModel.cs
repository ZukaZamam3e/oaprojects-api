using System.Diagnostics.Contracts;

namespace OAProjects.Models.FinanceTracker.Models;
public class CalendarModel
{
    public int AccountId { get; set; }

    public int UserId { get; set; }

    public string CalendarId => $"{UserId}_{AccountId}";

    public int MyProperty { get; set; }

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public List<DayModel> Days { get; set; }

    public List<TransactionModel> Transactions { get; set; }

    public List<TransactionOffsetModel> Offsets { get; set; }
}

// response object will have the low day
