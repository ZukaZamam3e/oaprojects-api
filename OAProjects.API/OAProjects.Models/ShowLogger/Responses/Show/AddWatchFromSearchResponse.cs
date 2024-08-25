using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Models.ShowLogger.Models.WatchList;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Responses.Show;
public class AddWatchFromSearchResponse
{
    public DetailedShowModel Show { get; set; }
    public DetailedWatchListModel WatchList { get; set; }

    public bool IsSuccess => Show != null || WatchList != null;
}
