using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Batch;
public class ReturningSeriesModel
{
    public int TvInfoId { get; set; }

    public string SeriesName { get; set; }

    public DateTime LastRefreshDate { get; set; }
}
