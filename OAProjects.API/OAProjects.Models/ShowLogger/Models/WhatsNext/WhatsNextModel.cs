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
    public int TvInfoId { get; set; }

    public string ShowName { get; set; }

    public string Status { get; set; }

    public int? SeasonNumber { get; set; }

    public string SeasonName { get; set; }


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
}
