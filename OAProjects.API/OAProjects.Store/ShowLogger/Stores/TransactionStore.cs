using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Transaction;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.Store.ShowLogger.Stores;
public class TransactionStore : ITransactionStore
{
    private readonly ShowLoggerDbContext _context;

    public TransactionStore(ShowLoggerDbContext context)
    {
        _context = context;
    }

    public IEnumerable<TransactionModel> GetTransactions(int userId, Expression<Func<TransactionModel, bool>>? predicate = null)
    {
        Dictionary<int, string> transactionTypeIds = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.TRANSACTION_TYPE_ID).ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);
        Dictionary<int, string> showIds = _context.SL_SHOW.Where(m => m.USER_ID == userId).ToDictionary(m => m.SHOW_ID, m => m.SHOW_NAME);

        IEnumerable<TransactionModel> query = _context.SL_TRANSACTION
            .Select(m => new TransactionModel
            {
                TransactionId = m.TRANSACTION_ID,
                UserId = m.USER_ID,
                TransactionTypeId = m.TRANSACTION_TYPE_ID,
                TransactionTypeIdZ = transactionTypeIds[m.TRANSACTION_TYPE_ID],
                ShowId = m.SHOW_ID,
                ShowIdZ = m.SHOW_ID != null ? showIds[m.SHOW_ID.Value] : "",
                Item = m.ITEM,
                CostAmt = m.COST_AMT,
                Quantity = m.QUANTITY,
                TransactionNotes = m.TRANSACTION_NOTES,
                TransactionDate = m.TRANSACTION_DATE,
            });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public int CreateTransaction(int userId, TransactionModel model)
    {
        SL_TRANSACTION entity = new SL_TRANSACTION
        {
            TRANSACTION_TYPE_ID = model.TransactionTypeId,
            SHOW_ID = model.ShowId,
            ITEM = model.Item,
            COST_AMT = model.CostAmt,
            QUANTITY = model.Quantity,
            TRANSACTION_NOTES = model.TransactionNotes,
            TRANSACTION_DATE = model.TransactionDate,
            USER_ID = userId
        };

        _context.SL_TRANSACTION.Add(entity);
        _context.SaveChanges();
        int id = entity.TRANSACTION_ID;
        return id;
    }

    public int UpdateTransaction(int userId, TransactionModel model)
    {
        SL_TRANSACTION? entity = _context.SL_TRANSACTION.FirstOrDefault(m => m.TRANSACTION_ID == model.TransactionId && m.USER_ID == userId);

        if (entity != null)
        {
            entity.TRANSACTION_TYPE_ID = model.TransactionTypeId;
            entity.SHOW_ID = model.ShowId;
            entity.ITEM = model.Item;
            entity.COST_AMT = model.CostAmt;
            entity.QUANTITY = model.Quantity;
            entity.TRANSACTION_NOTES = model.TransactionNotes;
            entity.TRANSACTION_DATE = model.TransactionDate;

            return _context.SaveChanges();
        }
        return 0;
    }

    public bool DeleteTransaction(int userId, int transactionId)
    {
        bool result = false;
        SL_TRANSACTION? entity = _context.SL_TRANSACTION.FirstOrDefault(m => m.TRANSACTION_ID == transactionId && m.USER_ID == userId);

        if (entity != null)
        {
            _context.SL_TRANSACTION.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }
}
