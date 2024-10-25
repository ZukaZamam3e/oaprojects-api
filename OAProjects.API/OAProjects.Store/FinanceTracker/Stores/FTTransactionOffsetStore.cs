using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace OAProjects.Store.FinanceTracker.Stores;
public class FTTransactionOffsetStore(FinanceTrackerDbContext _context) : IFTTransactionOffsetStore
{
    public IEnumerable<TransactionOffsetModel> GetTransactionOffsets(int userId, int? transactionId = null, int? accountId = null)
    {
        Expression<Func<FT_TRANSACTION_OFFSET, bool>>? predicate = m => true;

        if (transactionId != null)
        {
            predicate = m => m.TRANSACTION_ID == transactionId && m.USER_ID == userId;
        }
        else if (accountId != null)
        {
            predicate = m => m.ACCOUNT_ID == accountId && m.USER_ID == userId;
        }

        Dictionary<int, string> frequencyTypeIds = _context.FT_CODE_VALUE
            .Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.TRANSACTION_TYPE_ID)
            .ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);

        FT_TRANSACTION_OFFSET[] offsetEntities = _context.FT_TRANSACTION_OFFSET.Where(predicate).ToArray();

        IEnumerable<TransactionOffsetModel> query = offsetEntities.Select(m => new TransactionOffsetModel
        {
            TransactionId = m.TRANSACTION_ID,
            AccountId = m.ACCOUNT_ID,
            UserId = m.USER_ID,
            OffsetAmount = m.OFFSET_AMOUNT,
            OffsetDate = m.OFFSET_DATE,
        });

        return query;
    }

    public int CreateTransactionOffset(int userId, int accountId, TransactionOffsetModel offset)
    {
        FT_TRANSACTION_OFFSET entity = new FT_TRANSACTION_OFFSET
        {
            ACCOUNT_ID = offset.AccountId,
            OFFSET_AMOUNT = offset.OffsetAmount,
            OFFSET_DATE = offset.OffsetDate,
            USER_ID = userId,
            TRANSACTION_ID = offset.TransactionId,
        };

        _context.FT_TRANSACTION_OFFSET.Add(entity);
        _context.SaveChanges();

        int id = entity.TRANSACTION_OFFSET_ID;
        return id;
    }

    public int UpdateTransactionOffset(int userId, int accountId, TransactionOffsetModel offset)
    {
        int result = 0;
        FT_TRANSACTION_OFFSET? entity = _context.FT_TRANSACTION_OFFSET.FirstOrDefault(m => m.TRANSACTION_OFFSET_ID == offset.TransactionOffsetId && m.ACCOUNT_ID == accountId && m.USER_ID == userId);

        if (entity != null)
        {
            entity.OFFSET_AMOUNT = offset.OffsetAmount;
            entity.OFFSET_DATE = offset.OffsetDate;

            result = _context.SaveChanges();
        }

        return result;
    }

    public bool DeleteTransactionOffset(int userId, int accountId, int offsetId)
    {
        bool result = false;

        FT_TRANSACTION_OFFSET? entity = _context.FT_TRANSACTION_OFFSET.FirstOrDefault(m => m.TRANSACTION_OFFSET_ID == offsetId && m.ACCOUNT_ID == accountId && m.USER_ID == userId);

        if (entity != null)
        {
            IEnumerable<FT_TRANSACTION_OFFSET> offsetEntities = _context.FT_TRANSACTION_OFFSET.Where(m => m.TRANSACTION_ID == offsetId);

            _context.FT_TRANSACTION_OFFSET.RemoveRange(offsetEntities);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }
}



