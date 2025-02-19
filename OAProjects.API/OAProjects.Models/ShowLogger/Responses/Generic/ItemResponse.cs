namespace OAProjects.Models.ShowLogger.Responses.Generic;
public class ItemResponse<T>
{
    public IEnumerable<T> Items { get; set; }

    public int Count { get; set; }
}
