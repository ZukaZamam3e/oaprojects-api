using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.ShowLogger.Models.Config;
public class Auth0Config
{
    public string Domain { get; set; }
    public string Audience { get; set; }
    public string Url { get; set; }
    public string Auth0ClientId { get; set; }
    public string Auth0ClientSecret { get; set; }
    public string Auth0Audience { get; set; }
    public string Auth0GrantType { get; set; }
    public string Auth0Scopes { get; set; }
}