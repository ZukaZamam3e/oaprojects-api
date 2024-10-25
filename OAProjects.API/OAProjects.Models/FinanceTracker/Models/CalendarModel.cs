namespace OAProjects.Models.FinanceTracker.Models;
public class CalendarModel(int userId, int accountId, DateTime startDate, IEnumerable<TransactionModel> transactions, IEnumerable<TransactionOffsetModel> offsets)
{
    public int AccountId { get; set; } = accountId;

    public int UserId { get; set; } = userId;

    public string CalendarId => $"{UserId}_{AccountId}";

    public int MyProperty { get; set; }

    public DateTime StartDate { get; set; } = startDate;

    public DateTime EndDate { get; set; }

    public List<DayModel> Days { get; set; } = [];

    public List<TransactionModel> Transactions { get; set; } = transactions.ToList();

    public List<TransactionOffsetModel> Offsets { get; set; } = offsets.ToList();

    private const int HARDSET = 1000;
    private const int SINGLE = 1001;
    private const int DAILY = 1002;
    private const int WEEKLY = 1003;
    private const int BIWEEKLY = 1004;
    private const int EVERFOURWEEKS = 1005;
    private const int MONTHLY = 1006;
    private const int QUARTERLY = 1007;
    private const int YEARLY = 1008;
    private const int EVERY_N_DAYS = 1009;
    private const int EVERY_N_WEEKS = 1010;
    private const int EVERY_N_MONTHS = 1011;

    public bool Calculate(DateTime startDate, DateTime endDate)
    {
        bool updated = false;

        decimal currentTotal = 0;

        if(Days.Count > 0)
        {
            Days = Days.Where(m => m.Date < startDate).ToList();
            currentTotal = Days.LastOrDefault()?.Total ?? 0;
        }

        if (EndDate < endDate)
        {
            EndDate = endDate;
        }

        DateTime iterDate = startDate;

        while (iterDate <= endDate)
        {
            DayModel? day = Days.FirstOrDefault(m => m.Date == iterDate);

            if (day == null)
            {
                updated = true;
                day = new DayModel
                {
                    Date = iterDate,
                    Transactions = [],
                    Offsets = []
                };

                Days.Add(day);
            }

            TransactionModel? hardset = Transactions.FirstOrDefault(m => m.StartDate == iterDate && m.FrequencyTypeId == HARDSET);

            if (hardset != null)
            {
                currentTotal = GetTransactionAmount(day, hardset, Offsets);
            }

            foreach (TransactionModel transaction in Transactions.Where(m => m.StartDate <= iterDate && m.FrequencyTypeId != HARDSET))
            {
                if (DoesDateIntersect(transaction.StartDate, transaction.EndDate, iterDate, transaction.FrequencyTypeId, transaction.Interval))
                {
                    decimal amount = GetTransactionAmount(day, transaction, Offsets);

                    if (amount >= 0)
                    {
                        day.Income += amount;
                    }
                    else
                    {
                        day.Expenses += amount;
                    }
                }
            }

            currentTotal += day.Income + day.Expenses;
            day.Total = currentTotal;

            iterDate = iterDate.AddDays(1);
        }

        return updated;
    }

    private static decimal GetTransactionAmount(DayModel day, TransactionModel transaction, IEnumerable<TransactionOffsetModel> offsets)
    {
        TransactionOffsetModel? offset = offsets.FirstOrDefault(m => m.TransactionId == transaction.TransactionId && m.OffsetDate == day.Date);

        day.Transactions.Add(transaction);

        decimal amount = transaction.Amount;

        if (offset is not null)
        {
            amount += offset.OffsetAmount;
            day.Offsets.Add(offset);
        }

        return amount;
    }

    private static bool DoesDateIntersect(DateTime startDate, DateTime? endDate, DateTime checkDate, int frequencyType, int? interval)
    {
        if (checkDate < startDate
            || (endDate is not null && checkDate > endDate))
        {
            return false;
        }

        return frequencyType switch
        {
            SINGLE => checkDate == startDate,
            DAILY => true,
            WEEKLY => (checkDate - startDate).Days % 7 == 0,
            BIWEEKLY => (checkDate - startDate).Days % 14 == 0,
            EVERFOURWEEKS => (checkDate - startDate).Days % 28 == 0,
            MONTHLY => checkDate.Day == startDate.Day
                || (checkDate.Day == DateTime.DaysInMonth(checkDate.Year, checkDate.Month)
                    && startDate.Day > checkDate.Day),
            QUARTERLY => checkDate.Day == startDate.Day && (checkDate.Month - startDate.Month % 3 == 0),
            YEARLY => checkDate.Day == startDate.Day && checkDate.Month == startDate.Month,
            EVERY_N_DAYS => (checkDate - startDate).Days % interval == 0,
            EVERY_N_WEEKS => (checkDate - startDate).Days % (7 * interval) == 0,
            EVERY_N_MONTHS => checkDate.Month - startDate.Month % interval == 0,
            _ => false
        };
    }
}

// response object will have the low day
