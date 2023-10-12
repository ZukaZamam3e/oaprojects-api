namespace OAProjects.API.Responses;

public class GetResponse<T>
{
    public T Model { get; set; }

    public IEnumerable<string> Errors { get; set; }

    public GetResponse()
    {

    }

    public GetResponse(T model, IEnumerable<string> errors)
    {
        Model = model;
        Errors = errors;
    }
}
