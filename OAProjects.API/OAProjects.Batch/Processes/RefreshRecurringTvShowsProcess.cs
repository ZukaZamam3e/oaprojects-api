using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using OAProjects.Batch.Config;
using OAProjects.Batch.Models;
using OAProjects.Batch.Processes.Interface;
using OAProjects.Models.Common.Responses;
using OAProjects.Models.ShowLogger.Models.Batch;
using OAProjects.Models.ShowLogger.Requests.Batch;
using OAProjects.Models.ShowLogger.Responses.Batch;
using Serilog;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAProjects.Batch.Processes;
public class RefreshRecurringTvShowsProcess : IRefreshRecurringTvShowsProcess
{
    private readonly Auth0APIConfig _auth0APIConfig;
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly IGetAuthTokenProcess _processGetAuthToken;
    private readonly ILogger _logger;

    public RefreshRecurringTvShowsProcess(
        Auth0APIConfig auth0APIConfig,
        IHttpClientFactory httpClientFactory,
        IGetAuthTokenProcess processGetAuthToken,
        ILogger logger)
    {
        _auth0APIConfig = auth0APIConfig;
        _httpClientFactory = httpClientFactory;
        _processGetAuthToken = processGetAuthToken;
        _logger = logger;
    }

    public async Task<bool> Run()
    {
        bool successful = false;

        string token = await _processGetAuthToken.Run();

        if(!string.IsNullOrEmpty(token))
        {
            Stopwatch timer = new Stopwatch();

            IEnumerable<ReturningSeriesModel> returningSeries = await GetReturningSeries(token);

            int total = returningSeries.Count();
            int index = 1;

            // calculate how many spots
            int tens = 0;
            int div = total;

            while(div > 0)
            {
                tens++;
                div /= 10;
            }

            foreach(ReturningSeriesModel series in returningSeries)
            {
                timer.Restart();
                bool refreshSuccessful = await RefreshSeries(series, token);
                timer.Stop();


                if (refreshSuccessful)
                {
                    
                    _logger.Information($"{$"{index++}/{total}",8}: {series.SeriesName} refreshed successfully after {timer.ElapsedMilliseconds} milliseconds.");
                }
                else
                {
                    _logger.Information($"{$"{index++}/{total}", 8}: {series.SeriesName} failed to refresh after {timer.ElapsedMilliseconds} milliseconds.");
                }
            }
        }

        return successful;
    }

    private async Task<IEnumerable<ReturningSeriesModel>> GetReturningSeries(string token)
    {
        IEnumerable<ReturningSeriesModel> returningSeries = new List<ReturningSeriesModel>();

        HttpClient httpClient = _httpClientFactory.CreateClient("OAProjectsAPI");
        httpClient.DefaultRequestHeaders.Add("Authorization", token);
        using HttpResponseMessage response = await httpClient.GetAsync($"batch/getreturningseries");
        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();
        GetResponse<ReturningSeriesResponse>? returningSeriesResponse = JsonConvert.DeserializeObject<GetResponse<ReturningSeriesResponse>>(result);

        if(returningSeriesResponse != null )
        {
            returningSeries = returningSeriesResponse.Model.ReturningSeries;
        }

        return returningSeries;
    }

    private async Task<bool> RefreshSeries(ReturningSeriesModel series, string token)
    {
        bool successful = false;

        HttpClient httpClient = _httpClientFactory.CreateClient("OAProjectsAPI");
        httpClient.DefaultRequestHeaders.Add("Authorization", token);

        using StringContent jsonContent = new(
            System.Text.Json.JsonSerializer.Serialize(new RefreshTvSeriesRequest
            {
                TvInfoId = series.TvInfoId,
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync("batch/refreshtvseries", jsonContent);
        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();
        PostResponse<bool>? refreshTvSeriesResponse = JsonConvert.DeserializeObject<PostResponse<bool>>(result);

        if(refreshTvSeriesResponse != null )
        {
            successful = refreshTvSeriesResponse.Model;
        }

        return successful;
    }
}
