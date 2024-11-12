using System.Linq.Expressions;

namespace OAProjects.Store.CatanLogger.Stores.Interfaces;
public interface IGroupStore
{
    IEnumerable<GroupModel> GetGroups(Expression<Func<GroupModel, bool>>? predicate);

    GroupModel CreateGroup(string groupName, int userId);

    void AddUserToGroup(int groupId, int userId);

    void ConfirmUserToGroup(int groupId, int userId, int confirmedUserId);
}
