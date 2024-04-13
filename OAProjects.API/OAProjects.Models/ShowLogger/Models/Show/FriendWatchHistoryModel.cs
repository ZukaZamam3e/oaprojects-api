using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Show;
public class FriendWatchHistoryModel : DetailedShowModel
{
    [Display(Name = "Name")]
    public string Name { get; set; }
}
