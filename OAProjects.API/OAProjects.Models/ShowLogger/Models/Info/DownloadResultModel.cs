using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Info;
public class DownloadResultModel
{
    public string Result { get; set; }

    public bool IsSuccessful => Result == ApiResults.Success;

    public INFO_API API { get; set; }

    public INFO_TYPE Type { get; set; }

    public long Id { get; set; }
}
