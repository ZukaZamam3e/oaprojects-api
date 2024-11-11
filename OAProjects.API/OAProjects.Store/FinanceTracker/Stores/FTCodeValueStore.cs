using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Store.FinanceTracker.Stores.Interfaces;
using System.Linq;
using System.Linq.Expressions;

namespace OAProjects.Store.FinanceTracker.Stores;
public class FTCodeValueStore(FinanceTrackerDbContext _context) : IFTCodeValueStore
{
    public IEnumerable<FTCodeValueModel> GetCodeValues(Expression<Func<FTCodeValueModel, bool>>? predicate = null)
    {
        IEnumerable<FTCodeValueModel> query = _context.FT_CODE_VALUE.Select(m => new FTCodeValueModel
        {
            CodeTableId = m.CODE_TABLE_ID,
            CodeValueId = m.CODE_VALUE_ID,
            DecodeTxt = m.DECODE_TXT
        });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }
}
