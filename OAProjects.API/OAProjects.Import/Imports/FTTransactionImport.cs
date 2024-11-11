using Newtonsoft.Json;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.OAIdentity.Entities;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Import.Config;

namespace OAProjects.Import.Imports;

public interface IFTTransactionImport : IImport { };

internal class FTTransactionImport(
    FinanceTrackerDbContext _financeTrackerContext,
    OAIdentityDbContext _oaIdentityContext,
    DataConfig _dataConfig) : IFTTransactionImport
{
    public void RunImport()
    {
        Console.WriteLine("----------- Account Import Started -----------");
        Console.WriteLine("Importing FT_ACCOUNT");

        string json = File.ReadAllText(Path.Join(_dataConfig.FinanceDataFolderPath, ImportFiles.ft_transaction));
        IEnumerable<FTTransactionDataImport> data = JsonConvert.DeserializeObject<IEnumerable<FTTransactionDataImport>>(json);

        int[] oldAccountIds = data.Select(m => m.ACCOUNT_ID).Distinct().ToArray();

        Dictionary<int, int> dictAccountIds = _financeTrackerContext.FT_ID_XREF.Where(m => m.TABLE_ID == (int)FTTableIds.FT_ACCOUNT && oldAccountIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);
        Dictionary<int, int> dictRevAccountIds = _financeTrackerContext.FT_ID_XREF.Where(m => m.TABLE_ID == (int)FTTableIds.FT_ACCOUNT && oldAccountIds.Contains(m.OLD_ID)).ToDictionary(m => m.NEW_ID, m => m.OLD_ID);
        Dictionary<int, int> dictUserIds = _financeTrackerContext.FT_ACCOUNT.Where(m => m.USER_ID != 1009).ToDictionary(m => dictRevAccountIds[m.ACCOUNT_ID], m => m.USER_ID);

        int count = data.Count();
        Console.WriteLine($"Items to be imported: {count}");

        for (int i = 0; i < count; i += 100)
        {
            List<FTTransactionDataImport> iterData = data.Skip(i).Take(100).ToList();

            iterData.ForEach(m =>
            {
                m.ENTITY = new FT_TRANSACTION
                {
                    ACCOUNT_ID = dictAccountIds[m.ACCOUNT_ID],
                    USER_ID = dictUserIds[m.ACCOUNT_ID],
                    TRANSACTION_NAME = m.TRANSACTION_NAME,
                    START_DATE = m.START_DATE,
                    END_DATE = m.END_DATE,
                    TRANSACTION_AMOUNT = m.TRANSACTION_AMOUNT,
                    FREQUENCY_TYPE_ID = m.FREQUENCY_TYPE_ID,
                    TRANSACTION_URL = m.TRANSACTION_URL,
                    TRANSACTION_NOTES = m.TRANSACTION_NOTES,
                };
            });

            _financeTrackerContext.FT_TRANSACTION.AddRange(iterData.Select(m => m.ENTITY));

            _financeTrackerContext.SaveChanges();

            _financeTrackerContext.FT_ID_XREF.AddRange(iterData.Select(m => new FT_ID_XREF
            {
                OLD_ID = m.TRANSACTION_ID,
                NEW_ID = m.ENTITY.TRANSACTION_ID,
                TABLE_ID = (int)FTTableIds.FT_TRANSACTION
            }));

            _financeTrackerContext.SaveChanges();
        }

        int importCount = _financeTrackerContext.FT_TRANSACTION.Count();


        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class FTTransactionDataImport
{
    public int TRANSACTION_ID { get; set; }
    public int ACCOUNT_ID { get; set; }
    public string TRANSACTION_NAME { get; set; }
    public DateTime START_DATE { get; set; }
    public DateTime? END_DATE { get; set; }
    public decimal TRANSACTION_AMOUNT { get; set; }
    public int FREQUENCY_TYPE_ID { get; set; }
    public string TRANSACTION_NOTES { get; set; }
    public string TRANSACTION_URL { get; set; }
    public FT_TRANSACTION ENTITY { get; set; }

}
