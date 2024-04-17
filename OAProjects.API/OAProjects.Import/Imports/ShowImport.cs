using Microsoft.EntityFrameworkCore;
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

public interface IShowImport : IImport { };

public class ShowImport : IShowImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public ShowImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- Show Import Started -----------");
        Console.WriteLine("Importing SL_SHOW");

        string showJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_show));
        IEnumerable<ShowDataImport> showData = JsonConvert.DeserializeObject<IEnumerable<ShowDataImport>>(showJson);

        int showCount = showData.Count();
        Console.WriteLine($"Items to be imported: {showCount}");

        for (int i = 0; i < showCount; i += 100)
        {
            List<ShowDataImport> data = showData.Skip(i).Take(100).ToList();

            int[] oldTvEpisodeInfoIds = data.Where(m => m.SHOW_TYPE_ID == (int)CodeValueIds.TV && m.INFO_ID != null).Select(m => m.INFO_ID.Value).ToArray();
            int[] oldMovieInfoIds = data.Where(m => m.SHOW_TYPE_ID != (int)CodeValueIds.TV && m.INFO_ID != null).Select(m => m.INFO_ID.Value).ToArray();
            int[] oldUserIds = data.Select(m => m.USER_ID).ToArray();

            Dictionary<int, int> dictTvEpisodeInfoIds = _showLoggerContext.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_TV_EPISODE_INFO && oldTvEpisodeInfoIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);
            Dictionary<int, int> dictMovieInfoIds = _showLoggerContext.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_MOVIE_INFO && oldMovieInfoIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);
            Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_SHOW
                {
                    USER_ID = dictUserIds[m.USER_ID],
                    SHOW_NAME = m.SHOW_NAME,
                    SHOW_TYPE_ID = m.SHOW_TYPE_ID,
                    SEASON_NUMBER = m.SEASON_NUMBER,
                    EPISODE_NUMBER = m.EPISODE_NUMBER,
                    DATE_WATCHED = m.DATE_WATCHED,
                    SHOW_NOTES = m.SHOW_NOTES,
                    RESTART_BINGE = m.RESTART_BINGE,
                    INFO_ID = m.INFO_ID != null ? (m.SHOW_TYPE_ID == (int)CodeValueIds.TV ? dictTvEpisodeInfoIds[m.INFO_ID.Value] : dictMovieInfoIds[m.INFO_ID.Value]) : null,
                };
            });

            _showLoggerContext.SL_SHOW.AddRange(data.Select(m => m.ENTITY));

            _showLoggerContext.SaveChanges();

            _showLoggerContext.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.SHOW_ID,
                NEW_ID = m.ENTITY.SHOW_ID,
                TABLE_ID = (int)TableIds.SL_SHOW
            }));

            _showLoggerContext.SaveChanges();
        }

        int showImportCount = _showLoggerContext.SL_SHOW.Count();

        Console.WriteLine($"Items that were imported: {showImportCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class ShowDataImport
{
    public int SHOW_ID { get; set; }
    public int USER_ID { get; set; }
    public string SHOW_NAME { get; set; }
    public int SHOW_TYPE_ID { get; set; }
    public int? SEASON_NUMBER { get; set; }
    public int? EPISODE_NUMBER { get; set; }
    public DateTime DATE_WATCHED { get; set; }
    public string SHOW_NOTES { get; set; }
    public bool RESTART_BINGE { get; set; }
    public int? INFO_ID { get; set; }
    public SL_SHOW ENTITY { get; set; }
}
