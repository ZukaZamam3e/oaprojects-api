using OAProjects.Models.CatanLogger.Models;
using System.Linq.Expressions;

namespace OAProjects.Store.CatanLogger.Stores.Interfaces;
public interface IGroupStore
{
    IEnumerable<GroupModel> GetGroups(Expression<Func<GroupModel, bool>>? predicate);

    int CreateGroup(int userId, GroupModel group);

    int UpdateGroup(int userId, GroupModel group);

    void AddUserToGroup(int groupId, int userId);

    void ConfirmUserToGroup(int groupId, int userId, int confirmedUserId);
}
