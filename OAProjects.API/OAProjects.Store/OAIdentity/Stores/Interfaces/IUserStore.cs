using OAProjects.Models.OAIdentity;

namespace OAProjects.Store.OAIdentity.Stores.Interfaces;
public interface IUserStore
{
    UserModel? GetUser(int? userId);

    UserModel AddUser(UserModel model);

    void AddToken(UserTokenModel model);

    UserModel? GetUserByToken(string token);

    UserModel? GetUserByEmail(string email);

    Dictionary<int, string> GetUserNameLookUps();

    Dictionary<int, UserModel> GetUserLookUps();

}
