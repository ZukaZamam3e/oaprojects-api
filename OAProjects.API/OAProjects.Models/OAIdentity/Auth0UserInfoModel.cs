using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Models.OAIdentity;
public class Auth0UserInfoModel
{
    public string Sub { get; set; }

    [JsonProperty("given_name")]
    public string GivenName { get; set; }

    [JsonProperty("family_name")]
    public string FamilyName { get; set; }

    public string NickName { get; set; }

    public string Name { get; set; }

    public string Picture { get; set; }

    public string Locale { get; set; }

    [JsonProperty("updated_at")]
    public DateTime UpdatedAt { get; set; }

    public string Email { get; set; }

    [JsonProperty("email_verified")]
    public bool EmailVerified { get; set; }
}
