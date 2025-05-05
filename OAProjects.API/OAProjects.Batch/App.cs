using Microsoft.Extensions.Logging;
using OAProjects.Batch.Processes;
using OAProjects.Batch.Processes.Interface;
using Serilog.Core;
using System.Diagnostics;
namespace OAProjects.Batch;
public class App
{

    private readonly IRefreshRecurringTvShowsProcess _refreshRecurringTvShowsProcess;
    private readonly ILogger _logger;

    public App(IRefreshRecurringTvShowsProcess refreshRecurringTvShowsProcess,
    ILogger<App> logger)
    {
        _refreshRecurringTvShowsProcess = refreshRecurringTvShowsProcess;
        _logger = logger;
    }

    public async Task<int> Run(string[] args)
    {
        _logger.LogInformation($"Starting OAProjects.Batch with args: {string.Join(' ', args)}");

        bool runProcess1 = args.Contains("p1");

        Stopwatch timer = new Stopwatch();

        if (runProcess1)
        {
            _logger.LogInformation($"Running refresh recurring TV shows process.");

            timer.Restart();
            await _refreshRecurringTvShowsProcess.Run();
            timer.Stop();

            _logger.LogInformation($"Refresh recurring TV shows has process completed after {timer.Elapsed.TotalSeconds} seconds.");
        }

        _logger.LogInformation($"OAProjects.Batch has completed.");

        return 1;
    }
}
