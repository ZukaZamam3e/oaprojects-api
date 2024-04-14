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

public interface IUserImport : IImport { };

public class UserImport : IUserImport
{
    private readonly OAIdentityDbContext _context;
    private readonly DataConfig _dataConfig;

    public UserImport(OAIdentityDbContext context,
        DataConfig dataConfig)
    {
        _context = context;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        Console.WriteLine("----------- User Import Started -----------");
        Console.WriteLine("Importing OA_USER");

        string usersJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.oa_users));
        IEnumerable<UserDataImport> usersData = JsonConvert.DeserializeObject<IEnumerable<UserDataImport>>(usersJson);

        int userCount = usersData.Count();
        Console.WriteLine($"Items to be imported: {userCount}");

        for (int i = 0; i < userCount; i += 100)
        {
            List<UserDataImport> data = usersData.Skip(i).Take(100).ToList();

            data.ForEach(m =>
            {
                int atIndex = m.EMAIL.IndexOf("@");
                m.ENTITY = new OA_USER
                {
                    FIRST_NAME = m.FIRSTNAME,
                    LAST_NAME = m.LASTNAME,
                    EMAIL = m.EMAIL,
                    USER_NAME = m.EMAIL.Substring(0, atIndex),
                    DATE_ADDED = DateTime.Now
                };
            });

            _context.OA_USER.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.OA_ID_XREF.AddRange(data.Select(m => new OA_ID_XREF
            {
                OLD_ID = m.USERID,
                NEW_ID = m.ENTITY.USER_ID,
                TABLE_ID = (int)OATableIds.OA_USER
            }));

            _context.SaveChanges();
        }

        int userImportCount = _context.OA_ID_XREF.Count();

        Console.WriteLine($"Items that were imported: {userImportCount}");
        Console.WriteLine("----------------------------------------------");
    }
}

public class UserDataImport
{
    public int USERID { get; set; }
    public string FIRSTNAME { get; set; }
    public string LASTNAME { get; set; }
    public string EMAIL { get; set; }
    public OA_USER ENTITY { get; set; }
}
