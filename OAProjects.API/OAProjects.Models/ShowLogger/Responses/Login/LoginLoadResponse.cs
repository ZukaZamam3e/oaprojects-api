using OAProjects.Models.ShowLogger.Models.Login;

namespace OAProjects.Models.ShowLogger.Responses.Login;
public class LoginLoadResponse
{
    public IEnumerable<string> Pages { get; set; }

    public UserPrefModel UserPref { get; set; }
}
