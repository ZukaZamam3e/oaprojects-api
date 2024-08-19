using OAProjects.Models.ShowLogger.Models.Login;
using OAProjects.Store.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface ILoginStore : IStore
{
    UserPrefModel GetUserPref(int userId);

    bool UpdateUserPref(int userId, UserPrefModel model);
}
