using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;

namespace OAProjects.Store.FinanceTracker.Stores;
public class FTAccountStore(FinanceTrackerDbContext _context) : IFTAccountStore
{
    public IEnumerable<AccountModel> GetAccounts(int userId)
    {
        IEnumerable<AccountModel> query = _context.FT_ACCOUNT.Where(m => m.USER_ID == userId).Select(m => new AccountModel
        {
            AccountId = m.ACCOUNT_ID,
            AccountName = m.ACCOUNT_NAME,
            DefaultIndc = m.DEFAULT_INDC
        });

        return query;
    }

    public int CreateAccount(int userId, AccountModel account)
    {
        FT_ACCOUNT entity = new FT_ACCOUNT
        {
            USER_ID = userId,
            ACCOUNT_NAME = account.AccountName,
            DEFAULT_INDC = account.DefaultIndc
        };

        _context.FT_ACCOUNT.Add(entity);
        _context.SaveChanges();
        int id = entity.ACCOUNT_ID;

        return id;
    }

    public int UpdateAccount(int userId, AccountModel account)
    {
        int result = 0;
        FT_ACCOUNT? entity = _context.FT_ACCOUNT.FirstOrDefault(m => m.ACCOUNT_ID == account.AccountId);

        if (entity != null)
        {
            entity.ACCOUNT_NAME = account.AccountName;

            if (!entity.DEFAULT_INDC && account.DefaultIndc)
            {
                FT_ACCOUNT? defaultRecord = _context.FT_ACCOUNT.FirstOrDefault(m => m.USER_ID == userId && m.DEFAULT_INDC);

                if (defaultRecord != null)
                {
                    defaultRecord.DEFAULT_INDC = false;
                }
            }

            entity.DEFAULT_INDC = account.DefaultIndc;

            result = _context.SaveChanges();
        }

        return result;
    }

    public bool DeleteAccount(int userId, int accountId)
    {
        bool result = false;
        FT_ACCOUNT? entity = _context.FT_ACCOUNT.FirstOrDefault(m => m.ACCOUNT_ID == accountId);

        if (entity != null)
        {
            IEnumerable<FT_TRANSACTION> transactionEntities = _context.FT_TRANSACTION.Where(m => m.ACCOUNT_ID == accountId);

            _context.FT_TRANSACTION.RemoveRange(transactionEntities);
            _context.FT_ACCOUNT.Remove(entity);

            _context.SaveChanges();

            result = true;
        }

        return result;
    }

    public int CloneAccount(int userId, int accountId)
    {
        int result = -1;
        FT_ACCOUNT? entity = _context.FT_ACCOUNT.FirstOrDefault(m => m.ACCOUNT_ID == accountId);

        if (entity != null)
        {
            FT_ACCOUNT clonedEntity = new FT_ACCOUNT
            {
                USER_ID = userId,
                ACCOUNT_NAME = $"Copy of {entity.ACCOUNT_NAME}",
                DEFAULT_INDC = false
            };

            _context.FT_ACCOUNT.Add(clonedEntity);
            _context.SaveChanges();

            result = clonedEntity.ACCOUNT_ID;

            IEnumerable<FT_TRANSACTION> transactionEntities = _context.FT_TRANSACTION.Where(m => m.ACCOUNT_ID == accountId);

            IEnumerable<Tuple<FT_TRANSACTION, FT_TRANSACTION>> clonedTransactionEntities = transactionEntities.Select(m =>
                new Tuple<FT_TRANSACTION, FT_TRANSACTION>(m, new FT_TRANSACTION
                {
                    ACCOUNT_ID = result,
                    TRANSACTION_NAME = m.TRANSACTION_NAME,
                    FREQUENCY_TYPE_ID = m.FREQUENCY_TYPE_ID,
                    TRANSACTION_AMOUNT = m.TRANSACTION_AMOUNT,
                    START_DATE = m.START_DATE,
                    END_DATE = m.END_DATE,
                    TRANSACTION_NOTES = m.TRANSACTION_NOTES,
                    TRANSACTION_URL = m.TRANSACTION_URL
                }));

            _context.FT_TRANSACTION.AddRange(clonedTransactionEntities.Select(m => m.Item2));

            _context.SaveChanges();

            Dictionary<int, int> transactionIdLookUp = clonedTransactionEntities.ToDictionary(m => m.Item1.TRANSACTION_ID, m => m.Item2.TRANSACTION_ID);

            int[] oldTransactionIds = transactionEntities.Select(m => m.TRANSACTION_ID).ToArray();

            IEnumerable<FT_TRANSACTION_OFFSET> transactionOffsetEntities = _context.FT_TRANSACTION_OFFSET.Where(m => oldTransactionIds.Contains(m.TRANSACTION_ID)).Select(m => new FT_TRANSACTION_OFFSET
            {
                TRANSACTION_ID = transactionIdLookUp[m.TRANSACTION_ID],
                OFFSET_AMOUNT = m.OFFSET_AMOUNT,
                OFFSET_DATE = m.OFFSET_DATE,
            });

            _context.SaveChanges();
        }

        return result;
    }
}
