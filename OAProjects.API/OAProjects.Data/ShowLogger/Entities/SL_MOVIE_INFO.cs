using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_MOVIE_INFO
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MOVIE_INFO_ID { get; set; }

    public string MOVIE_NAME { get; set; }

    public string? MOVIE_OVERVIEW { get; set; }

    public int? API_TYPE { get; set; }

    public string? API_ID { get; set; }

    public string? OTHER_NAMES { get; set; }

    public int? RUNTIME { get; set; }

    public DateTime? AIR_DATE { get; set; }

    public DateTime LAST_DATA_REFRESH { get; set; }

    public DateTime LAST_UPDATED { get; set; }

    public string? IMAGE_URL { get; set; }
}
