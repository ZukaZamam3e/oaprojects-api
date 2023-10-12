using OAProjects.Models.ShowLogger;
using OAProjects.Models.ShowLogger.Models;
using OAProjects.Store.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;

public interface IShowStore : IStore
{
    IEnumerable<SLCodeValueModel> GetCodeValues(Expression<Func<SLCodeValueModel, bool>>? predicate = null);

    IEnumerable<ShowModel> GetShows(Expression<Func<ShowModel, bool>>? predicate = null);

    IEnumerable<ShowModel> SearchShows(int userId, string text);

    int CreateShow(int userId, ShowModel model);

    int UpdateShow(int userId, ShowModel model);
}
