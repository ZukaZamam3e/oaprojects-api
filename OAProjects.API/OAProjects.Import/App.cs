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

    public App(IRestartImport restartImport,
        IUserImport userImport,
        IInfoImport infoImport)
    {
        _restartImport = restartImport;
        _userImport = userImport;
        _infoImport = infoImport;
    }

    public void Run(string[] args)
    {
        _restartImport.RunImport();
        _userImport.RunImport();
        _infoImport.RunImport();

        Console.Write("Import finished.");
    }
}
