using OAProjects.Models.ShowLogger.Models.Book;
using OAProjects.Models.ShowLogger.Models.Stat;
using OAProjects.Store.Stores.Interfaces;
using System.Linq.Expressions;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IBookStore : IStore
{
    IEnumerable<BookModel> GetBooks(Expression<Func<BookModel, bool>>? predicate = null);

    int CreateBook(int userId, BookModel model);

    int UpdateBook(int userId, BookModel model);

    bool DeleteBook(int userId, int showId);

}
