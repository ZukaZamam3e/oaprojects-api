using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.WhatsNext;
public class WhatsNextModel
{

    public int TvInfoId { get; set; }

    public string ShowName { get; set; }

    public int TvEpisodeInfoId { get; set; }

    public string EpisodeName { get; set; }

    public DateTime AirDate { get; set; }

    public string Status { get; set; }

}

public class WhatsNextModelInfo : WhatsNextModel
{
    public int UserId { get; set; }
    public int SeasonNumber { get; set; }

    public int[] TvEpisodeIds { get; set; }
}

public class WhatsNextShowModel
{
    public int WhatsNextSubId { get; set; }

    public int TvInfoId { get; set; }

    public string ShowName { get; set; }

    public string SeasonStatus { get; set; }

    public int? SeasonNumber { get; set; }

    public string SeasonName { get; set; }

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public string? PosterUrl { get; set; }

    public string? BackdropUrl { get; set; }

    public string? InfoUrl { get; set; }

    public string? SeasonUrl { get; set; }

    public string? Status { get; set; }

    public int DaysLeft { get; set; }

    public IEnumerable<WhatsNextEpisodeModel> Episodes { get; set; }
}

public class WhatsNextEpisodeModel
{
    public int TvEpisodeInfoId { get; set; }
    public int? SeasonNumber { get; set; }

    public string SeasonName { get; set; }

    public int? EpisodeNumber { get; set; }

    public string EpisodeName { get; set; }

    public DateTime AirDate { get; set; }

    public string EpisodeOverview { get; set; }

    public int? Runtime { get; set; }

    public string ImageUrl { get; set; }
}
