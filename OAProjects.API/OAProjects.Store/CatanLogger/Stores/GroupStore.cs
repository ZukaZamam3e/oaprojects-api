using Microsoft.EntityFrameworkCore;
using OAProjects.Data.CatanLogger.Context;
using OAProjects.Data.CatanLogger.Entities;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Models.CatanLogger;
using OAProjects.Models.CatanLogger.Models;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Store.CatanLogger.Stores.Interfaces;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace OAProjects.Store.CatanLogger.Stores;
public class GroupStore(CatanLoggerDbContext _context) : IGroupStore
{
    public IEnumerable<GroupModel> GetGroups(Expression<Func<GroupModel, bool>>? predicate)
    {
        IEnumerable<GroupModel> query = _context.CL_GROUP_USER
            .Include(m => m.GROUP)
            .ThenInclude(m => m.GROUP_USERS)
            .AsEnumerable()
            .Select(m => new GroupModel
            {
                GroupId = m.GROUP_ID,
                GroupName = m.GROUP.GROUP_NAME,
                DateAdded = m.GROUP.DATE_ADDED,
                Users = m.GROUP.GROUP_USERS.Select(g => new UserGroupModel
                {
                    GroupId = g.GROUP_ID,
                    GroupUserId = g.GROUP_USER_ID,
                    UserId = g.USER_ID,
                    RoleId = g.ROLE_ID,
                })
            });

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate);
        }

        return query;
    }

    public int CreateGroup(int userId, GroupModel group)
    {
        CL_GROUP entity = new CL_GROUP
        {
            GROUP_NAME = group.GroupName,
            DATE_ADDED = DateTime.Now
        };

        _context.CL_GROUP.Add(entity);
        _context.SaveChanges();

        CL_GROUP_USER guEntity = new CL_GROUP_USER
        {
            GROUP_ID = entity.GROUP_ID,
            USER_ID = userId,
            CONFIRMED_USER_ID = userId,
            GROUP_USER_STATUS = (int)CL_Status.APPROVED,
            ROLE_ID = (int)CL_RoleIds.ADMIN,
            DATE_ADDED = DateTime.Now,
        };

        _context.CL_GROUP_USER.Add(guEntity);

        _context.SaveChanges();
        int id = entity.GROUP_ID;

        return id;
    }

    public int UpdateGroup(int userId, GroupModel group)
    {
        int result = 0;
        CL_GROUP? entity = _context.CL_GROUP.FirstOrDefault(m => m.GROUP_ID == group.GroupId);

        bool canEdit = CanEdit(group.GroupId, userId);

        if (entity != null && canEdit)
        {
            entity.GROUP_NAME = group.GroupName;

            result = _context.SaveChanges();
        }

        return result;
    }

    public void AddUserToGroup(int groupId, int userId)
    {
        if (_context.CL_GROUP_USER.Any(m => m.GROUP_ID == groupId && m.USER_ID == userId))
        {
            return;
        }

        CL_GROUP_USER entity = new CL_GROUP_USER
        {
            USER_ID = userId,
            GROUP_USER_STATUS = (int)CL_Status.PENDING,
            DATE_ADDED = DateTime.Now,
            ROLE_ID = (int) CL_RoleIds.READ,
        };

        _context.CL_GROUP_USER.Add(entity);

        _context.SaveChanges();
    }

    public void ConfirmUserToGroup(int groupId, int userId, int confirmedUserId)
    {
        CL_GROUP_USER? entity = _context.CL_GROUP_USER.FirstOrDefault(m => m.GROUP_ID == groupId && m.USER_ID == userId);

        if (entity == null
            || entity.GROUP_USER_STATUS == (int)CL_Status.APPROVED)
        {
            return;
        }

        entity.GROUP_USER_STATUS = (int)CL_Status.APPROVED;
        entity.CONFIRMED_USER_ID = confirmedUserId;

        _context.SaveChanges();
    }

    private bool CanEdit(int groupId, int userId)
    {
        int[] editableRoles = [(int)CL_RoleIds.EDIT, (int)CL_RoleIds.ADMIN];
        bool canEdit = _context.CL_GROUP_USER.Any(m => m.GROUP_ID == groupId && m.USER_ID == userId && editableRoles.Contains(m.ROLE_ID));
        return canEdit;
    }
}
