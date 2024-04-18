using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using OAProjects.API.Responses.ShowLogger.Show;
using OAProjects.API.Responses;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;
using OAProjects.Store.OAIdentity.Stores.Interfaces;
using OAProjects.Store.ShowLogger.Stores;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System.Linq.Expressions;
using OAProjects.Models.ShowLogger.Models.Book;
using OAProjects.API.Responses.ShowLogger.Book;
using TMDbLib.Objects.Search;
using FluentValidation;
using OAProjects.API.Requests.Show;
using OAProjects.API.Requests.Book;
using FluentValidation.Results;

namespace OAProjects.API.Controllers.ShowLogger;

[ApiController]
[Route("api/[controller]")]
[EnableCors("_myAllowSpecificOrigins")]
[Authorize("User.ReadWrite")]
public class BookController : BaseController
{
    private readonly ILogger<BookController> _logger;
    private readonly IBookStore _bookStore;
    private readonly ICodeValueStore _codeValueStore;

    public BookController(ILogger<BookController> logger,
        IUserStore userStore,
        IBookStore bookStore,
        IHttpClientFactory httpClientFactory)
        : base(logger, userStore, httpClientFactory)
    {
        _logger = logger;
        _bookStore = bookStore;
    }

    [HttpGet("Load")]
    public async Task<IActionResult> Load()
    {
        GetResponse<BookLoadResponse> response = new GetResponse<BookLoadResponse>();

        try
        {
            int take = 10;
            int userId = await GetUserId();
            response.Model = new BookLoadResponse();

            response.Model.Books = GetBooks(userId);
            response.Model.Count = response.Model.Books.Count();
            response.Model.Books = response.Model.Books.OrderByDescending(m => m.EndDate == null).ThenByDescending(m => m.EndDate).ThenByDescending(m => m.StartDate).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpGet("Get")]
    public async Task<IActionResult> Get(int offset = 0, string? search = null, int take = 10)
    {
        GetResponse<BookGetResponse> response = new GetResponse<BookGetResponse>();

        try
        {
            int userId = await GetUserId();

            response.Model = new BookGetResponse();

            response.Model.Books = GetBooks(userId, search);
            response.Model.Count = response.Model.Books.Count();
            response.Model.Books = response.Model.Books.OrderByDescending(m => m.EndDate == null).ThenByDescending(m => m.EndDate).ThenByDescending(m => m.StartDate).Skip(offset).Take(take).ToArray();
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }
        return Ok(response);
    }

    private IEnumerable<BookModel> GetBooks(int userId, string? search = null)
    {
        Expression<Func<BookModel, bool>>? predicate = null;

        if (!string.IsNullOrEmpty(search))
        {
            predicate = m => m.BookName.ToLower().Contains(search.ToLower())
                && m.UserId == userId;
        }
        else
        {
            predicate = m => m.UserId == userId;
        }

        IEnumerable<BookModel> query = _bookStore.GetBooks(predicate);

        return query;
    }

    [HttpPost("Save")]
    public async Task<IActionResult> SaveShow(BookModel model,
        [FromServices] IValidator<BookModel> validator)
    {
        PostResponse<BookModel> response = new PostResponse<BookModel>();
        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(model);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                int bookId = model.BookId;

                if (bookId <= 0)
                {
                    bookId = _bookStore.CreateBook(userId, model);
                }
                else
                {
                    _bookStore.UpdateBook(userId, model);
                }

                response.Model = _bookStore.GetBooks(m => m.UserId == userId && m.BookId == bookId).First();
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }

    [HttpPost("Delete")]
    public async Task<IActionResult> Delete(BookIdRequest request,
        [FromServices] IValidator<BookIdRequest> validator)
    {
        PostResponse<bool> response = new PostResponse<bool>();

        try
        {
            int userId = await GetUserId();
            ValidationResult result = await validator.ValidateAsync(request);

            if (!result.IsValid)
            {
                response.Errors = result.Errors.Select(m => m.ErrorMessage);
            }
            else
            {
                response.Model = _bookStore.DeleteBook(userId, request.BookId);
            }
        }
        catch (Exception ex)
        {
            response.Errors = new List<string>() { ex.Message };
        }

        return Ok(response);
    }
}
