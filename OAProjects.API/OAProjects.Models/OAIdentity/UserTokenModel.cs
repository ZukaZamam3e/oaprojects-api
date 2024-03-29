using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.OAIdentity;
public class UserTokenModel
{
    public int UserTokenId { get; set; }

    public int UserId { get; set; }

    public string Token { get; set; }

    public int ExpiryTime { get; set; }
}
