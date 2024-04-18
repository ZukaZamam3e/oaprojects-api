namespace OAProjects.Models.ShowLogger.Models.Book;
public class BookModel
{
    public int BookId { get; set; }

    public int UserId { get; set; }

    public string BookName { get; set; }

    public DateTime? StartDate { get; set; }

    public DateTime? EndDate { get; set; }

    public int? Chapters { get; set; }

    public int? Pages { get; set; }

    public string? BookNotes { get; set; }
}
