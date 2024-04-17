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

public interface IUserPrefImport : IImport { };

internal class UserPrefImport : IUserPrefImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public UserPrefImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- User Pref Import Started -----------");
        Console.WriteLine("Importing SL_USER_PREF");

        string json = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_user_pref));
        IEnumerable<UserPrefImportData> userPrefData = JsonConvert.DeserializeObject<IEnumerable<UserPrefImportData>>(json);

        int count = userPrefData.Count();
        Console.WriteLine($"Items to be imported: {count}");

        for (int i = 0; i < count; i += 100)
        {
            List<UserPrefImportData> data = userPrefData.Skip(i).Take(100).ToList();

            int[] oldUserIds = data.Select(m => m.USER_ID).ToArray();

            Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_USER_PREF
                {
                    USER_ID = dictUserIds[m.USER_ID],
                    DEFAULT_AREA = m.DEFAULT_AREA,
                };
            });

            _showLoggerContext.SL_USER_PREF.AddRange(data.Select(m => m.ENTITY));

            _showLoggerContext.SaveChanges();

            _showLoggerContext.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.USER_PREF_ID,
                NEW_ID = m.ENTITY.USER_PREF_ID,
                TABLE_ID = (int)TableIds.SL_USER_PREF
            }));

            _showLoggerContext.SaveChanges();
        }

        int importCount = _showLoggerContext.SL_USER_PREF.Count();

        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class UserPrefImportData
{
    public int USER_PREF_ID { get; set; }
    public int USER_ID { get; set; }
    public string DEFAULT_AREA { get; set; }
    public SL_USER_PREF ENTITY { get; set; }
}
