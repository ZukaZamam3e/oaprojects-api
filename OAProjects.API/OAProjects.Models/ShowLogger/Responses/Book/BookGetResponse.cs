using OAProjects.Models.ShowLogger.Models.Book;

namespace OAProjects.Models.ShowLogger.Responses.Book;

public class BookGetResponse
{
    public IEnumerable<BookModel> Books { get; set; }

    public int Count { get; set; }
}
