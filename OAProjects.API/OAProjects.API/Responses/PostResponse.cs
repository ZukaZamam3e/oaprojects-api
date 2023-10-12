namespace OAProjects.API.Responses;

public class PostResponse<T>
{
    public T Model { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public PostResponse()
    {

    }

    public PostResponse(T model, IEnumerable<string> errors)
    {
        Model = model;
        Errors = errors;
    }
}
