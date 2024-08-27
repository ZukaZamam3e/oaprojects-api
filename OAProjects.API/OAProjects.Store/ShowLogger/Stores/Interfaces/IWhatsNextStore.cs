using OAProjects.Models.ShowLogger.Models.WhatsNext;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IWhatsNextStore
{
    IEnumerable<WhatsNextModel> GetWhatsNext(int userId);
}
