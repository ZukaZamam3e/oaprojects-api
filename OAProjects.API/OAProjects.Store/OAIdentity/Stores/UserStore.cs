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

    public UserModel? GetUser(int? userId)
    {
        UserModel? user = _context.OA_USER.Where(m =>
            (userId != null && m.USER_ID == userId))
            .Select(m => new UserModel
            {
                UserId = m.USER_ID,
                UserName = m.USER_NAME,
            }).FirstOrDefault();


        return user;
    }

    public UserModel? GetUserByEmail(string email)
    {
        UserModel? user = _context.OA_USER.Where(m => m.EMAIL.ToLower() == email.ToLower())
            .Select(m => new UserModel
            {
                UserId = m.USER_ID,
                UserName = m.USER_NAME,
            }).FirstOrDefault();

        return user;
    }

    public UserModel? GetUserByToken(string token)
    {
        OA_USER_TOKEN? entity = _context.OA_USER_TOKEN.FirstOrDefault(m => m.TOKEN == token);

        if(entity == null)
        {
            return null;
        }
        else
        {
            return GetUser(entity.USER_ID);
        }
    }

    public void AddToken(UserTokenModel model)
    {
        OA_USER_TOKEN entity = new OA_USER_TOKEN
        {
            USER_ID = model.UserId,
            TOKEN = model.Token,
            EXPIRY_TIME = model.ExpiryTime,
            ISSUED_AT = model.IssuedAt,
            EXPIRY_DATE_UTC = model.ExpiryDateUtc,
            ISSUED_AT_DATE_UTC = model.IssuedAtDateUtc
        };

        _context.OA_USER_TOKEN.Add(entity);

        _context.SaveChanges();
    }

    public UserModel AddUser(UserModel model)
    {
        OA_USER entity = new OA_USER
        {
            USER_NAME = model.UserName,
            EMAIL = model.Email,
            FIRST_NAME = model.FirstName,
            LAST_NAME = model.LastName,
            DATE_ADDED = DateTime.Now,
        };

        _context.OA_USER.Add(entity);

        _context.SaveChanges();

        model.UserId = entity.USER_ID;

        return model;
    }

    public Dictionary<int, string> GetUserLookUps()
    {
        Dictionary<int, string> userLookUp = _context.OA_USER.ToDictionary(m => m.USER_ID, m => $"{m.LAST_NAME}, {m.FIRST_NAME}");

        return userLookUp;
    }
}
