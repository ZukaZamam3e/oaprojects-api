using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.CatanLogger.Models;
public class GroupModel
{
    public int GroupId { get; set; }

    public string GroupName { get; set; }

    public DateTime DateAdded { get; set; }

    public IEnumerable<UserGroupModel> Users { get; set; }
}
