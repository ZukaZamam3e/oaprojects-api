using OAProjects.Models.ShowLogger.Models.Transaction;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface ITransactionStore
{
    IEnumerable<TransactionModel> GetTransactions(int userId, Expression<Func<TransactionModel, bool>>? predicate = null);
    int CreateTransaction(int userId, TransactionModel model);
    int UpdateTransaction(int userId, TransactionModel model);
    bool DeleteTransaction(int userId, int transactionId);
}
