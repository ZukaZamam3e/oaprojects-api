using OAProjects.Models.FinanceTracker.Models;
using System.Linq.Expressions;

namespace OAProjects.Store.FinanceTracker.Stores.Interfaces;
public interface IFTCodeValueStore
{
    IEnumerable<FTCodeValueModel> GetCodeValues(Expression<Func<FTCodeValueModel, bool>>? predicate = null);
}
