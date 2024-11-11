using Newtonsoft.Json;
using OAProjects.Data.FinanceTracker.Context;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.OAIdentity.Entities;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Import.Config;

namespace OAProjects.Import.Imports;

public interface IFTTransactionOffsetImport : IImport { };

internal class FTTransactionOffsetImport(
    FinanceTrackerDbContext _financeTrackerContext,
    OAIdentityDbContext _oaIdentityContext,
    DataConfig _dataConfig) : IFTTransactionOffsetImport
{
    public void RunImport()
    {
        Console.WriteLine("----------- Account Import Started -----------");
        Console.WriteLine("Importing FT_ACCOUNT");



        string json = File.ReadAllText(Path.Join(_dataConfig.FinanceDataFolderPath, ImportFiles.ft_transaction_offset));
        IEnumerable<FTTransactionOffsetDataImport> data = JsonConvert.DeserializeObject<IEnumerable<FTTransactionOffsetDataImport>>(json);

        int[] oldTransactionIds = data.Select(m => m.TRANSACTION_ID).ToArray();

        Dictionary<int, int> dictTransactionIds = _financeTrackerContext.FT_ID_XREF.Where(m => m.TABLE_ID == (int)FTTableIds.FT_TRANSACTION).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);
        Dictionary<int, int> dictNewTransactionIds = _financeTrackerContext.FT_ID_XREF.Where(m => m.TABLE_ID == (int)FTTableIds.FT_TRANSACTION).ToDictionary(m => m.NEW_ID, m => m.OLD_ID);
        //int[] newIds = dictTransactionIds.Select(m => m.Value).ToArray();
        Dictionary<int, int> dictAccountIds = _financeTrackerContext.FT_TRANSACTION.ToDictionary(m => dictNewTransactionIds[m.TRANSACTION_ID], m => m.ACCOUNT_ID);
        Dictionary<int, int> dictUserIds = _financeTrackerContext.FT_TRANSACTION.ToDictionary(m => dictNewTransactionIds[m.TRANSACTION_ID], m => m.USER_ID);

        int count = data.Count();
        Console.WriteLine($"Items to be imported: {count}");

        for (int i = 0; i < count; i += 100)
        {
            List<FTTransactionOffsetDataImport> iterData = data.Skip(i).Take(100).ToList();

            iterData.ForEach(m =>
            {
                m.ENTITY = new FT_TRANSACTION_OFFSET
                {
                    ACCOUNT_ID = dictAccountIds[m.TRANSACTION_ID],
                    USER_ID = dictUserIds[m.TRANSACTION_ID],
                    TRANSACTION_ID = dictTransactionIds[m.TRANSACTION_ID],
                    OFFSET_AMOUNT = m.OFFSET_AMOUNT,
                    OFFSET_DATE = m.OFFSET_DATE
                };
            });

            _financeTrackerContext.FT_TRANSACTION_OFFSET.AddRange(iterData.Select(m => m.ENTITY));

            _financeTrackerContext.SaveChanges();

            _financeTrackerContext.FT_ID_XREF.AddRange(iterData.Select(m => new FT_ID_XREF
            {
                OLD_ID = m.TRANSACTION_OFFSET_ID,
                NEW_ID = m.ENTITY.TRANSACTION_OFFSET_ID,
                TABLE_ID = (int)FTTableIds.FT_TRANSACTION_OFFSET
            }));

            _financeTrackerContext.SaveChanges();
        }

        int importCount = _financeTrackerContext.FT_TRANSACTION_OFFSET.Count();


        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class FTTransactionOffsetDataImport
{
    public int TRANSACTION_OFFSET_ID { get; set; }
    public int TRANSACTION_ID { get; set; }
    public DateTime OFFSET_DATE { get; set; }
    public decimal OFFSET_AMOUNT { get; set; }
    public FT_TRANSACTION_OFFSET ENTITY { get; set; }

}
