using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Info;

public enum INFO_API
{
    TMDB_API = 0,
    OMDB_API = 1
}

public enum INFO_TYPE
{
    TV = 0,
    MOVIE = 1,
    EPISODE = 2
}

public static class ApiResults
{
    public const string NotFound = "Not found.";
    public const string MoreThanOneResult = "More than one result found. Please refine your search or enter one of the following ids.<br />{0}";
    public const string Found = "Found. Redirecting in a few seconds.";
    public const string Success = "Success";
    public const string SearchNameMissing = "Name is missing. Please enter a name.";
    public const string SearchNameTooShort = "Name is too short. Please enter a longer name.";

    public const string TMDBApiKeyMissing = "TMDb API Key is empty.";
    public const string OMDBApiKeyMissing = "OMDb API Key is empty.";
}

public class InfoApiSearchModel
{
    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Name { get; set; }
}

public class InfoApiDownloadModel
{
    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Id { get; set; }
}
