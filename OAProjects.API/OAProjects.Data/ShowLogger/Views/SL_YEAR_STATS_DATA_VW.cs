using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Data.ShowLogger.Views;
public class SL_YEAR_STATS_DATA_VW
{
    public int USER_ID { get; set; }

    public string SHOW_NAME { get; set; }

    public int YEAR { get; set; }

    public int SHOW_TYPE_ID { get; set; }

    public int? API_TYPE { get; set; }

    public string? API_ID { get; set; }

    public string? BACKDROP_URL { get; set; }

    public int? TOTAL_RUNTIME { get; set; }

    public int WATCH_COUNT { get; set; }
}
