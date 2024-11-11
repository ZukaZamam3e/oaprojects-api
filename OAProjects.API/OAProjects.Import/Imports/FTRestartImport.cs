using Microsoft.EntityFrameworkCore;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Import.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Import.Imports;

public interface IFTRestartImport : IImport { };

public class FTRestartImport
(
    FinanceTrackerDbContext _financeTrackerDbContext,
    DataConfig _dataConfig
) : IFTRestartImport
{

    public void RunImport()
    {
        Console.WriteLine("----------- Restart Import Started -----------");

        Console.WriteLine("Clearing Finance Tracker data.");

        string sql = File.ReadAllText(Path.Join(_dataConfig.FinanceDataFolderPath, "ft_clear_tables.sql"));
        _financeTrackerDbContext.Database.ExecuteSqlRaw(sql);
        _financeTrackerDbContext.SaveChanges();

        Console.WriteLine("----------- Restart Import Finished -----------");

    }
}
