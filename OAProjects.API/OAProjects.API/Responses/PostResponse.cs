namespace OAProjects.API.Responses;

public class PostResponse<T>
{
    public T Model { get; set; }

    public IEnumerable<string> Errors { get; set; } = new List<string>();

    public PostResponse()
    {

    }

    public PostResponse(T model, IEnumerable<string> errors)
    {
        Model = model;
        Errors = errors;
        Errors ??= new List<string>();
    }
}
