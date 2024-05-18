using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Stat;
public class YearStatDataModel
{
    public int UserId { get; set; }

    public int Year { get; set; }

    public int ShowTypeId { get; set; }

    public string ShowTypeIdZ { get; set; }

    public string ShowName { get; set; }

    public int WatchCount { get; set; }

    public int? TotalRuntime { get; set; }

    public string TotalRuntimeZ => ConvertRuntime(TotalRuntime);

    public string? InfoBackdropUrl { get; set; }

    public string? InfoUrl { get; set; }

    private string ConvertRuntime(int? minutes)
    {
        if (minutes == null || minutes == 0)
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
                result.Append('s');
            }

            result.Append(' ');
        }

        return result.ToString().Trim();
    }
}
