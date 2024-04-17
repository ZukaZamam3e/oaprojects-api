using Newtonsoft.Json;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.OAIdentity.Entities;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Import.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Import.Imports;

public interface IFriendImport : IImport { };

internal class FriendImport : IFriendImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public FriendImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- Friend Import Started -----------");
        Console.WriteLine("Importing SL_FRIEND");

        string friendJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_friend));
        IEnumerable<FriendImportData> friendData = JsonConvert.DeserializeObject<IEnumerable<FriendImportData>>(friendJson);

        int friendCount = friendData.Count();
        Console.WriteLine($"Items to be imported: {friendCount}");

        for (int i = 0; i < friendCount; i += 100)
        {
            List<FriendImportData> data = friendData.Skip(i).Take(100).ToList();

            int[] oldUserIds = data.Select(m => m.USER_ID).ToArray();

            Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_FRIEND
                {
                    USER_ID = dictUserIds[m.USER_ID],
                    FRIEND_USER_ID = m.FRIEND_USER_ID,
                    CREATED_DATE = m.CREATED_DATE,
                };
            });

            _showLoggerContext.SL_FRIEND.AddRange(data.Select(m => m.ENTITY));

            _showLoggerContext.SaveChanges();

            _showLoggerContext.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.FRIEND_ID,
                NEW_ID = m.ENTITY.FRIEND_ID,
                TABLE_ID = (int)TableIds.SL_FRIEND
            }));

            _showLoggerContext.SaveChanges();
        }

        int friendImportCount = _showLoggerContext.SL_FRIEND.Count();

        Console.WriteLine($"Items that were imported: {friendImportCount}");
        Console.WriteLine("----------------------------------------------");


        Console.WriteLine("----------- Friend Import Started -----------");
        Console.WriteLine("Importing SL_FRIEND");

        string friendRequestJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_friend_request));
        IEnumerable<FriendRequestImportData> friendRequestData = JsonConvert.DeserializeObject<IEnumerable<FriendRequestImportData>>(friendRequestJson);

        int friendRequestCount = friendRequestData.Count();
        Console.WriteLine($"Items to be imported: {friendRequestCount}");

        if (friendRequestCount > 0)
        {

            for (int i = 0; i < friendRequestCount; i += 100)
            {
                List<FriendRequestImportData> data = friendRequestData.Skip(i).Take(100).ToList();

                int[] oldSentUserIds = data.Select(m => m.SENT_USER_ID).ToArray();
                int[] oldReceivedUserIds = data.Select(m => m.RECEIVED_USER_ID).ToArray();

                Dictionary<int, int> dictSentUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldSentUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);
                Dictionary<int, int> dictReceivedUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldReceivedUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

                data.ForEach(m =>
                {
                    m.ENTITY = new SL_FRIEND_REQUEST
                    {
                        SENT_USER_ID = dictSentUserIds[m.SENT_USER_ID],
                        RECEIVED_USER_ID = dictReceivedUserIds[m.RECEIVED_USER_ID],
                        DATE_SENT = m.DATE_SENT,
                    };
                });

                _showLoggerContext.SL_FRIEND_REQUEST.AddRange(data.Select(m => m.ENTITY));

                _showLoggerContext.SaveChanges();

                _showLoggerContext.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
                {
                    OLD_ID = m.FRIEND_REQUEST_ID,
                    NEW_ID = m.ENTITY.FRIEND_REQUEST_ID,
                    TABLE_ID = (int)TableIds.SL_FRIEND_REQUEST
                }));

                _showLoggerContext.SaveChanges();
            }

            
        }

        int friendRequestImportCount = _showLoggerContext.SL_FRIEND_REQUEST.Count();

        Console.WriteLine($"Items that were imported: {friendRequestImportCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class FriendImportData
{
    public int FRIEND_ID { get; set; }
    public int USER_ID { get; set; }
    public int FRIEND_USER_ID { get; set; }
    public DateTime CREATED_DATE { get; set; }
    public SL_FRIEND ENTITY { get; set; }

}

public class FriendRequestImportData
{
    public int FRIEND_REQUEST_ID { get; set; }
    public int SENT_USER_ID { get; set; }
    public int RECEIVED_USER_ID { get; set; }
    public DateTime DATE_SENT { get; set; }
    public SL_FRIEND_REQUEST ENTITY { get; set; }
}
