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
public interface IWatchListImport : IImport { };
internal class WatchListImport : IWatchListImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public WatchListImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- Watchlist Import Started -----------");
        Console.WriteLine("Importing SL_WATCHLIST");

        string json = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_watchlist));
        IEnumerable<WatchListImportData> watchlistData = JsonConvert.DeserializeObject<IEnumerable<WatchListImportData>>(json);

        int count = watchlistData.Count();
        Console.WriteLine($"Items to be imported: {count}");

        for (int i = 0; i < count; i += 100)
        {
            List<WatchListImportData> data = watchlistData.Skip(i).Take(100).ToList();

            int[] oldUserIds = data.Select(m => m.USER_ID).ToArray();

            Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_WATCHLIST
                {
                    USER_ID = dictUserIds[m.USER_ID],
                    SHOW_NAME = m.SHOW_NAME,
                    SHOW_TYPE_ID = m.SHOW_TYPE_ID,
                    SEASON_NUMBER = m.SEASON_NUMBER,
                    EPISODE_NUMBER = m.EPISODE_NUMBER,
                    DATE_ADDED = m.DATE_ADDED,
                    SHOW_NOTES = m.SHOW_NOTES,
                };
            });

            _showLoggerContext.SL_WATCHLIST.AddRange(data.Select(m => m.ENTITY));

            _showLoggerContext.SaveChanges();

            _showLoggerContext.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.WATCHLIST_ID,
                NEW_ID = m.ENTITY.WATCHLIST_ID,
                TABLE_ID = (int)TableIds.SL_WATCHLIST
            }));

            _showLoggerContext.SaveChanges();
        }

        int importCount = _showLoggerContext.SL_WATCHLIST.Count();

        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class WatchListImportData
{
    public int WATCHLIST_ID { get; set; }
    public int USER_ID { get; set; }
    public string SHOW_NAME { get; set; }
    public int SHOW_TYPE_ID { get; set; }
    public int? SEASON_NUMBER { get; set; }
    public int? EPISODE_NUMBER { get; set; }
    public DateTime DATE_ADDED { get; set; }
    public string SHOW_NOTES { get; set; }
    public SL_WATCHLIST ENTITY { get; set; }
}