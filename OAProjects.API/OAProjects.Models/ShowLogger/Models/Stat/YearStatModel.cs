using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Stat;
public class YearStatModel
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public int Year { get; set; }

    public int TvCnt { get; set; }

    public int TvNotTrackedCnt { get; set; }

    public int? TvRuntime { get; set; }

    public string TvRuntimeZ => ConvertRuntime(TvRuntime);

    public string TvStats => $"{TvCnt} ({TvNotTrackedCnt}){TvRuntimeZ}";

    public int MoviesCnt { get; set; }

    public int MoviesNotTrackedCnt { get; set; }

    public int? MoviesRuntime { get; set; }

    public string MoviesRuntimeZ => ConvertRuntime(MoviesRuntime);

    public string MovieStats => $"{MoviesCnt} ({MoviesNotTrackedCnt}){MoviesRuntimeZ}";

    public int AmcCnt { get; set; }

    public int AmcNotTrackedCnt { get; set; }

    public int? AmcRuntime { get; set; }

    public string AmcRuntimeZ => ConvertRuntime(AmcRuntime);

    public string AmcStats => $"{AmcCnt} ({AmcNotTrackedCnt}){AmcRuntimeZ}";

    public decimal AListMembership { get; set; }

    public decimal AListTickets { get; set; }

    public decimal AmcPurchases { get; set; }

    private string ConvertRuntime(int? minutes)
    {
        if (minutes == null)
            return "";

        TimeSpan timespan = TimeSpan.FromMinutes(minutes.Value);

        StringBuilder result = new StringBuilder("");

        if (timespan.Days > 0)
        {
            result.Append($"{timespan.Days} day");

            if (timespan.Days > 1)
            {
                result.Append('s');
            }

            result.Append(' ');
        }

        if (timespan.Hours > 0)
        {
            result.Append($"{timespan.Hours} hour");

            if (timespan.Hours > 1)
            {
                result.Append('s');
            }

            result.Append(' ');
        }

        if (timespan.Minutes > 0)
        {
            result.Append($"{timespan.Minutes} minute");

            if (timespan.Minutes > 1)
            {
                result.Append("s");
            }

            result.Append(" ");
        }

        return result.ToString();
    }
}
