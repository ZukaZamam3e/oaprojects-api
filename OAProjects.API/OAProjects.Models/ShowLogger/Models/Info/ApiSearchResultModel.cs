namespace OAProjects.Models.ShowLogger.Models.Info;
public class ApiSearchResultModel
{
    public INFO_API Api { get; set; }

    public INFO_TYPE Type { get; set; }

    public string Id { get; set; }

    public string Name { get; set; }

    public DateTime? AirDate { get; set; }

    public string AirYear { get; set; }

    public string AirDateZ => AirDate.HasValue ? AirDate.Value.ToShortDateString() : !string.IsNullOrEmpty(AirYear) ? AirYear : "No Air Date Available";

    public string Link { get; set; }

    public string ImageUrl { get; set; }

    

}
