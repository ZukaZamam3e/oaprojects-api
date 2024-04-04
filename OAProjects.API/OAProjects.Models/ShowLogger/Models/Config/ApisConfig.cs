namespace OAProjects.Models.ShowLogger.Models.Config;
public class ApisConfig
{
    public string TMDbURL { get; set; }
    public string TMDbAPIKey { get; set; }
}

public static class TMDBApiPaths
{
    public const string TV = "tv/";
    public const string Movie = "movie/";
    public const string Image = "t/p/original/";
}