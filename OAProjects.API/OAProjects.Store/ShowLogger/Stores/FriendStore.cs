using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Friend;
using OAProjects.Models;
using OAProjects.Store.ShowLogger.Stores.Interfaces;

namespace OAProjects.Store.ShowLogger.Stores;
public class FriendStore : IFriendStore
{
    private readonly ShowLoggerDbContext _context;

    public FriendStore(ShowLoggerDbContext context)
    {
        _context = context;
    }

    public IEnumerable<FriendModel> GetFriends(int userId)
    {
        List<FriendModel> query = _context.SL_FRIEND.Where(m => m.USER_ID == userId || m.FRIEND_USER_ID == userId)
            .Select(m => new FriendModel
            {
                Id = m.FRIEND_ID,
                FriendUserId = userId != m.FRIEND_USER_ID ? m.FRIEND_USER_ID : m.USER_ID,
                CreatedDate = m.CREATED_DATE,
                IsPending = false
            }).ToList();

        List<FriendModel> friendRequests = _context.SL_FRIEND_REQUEST.Where(m => m.RECEIVED_USER_ID == userId)
            .Select(m => new FriendModel
            {
                Id = m.FRIEND_REQUEST_ID,
                FriendUserId = userId != m.RECEIVED_USER_ID ? m.RECEIVED_USER_ID : m.SENT_USER_ID,
                CreatedDate = m.DATE_SENT,
                IsPending = true
            }).ToList();

        query.AddRange(friendRequests);

        return query;
    }

    public bool SendFriendRequest(int userId, int friendId)
    {
        bool successful = false;
        SL_FRIEND? friendEntity = _context.SL_FRIEND.Where(m => (m.FRIEND_USER_ID == userId && m.USER_ID == friendId)
            || (m.FRIEND_USER_ID == friendId && m.USER_ID == userId)).FirstOrDefault();
        SL_FRIEND_REQUEST? entity = _context.SL_FRIEND_REQUEST.FirstOrDefault(m => m.SENT_USER_ID == userId && m.RECEIVED_USER_ID == friendId);
        SL_FRIEND_REQUEST? otherEntity = _context.SL_FRIEND_REQUEST.FirstOrDefault(m => m.SENT_USER_ID == friendId && m.RECEIVED_USER_ID == userId);

        if (friendEntity == null)
        {
            if (otherEntity != null)
            {
                successful = AcceptFriendRequest(userId, otherEntity.FRIEND_REQUEST_ID);
            }
            else if (entity == null)
            {
                entity = new SL_FRIEND_REQUEST
                {
                    SENT_USER_ID = userId,
                    RECEIVED_USER_ID = friendId,
                    DATE_SENT = DateTime.Now,
                };

                _context.SL_FRIEND_REQUEST.Add(entity);
                _context.SaveChanges();
            }
        }

        successful = true;

        return successful;
    }

    public bool AcceptFriendRequest(int userId, int friendRequestId)
    {
        bool successful = false;

        SL_FRIEND_REQUEST? entity = _context.SL_FRIEND_REQUEST.FirstOrDefault(m => m.RECEIVED_USER_ID == userId && m.FRIEND_REQUEST_ID == friendRequestId);

        if (entity != null)
        {
            _context.SL_FRIEND_REQUEST.Remove(entity);

            _context.SL_FRIEND.Add(new SL_FRIEND
            {
                USER_ID = entity.SENT_USER_ID,
                FRIEND_USER_ID = entity.RECEIVED_USER_ID,
                CREATED_DATE = DateTime.Now,
            });

            _context.SaveChanges();
            successful = true;
        }

        return successful;
    }

    public bool DenyFriendRequest(int userId, int friendRequestId)
    {
        bool successful = false;

        SL_FRIEND_REQUEST? entity = _context.SL_FRIEND_REQUEST.FirstOrDefault(m => m.RECEIVED_USER_ID == userId && m.FRIEND_REQUEST_ID == friendRequestId);

        if (entity != null)
        {
            _context.SL_FRIEND_REQUEST.Remove(entity);
            _context.SaveChanges();
            successful = true;
        }

        return successful;
    }

    public bool DeleteFriend(int userId, int friendId)
    {
        bool successful = false;

        SL_FRIEND? entity = _context.SL_FRIEND.Where(m => (m.USER_ID == userId || m.FRIEND_USER_ID == userId) && m.FRIEND_ID == friendId).FirstOrDefault();

        if (entity != null)
        {
            _context.SL_FRIEND.Remove(entity);
            _context.SaveChanges();
            successful = true;
        }

        return successful;
    }
}
