using Microsoft.EntityFrameworkCore;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Import.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Import.Imports;

public interface IRestartImport : IImport { };

public class RestartImport : IRestartImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public RestartImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- Restart Import Started -----------");
        Console.WriteLine("Clearing OA Identity data.");

        string sql = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, "oa_clear_tables.sql"));
        _oaIdentityContext.Database.ExecuteSqlRaw(sql);
        _oaIdentityContext.SaveChanges();

        Console.WriteLine("Clearing Show Logger data.");

        sql = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, "sl_clear_tables.sql"));
        _showLoggerContext.Database.ExecuteSqlRaw(sql);
        _showLoggerContext.SaveChanges();

        Console.WriteLine("----------- Restart Import Finished -----------");

    }
}
