namespace OAProjects.Models.FinanceTracker.Models;
public record FTCodeValueModel
{
    public int CodeValueId { get; set; }

    public int CodeTableId { get; set; }

    public string CodeTable { get; set; }

    public string DecodeTxt { get; set; }
}
