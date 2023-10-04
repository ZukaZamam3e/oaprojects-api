using OAProjects.Models.ShowLogger;
using OAProjects.Store.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;

internal interface IShowStore : IStore
{
    IEnumerable<SLCodeValueModel> GetCodeValues(Expression<Func<SLCodeValueModel, bool>>? predicate = null);

    IEnumerable<ShowModel> GetShows(Expression<Func<ShowModel, bool>>? predicate = null);

    long CreateShow(int userId, ShowModel model);

    long UpdateShow(int userId, ShowModel model);
}
