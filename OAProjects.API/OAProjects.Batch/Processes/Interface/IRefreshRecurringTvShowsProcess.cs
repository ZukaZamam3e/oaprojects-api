using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Batch.Processes.Interface;
public interface IRefreshRecurringTvShowsProcess : IProcess
{
    Task<bool> Run();
}
