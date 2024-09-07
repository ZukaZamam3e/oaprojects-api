using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Stat;
public class BookYearStatDataModel
{
    public int UserId { get; set; }

    public int Year => StartDate.Year;

    public string BookName { get; set; }

    public int Month => StartDate.Month;

    public DateTime StartDate { get; set; }

    public DateTime EndDate { get; set; }

    public int? Chapters { get; set; }

    public int? Pages { get; set; }
}
