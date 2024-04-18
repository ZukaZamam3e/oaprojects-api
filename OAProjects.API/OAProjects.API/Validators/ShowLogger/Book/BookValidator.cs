using FluentValidation;
using OAProjects.Models.ShowLogger.Models.Book;

namespace OAProjects.API.Validators.ShowLogger.Book;

public class BookValidator : AbstractValidator<BookModel>
{
    public BookValidator()
    {
        RuleFor(x => x.BookName).NotEmpty();
    }
}
