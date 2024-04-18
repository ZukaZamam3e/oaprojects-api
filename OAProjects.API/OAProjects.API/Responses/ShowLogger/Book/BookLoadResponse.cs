using OAProjects.Models.ShowLogger.Models.Book;

namespace OAProjects.API.Responses.ShowLogger.Book;

public class BookLoadResponse
{
    public IEnumerable<BookModel> Books { get; set; }

    public int Count { get; set; }
}
