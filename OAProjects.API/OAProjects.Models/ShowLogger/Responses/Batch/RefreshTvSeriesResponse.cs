using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Responses.Batch;
public class RefreshTvSeriesResponse
{
    public bool Successful { get; set; }

    public int UpdatedEpisodeCount { get; set; }
}
