using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models;
public class Utilities
{
}

public static class DateTimeExtensions
{
    public static DateTime GetEST(DateTime date)
    {
        DateTime timeUtc = DateTime.UtcNow;
        TimeZoneInfo easternZone = TimeZoneInfo.FindSystemTimeZoneById("Eastern Standard Time");
        DateTime easternTime = TimeZoneInfo.ConvertTimeFromUtc(timeUtc, easternZone);

        return easternTime;
    }
}
