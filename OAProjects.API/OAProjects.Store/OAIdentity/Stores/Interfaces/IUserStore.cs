using OAProjects.Models.OAIdentity;

namespace OAProjects.Store.OAIdentity.Stores.Interfaces;
public interface IUserStore
{
    UserModel? GetUser(int? userId, string? userGuid);

    UserModel AddUser(string userGuid, string userName, string loginType);
}
