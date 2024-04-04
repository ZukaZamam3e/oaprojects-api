using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.API.Responses.ShowLogger.Info;

public class DownloadInfoResponse
{
    public string Result { get; set; }

    public bool IsSuccessful { get; set; }

    public long Id { get; set; }
}
