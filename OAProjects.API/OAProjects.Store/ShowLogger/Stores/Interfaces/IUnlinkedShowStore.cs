using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IUnlinkedShowStore
{
    IEnumerable<UnlinkedShowModel> GetUnlinkedShows(Expression<Func<UnlinkedShowModel, bool>>? predicate = null);
    bool UpdateShowNames(UpdateUnlinkedNameModel model);

    bool LinkShows(LinkShowModel model);
}
