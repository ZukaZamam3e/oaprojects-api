using OAProjects.Models.ShowLogger.Models.Transaction;
using System.Reflection;

namespace OAProjects.Models.FinanceTracker.Models;
public class CalendarModel(int userId, int accountId, DateTime startDate, IEnumerable<FTTransactionModel> transactions, IEnumerable<TransactionOffsetModel> offsets)
{
    public int AccountId { get; set; } = accountId;

    public int UserId { get; set; } = userId;

    public string CalendarId => $"{UserId}_{AccountId}";

    public int MyProperty { get; set; }

    public DateTime StartDate { get; set; } = startDate;

    public DateTime EndDate { get; set; }

    public List<DayModel> Days { get; set; } = [];

    public List<FTTransactionModel> Transactions { get; set; } = transactions.ToList();

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
    private const int OCCURS_THREE_MONTH = 2000;

    public bool Calculate(DateTime startDate, DateTime endDate)
    {
        bool updated = false;

        decimal currentTotal = 0;

        if (Days.Count > 0)
        {
            DateTime maxEndDate = Days.Max(m => m.Date);

            if (maxEndDate < startDate)
            {
                startDate = maxEndDate;
            }
        }

        if (Days.Count > 0)
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

            FTTransactionModel? hardset = Transactions.FirstOrDefault(m => m.StartDate == iterDate && m.FrequencyTypeId == HARDSET);

            if (hardset != null)
            {
                currentTotal = GetTransactionAmount(day, hardset, Offsets);
            }

            foreach (FTTransactionModel transaction in Transactions.Where(m => m.StartDate <= iterDate && m.FrequencyTypeId != HARDSET))
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

    public IEnumerable<MonthlyTransactionModel> GetMonthlyTransactions(DateTime day)
    {
        List<MonthlyTransactionModel> monthTransactions = new List<MonthlyTransactionModel>();
        IEnumerable<DayModel> monthDays = Days.Where(m => m.Date.Month == day.Month && m.Date.Year == day.Year);

        //foreach (FTTransactionModel transaction in transactions)
        //{
        //    FTTransactionModel model = transaction;

        //    IEnumerable<DateTime> all = transaction.Dates.Where(m => m.Month == day.Month && m.Year == day.Year);

        //    foreach (DateTime date in all)
        //    {
        //        FTTransactionModel m = model.Clone();

        //        m.StartDate = date;
        //        lst.Add(m);
        //    }
        //}

        // Loop through each day of the month
        // Sort by amounts greatest -> lowest
        // First one shows the date, rest do not
        // Hardset -> Income -> Expenses
        // Last one shows the end of day balance

        DateTime endOfLastMonth = new DateTime(day.Year, day.Month, 1).AddDays(-1);

        decimal endOfDayBalance = Days.FirstOrDefault(m => m.Date == endOfLastMonth)?.Total ?? 0;

        foreach (DayModel monthDay in monthDays)
        {
            IEnumerable<FTTransactionModel> dailyTransactions = monthDay.Transactions
                .Select(m => m.Clone(monthDay.Date))
                .OrderByDescending(m => GetAmount(monthDay.Date, m, Offsets))
                .ThenBy(m => m.FrequencyTypeId);

            if (dailyTransactions.Any())
            {
                foreach (FTTransactionModel transaction in dailyTransactions)
                {
                    MonthlyTransactionModel model = new MonthlyTransactionModel();

                    if (dailyTransactions.First().TransactionId == transaction.TransactionId)
                    {
                        model.TransactionDate = transaction.StartDate;
                    }

                    if (dailyTransactions.Last().TransactionId == transaction.TransactionId)
                    {
                        model.EndOfDayBalance = endOfDayBalance = monthDay.Total;
                    }

                    if (transaction.FrequencyTypeId == HARDSET)
                    {
                        model.EndOfDayBalance = endOfDayBalance = GetAmount(monthDay.Date, transaction, Offsets);
                    }

                    model.TransactionName = transaction.Name;
                    model.TransactionId = transaction.TransactionId;
                    model.FrequencyTypeIdZ = transaction.FrequencyTypeIdZ;
                    model.Url = transaction.TransactionUrl;

                    decimal amount = GetAmount(monthDay.Date, transaction, Offsets);
                    if (transaction.FrequencyTypeId != HARDSET)
                    {
                        if (amount >= 0)
                        {
                            model.Income = amount;
                        }
                        else
                        {
                            model.Expenses = amount;
                        }
                    }

                    monthTransactions.Add(model);
                }
            }
            else
            {
                monthTransactions.Add(new MonthlyTransactionModel
                {
                    TransactionName = "No Transactions",
                    TransactionDate = monthDay.Date,
                    EndOfDayBalance = endOfDayBalance,
                });

            }
        }

        return monthTransactions;
    }

    public IEnumerable<CategoryModel> GetCategories(DateTime day)
    {
        List<CategoryModel> result = new List<CategoryModel>();
        IEnumerable<DayModel> monthDays = Days.Where(m => m.Date.Month == day.Month && m.Date.Year == day.Year);

        IEnumerable<string?> categories = monthDays.SelectMany(m => m.Transactions.Select(m => m.Categories));
        List<string> allCategories = new List<string>();

        foreach (string? category in categories)
        {
            if (!string.IsNullOrEmpty(category))
            {
                string[] split = category.ToLower().Split(',');
                allCategories.AddRange(split);
            }
        }

        foreach (string? category in allCategories.Distinct())
        {
            if (!string.IsNullOrEmpty(category))
            {
                IEnumerable<FTTransactionModel> transactions = monthDays.SelectMany(m => m.Transactions.Where(n => !string.IsNullOrEmpty(n.Categories) && n.Categories.Contains(category, StringComparison.CurrentCultureIgnoreCase)));

                result.Add(new CategoryModel
                {
                    Category = category,
                    Transactions = transactions
                });
            }
        }

        IEnumerable<FTTransactionModel> uncategorized = monthDays.SelectMany(m => m.Transactions.Where(m => string.IsNullOrEmpty(m.Categories)));

        if(uncategorized.Any())
        {
            result.Add(new CategoryModel
            {
                Category = "uncategorized",
                Transactions = uncategorized
            });
        }

        return result;
    }

    private static decimal GetTransactionAmount(DayModel day, FTTransactionModel transaction, IEnumerable<TransactionOffsetModel> offsets)
    {
        TransactionOffsetModel? offset = offsets.FirstOrDefault(m => m.TransactionId == transaction.TransactionId && m.OffsetDate == day.Date);

        day.Transactions.Add(transaction);

        decimal amount = transaction.Amount;

        if (offset is not null)
        {
            amount += offset.OffsetAmount;
            day.Offsets.Add(offset);
        }

        if(transaction.Conditional is not null && transaction.Conditional == OCCURS_THREE_MONTH)
        {
            if(DoesDateOccurThreeMonth(day.Date, transaction.FrequencyTypeId, transaction.Interval))
            {
                amount += transaction.ConditionalAmount ?? 0;
            }
        }

        return amount;
    }

    private static decimal GetAmount(DateTime day, FTTransactionModel transaction, IEnumerable<TransactionOffsetModel> offsets)
    {
        TransactionOffsetModel? offset = offsets.FirstOrDefault(m => m.TransactionId == transaction.TransactionId && m.OffsetDate == day.Date);

        decimal amount = transaction.Amount;

        if (offset is not null)
        {
            amount += offset.OffsetAmount;
        }

        if (transaction.Conditional is not null && transaction.Conditional == OCCURS_THREE_MONTH)
        {
            if (DoesDateOccurThreeMonth(day.Date, transaction.FrequencyTypeId, transaction.Interval))
            {
                amount += transaction.ConditionalAmount ?? 0;
            }
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
            QUARTERLY => checkDate.Day == startDate.Day && ((checkDate.Month - startDate.Month) % 3 == 0),
            YEARLY => checkDate.Day == startDate.Day && checkDate.Month == startDate.Month,
            EVERY_N_DAYS => (checkDate - startDate).Days % interval == 0,
            EVERY_N_WEEKS => (checkDate - startDate).Days % (7 * interval) == 0,
            EVERY_N_MONTHS => ((checkDate.Month - startDate.Month) % interval == 0) && checkDate.Day == startDate.Day,
            _ => false
        };
    }

    private static bool DoesDateOccurThreeMonth(DateTime date, int frequencyType, int? interval)
    {
        int counter = 1;

        for (int i = 1; i <= 2; ++i)
        {
            counter += frequencyType switch
            {
                DAILY => 1,
                WEEKLY => DoesDateHaveSameMonth(date, 7 * i),
                BIWEEKLY => DoesDateHaveSameMonth(date, 14 * i),
                EVERY_N_DAYS => DoesDateHaveSameMonth(date, (interval ?? 0) * i),
                EVERY_N_WEEKS => DoesDateHaveSameMonth(date, (interval ?? 0) * 7 * i),
                _ => 0
            };

            counter += frequencyType switch
            {
                DAILY => 1,
                WEEKLY => DoesDateHaveSameMonth(date, -7 * i),
                BIWEEKLY => DoesDateHaveSameMonth(date, -14 * i),
                EVERY_N_DAYS => DoesDateHaveSameMonth(date, (interval ?? 0) * i * -1),
                EVERY_N_WEEKS => DoesDateHaveSameMonth(date, (interval ?? 0) * 7 * i * -1),
                _ => 0
            };

        }

        return counter >= 3;
    }

    public static int DoesDateHaveSameMonth(DateTime date, int days)
    {
        return date.AddDays(days).Month == date.Month ? 1 : 0;
    }
}