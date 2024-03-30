using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.WatchList;
public class WatchListModel
{
    public int WatchlistId { get; set; }

    public int UserId { get; set; }

    public string ShowName { get; set; }

    public int ShowTypeId { get; set; }

    public string? ShowTypeIdZ { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public DateTime DateAdded { get; set; }

    public string? ShowNotes { get; set; }
}
