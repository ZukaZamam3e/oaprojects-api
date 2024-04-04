using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.API.Responses.ShowLogger.Info;

public class InfoSearchApiResponse
{
    public IEnumerable<ApiSearchResultModel> SearchResults { get; set; }

    public string ResultMessage { get; set; }

    public int Count { get; set; }
}
