using OAProjects.Data.ShowLogger.Context;
using OAProjects.Import.Imports;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Import;
public class App
{
    private readonly IRestartImport _restartImport;
    private readonly IUserImport _userImport;
    private readonly IInfoImport _infoImport;
    private readonly IShowImport _showImport;
    private readonly ITransactionImport _transactionImport;
    private readonly IFriendImport _friendImport;
    private readonly IUserPrefImport _userPrefImport;
    private readonly IWatchListImport _watchListImport;
    private readonly IBookImport _bookImport;

    public App(IRestartImport restartImport,
        IUserImport userImport,
        IInfoImport infoImport,
        IShowImport showImport,
        ITransactionImport transactionImport,
        IFriendImport friendImport,
        IUserPrefImport userPrefImport,
        IWatchListImport watchListImport,
        IBookImport bookImport)
    {
        _restartImport = restartImport;
        _userImport = userImport;
        _infoImport = infoImport;
        _showImport = showImport;
        _transactionImport = transactionImport;
        _friendImport = friendImport;
        _userPrefImport = userPrefImport;
        _watchListImport = watchListImport;
        _bookImport = bookImport;

    }

    public void Run(string[] args)
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

        Console.Write("Import finished.");
    }
}
