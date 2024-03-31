namespace OAProjects.API.Responses;

public class ListResponse<T>
{
    IEnumerable<T> Results { get; set; }
}
