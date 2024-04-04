using OAProjects.Models.ShowLogger.Models.CodeValue;

namespace OAProjects.API.Responses.ShowLogger.Info;

public class InfoLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> InfoApiIds { get; set; }
    public IEnumerable<SLCodeValueSimpleModel> InfoTypeIds { get; set; }
}
