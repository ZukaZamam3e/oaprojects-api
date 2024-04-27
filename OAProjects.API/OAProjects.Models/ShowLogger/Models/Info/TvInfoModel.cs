using System.ComponentModel.DataAnnotations;
using System.Text.Json.Serialization;

namespace OAProjects.Models.ShowLogger.Models.Info;
public class TvInfoModel
{
    public int TvInfoId { get; set; }

    public string ShowName { get; set; }

    public string ShowOverview { get; set; }

    public int? ApiType { get; set; }

    public string? ApiId { get; set; }

    public DateTime LastDataRefresh { get; set; }

    public DateTime LastUpdated { get; set; }

    public string? ImageUrl { get; set; }

    public string? Status { get; set; }

    public IEnumerable<TvInfoSeasonModel> Seasons { get; set; }

    [Display(Name = "Episodes")]
    [JsonIgnore]
    public IEnumerable<TvEpisodeInfoModel> Episodes { get; set; }
}
