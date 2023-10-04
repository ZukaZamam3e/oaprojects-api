using OAProjects.Data.OAIdentity.Context;
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
        return _context.OA_USER.Where(m =>
            (userId != null && m.USER_ID == userId)
            || (userGuid != null && m.USER_GUID == userGuid))
            .Select(m => new UserModel
            {
                UserId = m.USER_ID,
                UserName = m.USER_NAME,
            }).FirstOrDefault();
    }

    public UserModel AddUser(string userGuid, string userName, string loginType)
    {
        throw new NotImplementedException();
    }

    
}
