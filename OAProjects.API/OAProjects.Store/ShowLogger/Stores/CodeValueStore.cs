using OAProjects.Data.ShowLogger.Context;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores;
public class CodeValueStore : ICodeValueStore
{
    private readonly ShowLoggerDbContext _context;

    public CodeValueStore(ShowLoggerDbContext context)
    {
        _context = context;
    }

    public IEnumerable<SLCodeValueModel> GetCodeValues(Expression<Func<SLCodeValueModel, bool>>? predicate = null)
    {
        IEnumerable<SLCodeValueModel> query = _context.SL_CODE_VALUE.Select(m => new SLCodeValueModel
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
