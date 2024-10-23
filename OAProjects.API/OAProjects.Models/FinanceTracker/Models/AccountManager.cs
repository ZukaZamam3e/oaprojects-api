namespace OAProjects.Models.FinanceTracker.Models;
public class AccountManager
{
    private const int HARDSET = 1000;
    private const int SINGLE = 1001;
    private const int DAILY = 1002;
    private const int WEEKLY = 1003;
    private const int BIWEEKLY = 1004;
    private const int EVERFOURWEEKS = 1005;
    private const int MONTHLY = 1006;
    private const int QUARTERLY = 1007;
    private const int YEARLY = 1008;

    public CalendarModel Calendar { get; set; }

    public void Calculate(int userId, int accountId, DateTime startDate, DateTime endDate, IEnumerable<TransactionModel> transactions, IEnumerable<TransactionOffsetModel> offsets)
    {
        Calendar ??= new CalendarModel
        {
            AccountId = accountId,
            UserId = userId,
            StartDate = startDate,
            EndDate = endDate,
        };

        Calendar.Transactions = transactions.ToList();
        Calendar.Offsets = offsets.ToList();

        decimal currentTotal = 0;

        if(Calendar.StartDate > startDate)
        {
            Calendar.StartDate = startDate;
            Calendar.Days = [];
        }
        else
        {
            Calendar.Days = Calendar.Days.Where(m => m.Date < startDate).ToList();
            currentTotal = Calendar.Days.Last().Total;
        }

        if (Calendar.EndDate < endDate)
        {
            Calendar.EndDate = endDate;
        }

        DateTime iterDate = startDate;

        while (iterDate <= endDate)
        {
            DayModel? day = Calendar.Days.FirstOrDefault(m => m.Date == iterDate);

            if (day == null)
            {
                day = new DayModel
                {
                    Date = iterDate,
                    Transactions = [],
                    Offsets = []
                };

                Calendar.Days.Add(day);
            }

            IEnumerable<TransactionModel> dayTransactions = Calendar.Transactions.Where(m => m.StartDate == iterDate);

            TransactionModel? hardset = dayTransactions.FirstOrDefault(m => m.FrequencyTypeId == HARDSET);

            if(hardset != null)
            {
                currentTotal = GetTransactionAmount(day, hardset, Calendar.Offsets);
            }

            foreach(TransactionModel transaction in dayTransactions.Where(m => m.FrequencyTypeId != HARDSET))
            {
                if (DoesDateIntersect(transaction.StartDate, transaction.EndDate, iterDate, transaction.FrequencyTypeId))
                {
                    decimal amount = GetTransactionAmount(day, transaction, Calendar.Offsets);

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

    private static bool DoesDateIntersect(DateTime startDate, DateTime? endDate, DateTime checkDate, int checkType)
    {
        if(checkDate < startDate
            || (endDate is not null && checkDate > endDate))
        {
            return false;
        }

        return checkType switch
        {
            SINGLE => false,
            DAILY => true,
            WEEKLY => (checkDate - startDate).Days % 7 == 0,
            BIWEEKLY => (checkDate - startDate).Days % 14 == 0,
            EVERFOURWEEKS => (checkDate - startDate).Days % 28 == 0,
            MONTHLY => checkDate.Day == startDate.Day,
            QUARTERLY => checkDate.Month - startDate.Month % 3 == 0,
            YEARLY => checkDate.Day == startDate.Day && checkDate.Month == startDate.Month,
            _ => false
        };
    }


}