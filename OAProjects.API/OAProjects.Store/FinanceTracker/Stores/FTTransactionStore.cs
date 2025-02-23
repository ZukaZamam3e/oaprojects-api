using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.Store.FinanceTracker.Stores;
public class FTTransactionStore(FinanceTrackerDbContext _context) : IFTTransactionStore
{
    public IEnumerable<FTTransactionModel> GetTransactions(int userId, int? transactionId = null, int? accountId = null)
    {
        Expression<Func<FT_TRANSACTION, bool>>? predicate = m => true;

        if (transactionId != null)
        {
            predicate = m => m.TRANSACTION_ID == transactionId && m.USER_ID == userId;
        }
        else if (accountId != null)
        {
            predicate = m => m.ACCOUNT_ID == accountId && m.USER_ID == userId;
        }

        Dictionary<int, string> frequencyTypeIds = _context.FT_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)FT_CodeTableIds.FREQUENCY_TYPES)
            .ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        Dictionary<int, string> conditionals = _context.FT_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)FT_CodeTableIds.CONDITIONAL)
            .ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        FT_TRANSACTION[] transactionEntities = [.. _context.FT_TRANSACTION.Where(predicate)];

        IEnumerable<FTTransactionModel> query = transactionEntities.Select(m => new FTTransactionModel
        {
            TransactionId = m.TRANSACTION_ID,
            AccountId = m.ACCOUNT_ID,
            UserId = m.USER_ID,
            Name = m.TRANSACTION_NAME,
            Amount = m.TRANSACTION_AMOUNT,
            StartDate = m.START_DATE,
            EndDate = m.END_DATE,
            FrequencyTypeId = m.FREQUENCY_TYPE_ID,
            FrequencyTypeIdZ = frequencyTypeIds[m.FREQUENCY_TYPE_ID],
            TransactionUrl = m.TRANSACTION_URL,
            TransactionNotes = m.TRANSACTION_NOTES,
            Categories = m.CATEGORIES,
            Conditional = m.CONDITIONAL,
            ConditionalZ = m.CONDITIONAL is not null ? conditionals[(int)m.CONDITIONAL] : "",
            ConditionalAmount = m.CONDITIONAL_AMOUNT,
            Interval = m.INTERVAL,
        });

        return query;
    }

    public int CreateTransaction(int userId, int accountId, FTTransactionModel transaction)
    {
        FT_TRANSACTION entity = new FT_TRANSACTION
        {
            ACCOUNT_ID = accountId,
            USER_ID = userId,
            TRANSACTION_NAME = transaction.Name,
            TRANSACTION_AMOUNT = transaction.Amount,
            START_DATE = transaction.StartDate,
            END_DATE = transaction.EndDate,
            FREQUENCY_TYPE_ID = transaction.FrequencyTypeId,
            TRANSACTION_URL = transaction.TransactionUrl,
            TRANSACTION_NOTES = transaction.TransactionNotes, 
            INTERVAL = transaction.Interval,
            CATEGORIES = transaction.Categories,
            CONDITIONAL = transaction.Conditional,
            CONDITIONAL_AMOUNT = transaction.ConditionalAmount,
        };

        _context.FT_TRANSACTION.Add(entity);
        _context.SaveChanges();

        int id = entity.TRANSACTION_ID;
        return id;
    }

    public int UpdateTransaction(int userId, int accountId, FTTransactionModel transaction)
    {
        int result = 0;
        FT_TRANSACTION? entity = _context.FT_TRANSACTION.FirstOrDefault(m => m.TRANSACTION_ID == transaction.TransactionId && m.ACCOUNT_ID == accountId && m.USER_ID == userId);

        if (entity != null)
        {
            entity.TRANSACTION_NAME = transaction.Name;
            entity.TRANSACTION_AMOUNT = transaction.Amount;
            entity.START_DATE = transaction.StartDate;
            entity.END_DATE = transaction.EndDate;
            entity.FREQUENCY_TYPE_ID = transaction.FrequencyTypeId;
            entity.TRANSACTION_URL = transaction.TransactionUrl;
            entity.TRANSACTION_NOTES = transaction.TransactionNotes;
            entity.INTERVAL = transaction.Interval;
            entity.CATEGORIES = transaction.Categories;
            entity.CONDITIONAL = transaction.Conditional;
            entity.CONDITIONAL_AMOUNT = transaction.ConditionalAmount;

            result = _context.SaveChanges();
        }

        return result;
    }

    public bool DeleteTransaction(int userId, int accountId, int transactionId)
    {
        bool result = false;

        FT_TRANSACTION? entity = _context.FT_TRANSACTION.FirstOrDefault(m => m.TRANSACTION_ID == transactionId && m.ACCOUNT_ID == accountId && m.USER_ID == userId);

        if (entity != null)
        {
            IEnumerable<FT_TRANSACTION_OFFSET> offsetEntities = _context.FT_TRANSACTION_OFFSET.Where(m => m.TRANSACTION_ID == transactionId);

            _context.FT_TRANSACTION_OFFSET.RemoveRange(offsetEntities);
            _context.FT_TRANSACTION.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }
}



