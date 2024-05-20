using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Responses.Info;

public class InfoSearchApiResponse
{
    public IEnumerable<ApiSearchResultModel> SearchResults { get; set; }

    public string ResultMessage { get; set; }

    public int Count { get; set; }
}
