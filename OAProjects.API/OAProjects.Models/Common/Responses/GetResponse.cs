namespace OAProjects.Models.Common.Responses;

public class GetResponse<T>
{
    public T Model { get; set; }

    public IEnumerable<string> Errors { get; set; } = new List<string>();

    public GetResponse()
    {

    }

    public GetResponse(T model, IEnumerable<string> errors)
    {
        Model = model;
        Errors = errors;
        Errors ??= new List<string>();
    }
}
