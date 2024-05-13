using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.WatchList;
public class DetailedWatchListModel : WatchListModel
{
    public string? EpisodeName { get; set; }

    public int? Runtime { get; set; }

    public string RuntimeZ => Runtime != null ? $"{Runtime} minutes" : "";

    public string? ImageUrl { get; set; }

    public string? InfoUrl { get; set; }

    public string SeasonEpisode => SeasonNumber != null && EpisodeNumber != null ? $"s{SeasonNumber.Value.ToString().PadLeft(2, '0')}e{EpisodeNumber.Value.ToString().PadLeft(2, '0')}" : "";

    public string ShowNameZ => $"{(ShowTypeId == 1000 ? GetShowShowNameZ : GetMovieShowNameZ)}";

    private string GetShowShowNameZ => $"{ShowName}{$"{(!string.IsNullOrEmpty(EpisodeName) ? $"{$" - <a href =\"{InfoUrl}\" target=\"_blank\">{EpisodeName}</a>"}<br>" : "")}"}";

    private string GetMovieShowNameZ => $"{(InfoId != null ? $"<a href =\"{InfoUrl}\" target=\"_blank\">{ShowName}</a>" : ShowName)}";
}
