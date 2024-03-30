namespace OAProjects.Models.ShowLogger.Models.CodeValue;
public class SLCodeValueModel
{
    public int CodeValueId { get; set; }

    public int CodeTableId { get; set; }

    public string CodeTable { get; set; }

    public string DecodeTxt { get; set; }
}

public class SLCodeValueSimpleModel
{
    public int CodeValueId { get; set; }

    public string DecodeTxt { get; set; }
}