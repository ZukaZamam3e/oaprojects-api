namespace OAProjects.Models.Common.Responses;

public class ListResponse<T>
{
    IEnumerable<T> Results { get; set; }
}
