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
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace OAProjects.Import.Imports;

public interface ITransactionImport : IImport { };
public class TransactionImport : ITransactionImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public TransactionImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- Transaction Import Started -----------");
        Console.WriteLine("Importing SL_TRANSACTION");

        string json = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_transaction));
        IEnumerable<TransactionDataImport> transactionData = JsonConvert.DeserializeObject<IEnumerable<TransactionDataImport>>(json);

        int count = transactionData.Count();
        Console.WriteLine($"Items to be imported: {count}");

        IEnumerable<GroupedTransactionData> groupedTransactions = transactionData.GroupBy(m => new { m.SHOW_ID, m.TRANSACTION_DATE, m.USER_ID })
            .Select(m => new GroupedTransactionData
            {
                TransactionDate = m.Key.TRANSACTION_DATE,
                ShowId = m.Key.SHOW_ID,
                UserId = m.Key.USER_ID,
                Count = m.Count()
            }).OrderBy(m => m.TransactionDate);

        int[] oldUserIds = transactionData.Select(m => m.USER_ID).ToArray();
        Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

        int[] oldShowIds = transactionData.Where(m => m.SHOW_ID != null).Select(m => m.SHOW_ID.Value).ToArray();
        Dictionary<int, int> dictShowIds = _showLoggerContext.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_SHOW && oldShowIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);


        foreach (GroupedTransactionData groupedTransaction in groupedTransactions) 
        {
            IEnumerable<TransactionDataImport> transactions = transactionData.Where(m => m.SHOW_ID == groupedTransaction.ShowId && m.TRANSACTION_DATE == m.TRANSACTION_DATE).ToList();

            int userId = dictUserIds[groupedTransaction.UserId];
            int? showId = null;
            DateTime transactionDate = groupedTransaction.TransactionDate;

            if (groupedTransaction.ShowId != null)
            {
                showId = dictShowIds[groupedTransaction.ShowId.Value];
                TransactionDataImport? tax = transactions.FirstOrDefault(m => m.ITEM == "Tax");

                if (tax != null)
                {
                    SL_TRANSACTION taxEntity = new SL_TRANSACTION
                    {
                        USER_ID = userId,
                        TRANSACTION_TYPE_ID = (int)CodeValueIds.TAX,
                        SHOW_ID = showId,
                        ITEM = tax.ITEM,
                        COST_AMT = tax.COST_AMT,
                        QUANTITY = 1,
                        TRANSACTION_DATE = transactionDate,
                        TRANSACTION_NOTES = tax.TRANSACTION_NOTES,
                    };

                    _showLoggerContext.SL_TRANSACTION.Add(taxEntity);
                }

                IEnumerable<TransactionDataImport> benefits = transactions.Where(m => m.BENEFIT_AMT != null || m.ITEM == "AMC Stubs Benefit");

                if (benefits.Any())
                {
                    SL_TRANSACTION benefitEntity = new SL_TRANSACTION
                    {
                        USER_ID = userId,
                        TRANSACTION_TYPE_ID = (int)CodeValueIds.BENEFITS,
                        SHOW_ID = showId,
                        ITEM = "Benefits",
                        COST_AMT = benefits.Sum(m => m.BENEFIT_AMT).Value,
                        QUANTITY = 1,
                        TRANSACTION_DATE = transactionDate
                    };


                    _showLoggerContext.SL_TRANSACTION.Add(benefitEntity);
                }

                IEnumerable<TransactionDataImport> discounts = transactions.Where(m => m.DISCOUNT_AMT != null || m.ITEM == "AMC Stubs Rewards");

                if (discounts.Any())
                {
                    _showLoggerContext.SL_TRANSACTION.Add(new SL_TRANSACTION
                    {
                        USER_ID = userId,
                        TRANSACTION_TYPE_ID = (int)CodeValueIds.REWARDS,
                        SHOW_ID = showId,
                        ITEM = "Rewards",
                        COST_AMT = discounts.Sum(m => m.DISCOUNT_AMT).Value,
                        QUANTITY = 1,
                        TRANSACTION_DATE = transactionDate
                    });
                }

                IEnumerable<TransactionDataImport> tickets = transactions
                    .Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.TICKET 
                    || (m.TRANSACTION_TYPE_ID == (int)CodeValueIds.PURCHASE && m.ITEM == "Ticket"));

                if (tickets.Any())
                {
                    _showLoggerContext.SL_TRANSACTION.Add(new SL_TRANSACTION
                    {
                        USER_ID = userId,
                        TRANSACTION_TYPE_ID = (int)CodeValueIds.TICKET,
                        SHOW_ID = showId,
                        ITEM = "Ticket",
                        COST_AMT = tickets.Sum(m => m.COST_AMT),
                        QUANTITY = tickets.Count(),
                        TRANSACTION_DATE = transactionDate
                    });
                }

                IEnumerable<TransactionDataImport> aListTickets = transactions.Where(m => m.TRANSACTION_TYPE_ID == (int)CodeValueIds.ALIST_TICKET);

                if (aListTickets.Any())
                {
                    _showLoggerContext.SL_TRANSACTION.Add(new SL_TRANSACTION
                    {
                        USER_ID = userId,
                        TRANSACTION_TYPE_ID = (int)CodeValueIds.ALIST_TICKET,
                        SHOW_ID = showId,
                        ITEM = "A-list Ticket",
                        COST_AMT = aListTickets.Sum(m => m.COST_AMT),
                        QUANTITY = aListTickets.Count(),
                        TRANSACTION_DATE = transactionDate
                    });
                }

                int[] nonPurchases = [(int)CodeValueIds.TICKET, (int)CodeValueIds.REWARDS, (int)CodeValueIds.BENEFITS, (int)CodeValueIds.ALIST_TICKET, (int)CodeValueIds.TAX, (int)CodeValueIds.ALIST];
                string[] otherNonPurchases = ["AMC Stubs Benefit", "Tax", "AMC Stubs Rewards", "Ticket"];


                IEnumerable<TransactionDataImport> purchases = transactions.Where(m => !nonPurchases.Contains(m.TRANSACTION_TYPE_ID) && !otherNonPurchases.Contains(m.ITEM));

                if (purchases.Any())
                {
                    IEnumerable<SL_TRANSACTION> purchaseEntities = purchases.GroupBy(m => m.ITEM).Select(m => new SL_TRANSACTION
                    {
                        USER_ID = userId,
                        TRANSACTION_TYPE_ID = (int)CodeValueIds.PURCHASE,
                        SHOW_ID = showId,
                        ITEM = m.Key,
                        COST_AMT = m.Sum(n => n.COST_AMT),
                        QUANTITY = m.Count(),
                        TRANSACTION_DATE = transactionDate
                    });

                    _showLoggerContext.SL_TRANSACTION.AddRange(purchaseEntities);
                }

                _showLoggerContext.SaveChanges();
            }
            else
            {
                SL_TRANSACTION alistEntity = new SL_TRANSACTION
                {
                    USER_ID = userId,
                    TRANSACTION_TYPE_ID = (int)CodeValueIds.ALIST,
                    SHOW_ID = null,
                    ITEM = transactions.First().ITEM,
                    COST_AMT = transactions.First().COST_AMT,
                    QUANTITY = 1,
                    TRANSACTION_DATE = transactionDate,
                    TRANSACTION_NOTES = transactions.First().TRANSACTION_NOTES,
                };

                _showLoggerContext.SL_TRANSACTION.Add(alistEntity);

                _showLoggerContext.SaveChanges();
            }
        }

        int importCount = _showLoggerContext.SL_TRANSACTION.Count();

        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class TransactionDataImport
{
    public int TRANSACTION_ID { get; set; }
    public int USER_ID { get; set; }
    public int TRANSACTION_TYPE_ID { get; set; }
    public int? SHOW_ID { get; set; }
    public string ITEM { get; set; }
    public decimal COST_AMT { get; set; }
    public decimal? DISCOUNT_AMT { get; set; }
    public string TRANSACTION_NOTES { get; set; }
    public DateTime TRANSACTION_DATE { get; set; }
    public decimal? BENEFIT_AMT { get; set; }

    public SL_TRANSACTION ENTITY { get; set; }
}

public class GroupedTransactionData
{
    public DateTime TransactionDate { get; set; }

    public int? ShowId { get; set; }

    public int UserId { get; set; }

    public int Count { get; set; }
}
