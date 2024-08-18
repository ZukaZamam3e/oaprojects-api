using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Login;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores;
public class LoginStore : ILoginStore
{
    private readonly ShowLoggerDbContext _context;

    public LoginStore(ShowLoggerDbContext context)
    {
        _context = context;
    }

    public UserPrefModel GetUserPref(int userId)
    {
        UserPrefModel model = new UserPrefModel();

        SL_USER_PREF? entity = _context.SL_USER_PREF.FirstOrDefault(m => m.USER_ID == userId);

        if (entity != null)
        {
            model.DefaultArea = entity.DEFAULT_AREA;
        }

        return model;
    }

    public bool UpdateUserPref(int userId, UserPrefModel model)
    {
        bool successful = false;

        SL_USER_PREF? entity = _context.SL_USER_PREF.FirstOrDefault(m => m.USER_ID == userId);

        if (entity == null)
        {
            entity = new SL_USER_PREF
            {
                USER_ID = userId,
            };
            _context.SL_USER_PREF.Add(entity);
        }

        entity.DEFAULT_AREA = model.DefaultArea;

        _context.SaveChanges();

        successful = true;

        return successful;
    }
}
