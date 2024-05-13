using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.WatchList;
public class WatchListInfoModel : DetailedWatchListModel
{
    public int? InfoSeasonNumber { get; set; }
    public int? InfoEpisodeNumber { get; set; }
    public int? InfoApiType { get; set; }
    public string? InfoApiId { get; set; }
    public string? InfoImageUrl { get; set; }
}
