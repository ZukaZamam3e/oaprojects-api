using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.OAIdentity.Entities;
using OAProjects.Models.OAIdentity;
using OAProjects.Store.OAIdentity.Stores.Interfaces;

namespace OAProjects.Store.OAIdentity.Stores;
public class UserStore : IUserStore
{
    private readonly OAIdentityDbContext _context;

    public UserStore(OAIdentityDbContext context)
    {
        _context = context;
    }

    public UserModel? GetUser(int? userId, string? userGuid)
    {
        UserModel? user = _context.OA_USER.Where(m =>
            (userId != null && m.USER_ID == userId)
            || (userGuid != null && m.USER_GUID == userGuid))
            .Select(m => new UserModel
            {
                UserId = m.USER_ID,
                UserName = m.USER_NAME,
            }).FirstOrDefault();


        return user;
    }

    public UserModel AddUser(string userGuid, string userName, string loginType)
    {
        OA_USER entity = new OA_USER
        {
            USER_GUID = userGuid,
            USER_NAME = userName,
            USER_LOGIN_TYPE = loginType,
            DATE_ADDED = DateTime.Now,
        };

        _context.OA_USER.Add(entity);

        _context.SaveChanges();

        return new UserModel
        {
            UserId = entity.USER_ID,
            UserName = userName,
        };
    }

    
}
