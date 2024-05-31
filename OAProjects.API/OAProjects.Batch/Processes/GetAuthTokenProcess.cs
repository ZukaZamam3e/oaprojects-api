using Newtonsoft.Json;
using OAProjects.Batch.Config;
using OAProjects.Batch.Models;
using OAProjects.Batch.Processes.Interface;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace OAProjects.Batch.Processes;
public class GetAuthTokenProcess : IGetAuthTokenProcess
{
    private readonly Auth0APIConfig _auth0APIConfig;
    private readonly IHttpClientFactory _httpClientFactory;

    public GetAuthTokenProcess(
        Auth0APIConfig auth0APIConfig,
        IHttpClientFactory httpClientFactory)
    {
        _auth0APIConfig = auth0APIConfig;
        _httpClientFactory = httpClientFactory;
    }

    public async Task<string> Run()
    {
        HttpClient httpClient = _httpClientFactory.CreateClient("Auth0API");

        using StringContent jsonContent = new(
            System.Text.Json.JsonSerializer.Serialize(new
            {
                client_id = _auth0APIConfig.Auth0ClientId,
                client_secret = _auth0APIConfig.Auth0ClientSecret,
                audience = _auth0APIConfig.Auth0Audience,
                grant_type = _auth0APIConfig.Auth0GrantType,
                scopes = _auth0APIConfig.Auth0Scopes
            }),
            Encoding.UTF8,
            "application/json");

        using HttpResponseMessage response = await httpClient.PostAsync($"oauth/token", jsonContent);

        response.EnsureSuccessStatusCode();

        string result = await response.Content.ReadAsStringAsync();

        AuthTokenResponse? authToken = JsonConvert.DeserializeObject<AuthTokenResponse>(result);

        string token = "";

        if(authToken != null)
        {
            token = $"{authToken.TokenType} {authToken.AccessToken}";
        }

        return token;
    }
}
