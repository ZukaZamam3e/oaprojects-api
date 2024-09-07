using OAProjects.Models.ShowLogger.Models.Info;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Show;
public class AddWatchFromSearchModel
{
    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Id { get; set; }

    public string ShowName { get; set; }

    public int ShowTypeId { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public DateTime DateWatched { get; set; }

    public string? ShowNotes { get; set; }

    public bool RestartBinge { get; set; }

    public bool Watchlist { get; set; }

    public IEnumerable<ShowTransactionModel>? Transactions { get; set; }

}
