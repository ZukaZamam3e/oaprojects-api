namespace OAProjects.Models.ShowLogger.Models.Stat;
public class BookYearStatModel
{
    public int UserId { get; set; }

    public string Name { get; set; }

    public int Year { get; set; }

    public int BookCnt { get; set; }

    public int ChapterCnt { get; set; }

    public int PageCnt { get; set; }

    public decimal TotalDays { get; set; }

    public decimal DayAvg => TotalDays / BookCnt;

    public string DayAvgZ => $"{Math.Round(DayAvg, 2):0.##}";

    public decimal ChapterAvg => (decimal)ChapterCnt / BookCnt;

    public string ChapterAvgZ => $"{Math.Round(ChapterAvg, 2):0.##}";

    public decimal PageAvg => (decimal)PageCnt / BookCnt;

    public string PageAvgZ => $"{Math.Round(PageAvg, 2):0.##}";

    public decimal MonthAvg { get; set; }

    public string MonthAvgZ => $"{Math.Round(MonthAvg, 2):0.##}";

    public IEnumerable<BookYearStatDataModel> Data { get; set; }
}
