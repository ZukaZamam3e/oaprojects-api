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
public interface IBookImport : IImport { };
internal class BookImport : IBookImport
{
    private readonly ShowLoggerDbContext _showLoggerContext;
    private readonly OAIdentityDbContext _oaIdentityContext;
    private readonly DataConfig _dataConfig;

    public BookImport(ShowLoggerDbContext showLoggerContext,
        OAIdentityDbContext oaIdentityContext,
        DataConfig dataConfig)
    {
        _showLoggerContext = showLoggerContext;
        _oaIdentityContext = oaIdentityContext;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- Book Import Started -----------");
        Console.WriteLine("Importing SL_BOOK");

        string json = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_book));
        IEnumerable<BookImportData> bookData = JsonConvert.DeserializeObject<IEnumerable<BookImportData>>(json);

        int count = bookData.Count();
        Console.WriteLine($"Items to be imported: {count}");

        for (int i = 0; i < count; i += 100)
        {
            List<BookImportData> data = bookData.Skip(i).Take(100).ToList();

            int[] oldUserIds = data.Select(m => m.USER_ID).ToArray();

            Dictionary<int, int> dictUserIds = _oaIdentityContext.OA_ID_XREF.Where(m => m.TABLE_ID == (int)OATableIds.OA_USER && oldUserIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_BOOK
                {
                    USER_ID = dictUserIds[m.USER_ID],
                    BOOK_NAME = m.BOOK_NAME,
                    START_DATE = m.START_DATE,
                    END_DATE = m.END_DATE,
                    CHAPTERS = m.CHAPTERS,
                    PAGES = m.PAGES,
                    BOOK_NOTES = m.BOOK_NOTES,
                };
            });

            _showLoggerContext.SL_BOOK.AddRange(data.Select(m => m.ENTITY));

            _showLoggerContext.SaveChanges();

            _showLoggerContext.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.BOOK_ID,
                NEW_ID = m.ENTITY.BOOK_ID,
                TABLE_ID = (int)TableIds.SL_BOOK
            }));

            _showLoggerContext.SaveChanges();
        }

        int importCount = _showLoggerContext.SL_BOOK.Count();

        Console.WriteLine($"Items that were imported: {importCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class BookImportData
{
    public int BOOK_ID { get; set; }
    public int USER_ID { get; set; }
    public string BOOK_NAME { get; set; }
    public DateTime? START_DATE { get; set; }
    public DateTime? END_DATE { get; set; }
    public int? CHAPTERS { get; set; }
    public int? PAGES { get; set; }
    public string BOOK_NOTES { get; set; }

    public SL_BOOK ENTITY { get; set; }
}
