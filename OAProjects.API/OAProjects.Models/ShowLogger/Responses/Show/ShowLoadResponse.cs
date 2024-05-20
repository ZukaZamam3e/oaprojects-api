using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.Models.ShowLogger.Responses.Show;

public class ShowLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> ShowTypeIds { get; set; }

    public IEnumerable<SLCodeValueSimpleModel> TransactionTypeIds { get; set; }

    public IEnumerable<TransactionItemModel> TransactionItems { get; set; }

    public IEnumerable<DetailedShowModel> Shows { get; set; }

    public int Count { get; set; }
}
