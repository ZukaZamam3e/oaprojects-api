using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_WATCHED
{
    public int WATCHED_ID { get; set; }

    public int USER_ID { get; set; }

    public int INFO_TYPE { get; set; }

    public int INFO_ID { get; set; }

    public DateTime? DATE_WATCHED { get; set; }
}
