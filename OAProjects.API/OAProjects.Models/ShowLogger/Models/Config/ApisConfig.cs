using OAProjects.Models.ShowLogger.Models.Info;

namespace OAProjects.Models.ShowLogger.Models.Config;
public class ApisConfig
{
    public string TMDbURL { get; set; }
    public string TMDbAPIKey { get; set; }

    public string GetImageUrl(int? apiType, string? imageUrl)
    {
        if (apiType == null
            || string.IsNullOrEmpty(imageUrl))
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{TMDbURL}{TMDBApiPaths.Image}{imageUrl}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    public string GetTvEpisodeInfoUrl(int? apiType, string? apiId, int? seasonNumber, int? episodeNumber)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId)
            || seasonNumber == null
            || episodeNumber == null)
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{TMDbURL}{TMDBApiPaths.TV}{$"{apiId}/season/{seasonNumber}/episode/{episodeNumber}"}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    public string GetMovieInfoUrl(int? apiType, string? apiId)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId))
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{TMDbURL}{TMDBApiPaths.Movie}{apiId}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    public string GetTvInfoUrl(int? apiType, string? apiId)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId))
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{TMDbURL}{TMDBApiPaths.TV}{apiId}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }

    public string GetTvInfoSeasonUrl(int? apiType, string? apiId, int? seasonNumber)
    {
        if (apiType == null
            || string.IsNullOrEmpty(apiId)
            || seasonNumber == null)
        {
            return "";
        }

        return (INFO_API)apiType switch
        {
            INFO_API.TMDB_API => $"{TMDbURL}{TMDBApiPaths.TV}{apiId}/season/{seasonNumber}",
            INFO_API.OMDB_API => "",
            _ => throw new NotImplementedException(),
        };
    }
}

public static class TMDBApiPaths
{
    public const string TV = "tv/";
    public const string Movie = "movie/";
    public const string Image = "t/p/original";
}