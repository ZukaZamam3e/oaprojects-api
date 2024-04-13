using OAProjects.Models.ShowLogger.Models.Stat;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IStatStore
{
    IEnumerable<TvStatModel> GetTVStats(int userId);

    IEnumerable<MovieStatModel> GetMovieStats(int userId);

    IEnumerable<YearStatModel> GetYearStats(int userId, Dictionary<int, string> users);

    
}
