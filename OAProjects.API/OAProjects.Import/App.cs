using OAProjects.Data.ShowLogger.Context;
using OAProjects.Import.Imports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Import;
public class App(IRestartImport _restartImport,
        IUserImport _userImport,
        IInfoImport _infoImport,
        IShowImport _showImport,
        ITransactionImport _transactionImport,
        IFriendImport _friendImport,
        IUserPrefImport _userPrefImport,
        IWatchListImport _watchListImport,
        IBookImport _bookImport,
        IFTAccountImport _accountImport,
        IFTTransactionImport _ftTransactionImport,
        IFTTransactionOffsetImport _fTTransactionOffsetImport,
        IFTRestartImport _ftRestartImport
        )
{
   
    public void Run(string[] args)
    {
        bool runProcess1 = args.Contains("p1");
        bool runProcess2 = args.Contains("p2");

        if (runProcess1)
        {
            _restartImport.RunImport();
            _userImport.RunImport();
            _infoImport.RunImport();
            _showImport.RunImport();
            _transactionImport.RunImport();
            _friendImport.RunImport();
            _userPrefImport.RunImport();
            _watchListImport.RunImport();
            _bookImport.RunImport();
        }
        else if (runProcess2)
        {
            _ftRestartImport.RunImport();
            _accountImport.RunImport();
            _ftTransactionImport.RunImport();
            _fTTransactionOffsetImport.RunImport();
        }

        Console.Write("Import finished.");
    }
}
