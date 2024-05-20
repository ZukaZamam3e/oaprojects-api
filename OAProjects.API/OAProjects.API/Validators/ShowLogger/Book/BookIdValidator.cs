using FluentValidation;
using OAProjects.Models.ShowLogger.Requests.Book;

namespace OAProjects.API.Validators.ShowLogger.Book;

public class BookIdValidator : AbstractValidator<BookIdRequest>
{
    public BookIdValidator()
    {
        RuleFor(x => x.BookId).GreaterThan(0);
    }
}

