using Newtonsoft.Json;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.OAIdentity.Entities;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Import.Config;

namespace OAProjects.Import.Imports;

public interface IFTAccountImport : IImport { };

internal class FTAccountImport(
    FinanceTrackerDbContext _financeTrackerContext,
    OAIdentityDbContext _oaIdentityContext,
    DataConfig _dataConfig) : IFTAccountImport
{
    public void RunImport()
    {
        Console.WriteLine("----------- Account Import Started -----------");
        Console.WriteLine("Importing FT_ACCOUNT");

        

        string json = File.ReadAllText(Path.Join(_dataConfig.FinanceDataFolderPath, ImportFiles.ft_account));
        IEnumerable<AccountDataImport> data = JsonConvert.DeserializeObject<IEnumerable<AccountDataImport>>(json);

        int[] oldUserIds = data.Select(m => m.USER_ID).ToArray();
        Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

        int count = data.Count();
        Console.WriteLine($"Items to be imported: {count}");

        for (int i = 0; i < count; i += 100)
        {
            List<AccountDataImport> iterData = data.Skip(i).Take(100).ToList();

            iterData.ForEach(m =>
            {
                m.ENTITY = new FT_ACCOUNT
                {
                    USER_ID = dictUserIds[m.USER_ID],
                    ACCOUNT_NAME = m.ACCOUNT_NAME,
                    DEFAULT_INDC = m.IS_DEFAULT,
                };
            });

            _financeTrackerContext.FT_ACCOUNT.AddRange(data.Select(m => m.ENTITY));

            _financeTrackerContext.SaveChanges();

            _financeTrackerContext.FT_ID_XREF.AddRange(data.Select(m => new FT_ID_XREF
            {
                OLD_ID = m.ACCOUNT_ID,
                NEW_ID = m.ENTITY.ACCOUNT_ID,
                TABLE_ID = (int)FTTableIds.FT_ACCOUNT
            }));

            _financeTrackerContext.SaveChanges();
        }

        int importCount = _financeTrackerContext.FT_ACCOUNT.Count();


        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class AccountDataImport
{
    public int ACCOUNT_ID { get; set; }
    public int USER_ID { get; set; }
    public string ACCOUNT_NAME { get; set; }
    public bool IS_DEFAULT { get; set; }
    public FT_ACCOUNT ENTITY { get; set; }

}
