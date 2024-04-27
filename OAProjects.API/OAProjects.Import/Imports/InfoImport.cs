using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using OAProjects.Data.OAIdentity.Context;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Import.Config;

namespace OAProjects.Import.Imports;

public interface IInfoImport : IImport { };

public class InfoImport : IInfoImport
{
    private readonly ShowLoggerDbContext _context;
    private readonly DataConfig _dataConfig;

    public InfoImport(ShowLoggerDbContext context,
        DataConfig dataConfig)
    {
        _context = context;
        _dataConfig = dataConfig;
    }

    public void RunImport()
    {
        // SL_TV_INFO

        Console.WriteLine("----------- Info Import Started -----------");
        Console.WriteLine("Importing SL_TV_INFO");

        string tvInfoJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_tv_info));
        IEnumerable<TvInfoImport> tvInfoData = JsonConvert.DeserializeObject<IEnumerable<TvInfoImport>>(tvInfoJson);

        int tvInfoCount = tvInfoData.Count();
        Console.WriteLine($"Items to be imported: {tvInfoCount}");

        for (int i = 0; i < tvInfoCount; i+=100) 
        {
            List<TvInfoImport> data = tvInfoData.Skip(i).Take(100).ToList();

            data.ForEach(m =>
            {
                m.ENTITY = new SL_TV_INFO
                {
                    SHOW_NAME = m.SHOW_NAME,
                    SHOW_OVERVIEW = m.SHOW_OVERVIEW,
                    API_TYPE = m.API_TYPE,
                    API_ID = m.API_ID,
                    LAST_DATA_REFRESH = m.LAST_DATA_REFRESH,
                    LAST_UPDATED = m.LAST_UPDATED,
                    IMAGE_URL = m.IMAGE_URL,
                };
            });

            _context.SL_TV_INFO.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.TV_INFO_ID,
                NEW_ID = m.ENTITY.TV_INFO_ID,
                TABLE_ID = (int)TableIds.SL_TV_INFO
            }));

            _context.SaveChanges();
        }

        int tvInfoImportCount = _context.SL_TV_INFO.Count();
        Console.WriteLine($"Items that were imported: {tvInfoImportCount}");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("Importing SL_TV_EPISODE_INFO");

        string tvEpisodeInfoJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_tv_episode_info));
        IEnumerable<TvEpisodeInfoImport> tvEpisodeInfoData = JsonConvert.DeserializeObject<IEnumerable<TvEpisodeInfoImport>>(tvEpisodeInfoJson);

        int tvEpisodeInfoCount = tvEpisodeInfoData.Count();
        Console.WriteLine($"Items to be imported: {tvEpisodeInfoCount}");

        for (int i = 0; i < tvEpisodeInfoCount; i += 100)
        {
            List<TvEpisodeInfoImport> data = tvEpisodeInfoData.Skip(i).Take(100).ToList();

            int[] oldIds = data.Select(m => m.TV_INFO_ID).ToArray();

            Dictionary<int, int> dictIds = _context.SL_ID_XREF.Where(m => m.TABLE_ID == (int)TableIds.SL_TV_INFO && oldIds.Contains(m.OLD_ID)).ToDictionary(m => m.OLD_ID, m => m.NEW_ID);

            data.ForEach(m =>
            {
                m.ENTITY = new SL_TV_EPISODE_INFO
                {
                    TV_INFO_ID = dictIds[m.TV_INFO_ID],
                    SEASON_NAME = m.SEASON_NAME,
                    EPISODE_NAME = m.EPISODE_NAME,
                    SEASON_NUMBER = m.SEASON_NUMBER,
                    EPISODE_NUMBER = m.EPISODE_NUMBER,
                    EPISODE_OVERVIEW = m.EPISODE_OVERVIEW,
                    RUNTIME = m.RUNTIME,
                    AIR_DATE = m.AIR_DATE,
                    IMAGE_URL = m.IMAGE_URL,
                    API_TYPE = m.API_TYPE,
                    API_ID = m.API_ID,
                };
            });

            _context.SL_TV_EPISODE_INFO.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.TV_EPISODE_INFO_ID,
                NEW_ID = m.ENTITY.TV_EPISODE_INFO_ID,
                TABLE_ID = (int)TableIds.SL_TV_EPISODE_INFO
            }));

            _context.SaveChanges();
        }

        int tvEpisodeInfoImportCount = _context.SL_TV_EPISODE_INFO.Count();
        Console.WriteLine($"Items that were imported: {tvEpisodeInfoImportCount}");
        Console.WriteLine("----------------------------------------------");
        Console.WriteLine("Importing SL_MOVIE_INFO");

        string movieInfoJson = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, ImportFiles.sl_movie_info));
        IEnumerable<MovieInfoImport> movieInfoData = JsonConvert.DeserializeObject<IEnumerable<MovieInfoImport>>(movieInfoJson);

        int movieInfoCount = movieInfoData.Count();

        Console.WriteLine($"Items to be imported: {movieInfoCount}");


        for (int i = 0; i < movieInfoCount; i += 100)
        {
            List<MovieInfoImport> data = movieInfoData.Skip(i).Take(100).ToList();

            data.ForEach(m =>
            {
                m.ENTITY = new SL_MOVIE_INFO
                {
                    MOVIE_NAME = m.MOVIE_NAME,
                    MOVIE_OVERVIEW = m.MOVIE_OVERVIEW,
                    RUNTIME = m.RUNTIME,
                    AIR_DATE = m.AIR_DATE,
                    LAST_DATA_REFRESH = m.LAST_DATA_REFRESH,
                    LAST_UPDATED = m.LAST_UPDATED,
                    POSTER_URL = m.IMAGE_URL,
                    API_TYPE = m.API_TYPE,
                    API_ID = m.API_ID,
                };
            });

            _context.SL_MOVIE_INFO.AddRange(data.Select(m => m.ENTITY));

            _context.SaveChanges();

            _context.SL_ID_XREF.AddRange(data.Select(m => new SL_ID_XREF
            {
                OLD_ID = m.MOVIE_INFO_ID,
                NEW_ID = m.ENTITY.MOVIE_INFO_ID,
                TABLE_ID = (int)TableIds.SL_MOVIE_INFO
            }));

            _context.SaveChanges();
        }

        string sql = File.ReadAllText(Path.Join(_dataConfig.DataFolderPath, "sl_movie_info_images.sql"));
        _context.Database.ExecuteSqlRaw(sql);
        _context.SaveChanges();

        int movieInfoImportCount = _context.SL_MOVIE_INFO.Count();
        Console.WriteLine($"Items that were imported: {movieInfoImportCount}");

        Console.WriteLine("----------- Info Import Finished -----------");

    }
}

public class TvInfoImport
{
    public int TV_INFO_ID { get; set; }
    public string SHOW_NAME { get; set; }
    public string SHOW_OVERVIEW { get; set; }
    public int API_TYPE { get; set; }
    public string API_ID { get; set; }
    public DateTime LAST_DATA_REFRESH { get; set; }
    public DateTime LAST_UPDATED { get; set; }
    public string IMAGE_URL { get; set; }
    public SL_TV_INFO ENTITY { get; set; }
}

public class TvEpisodeInfoImport
{
    public int TV_EPISODE_INFO_ID { get; set; }
    public int TV_INFO_ID { get; set; }
    public string? SEASON_NAME { get; set; }
    public string? EPISODE_NAME { get; set; }
    public int? SEASON_NUMBER { get; set; }
    public int? EPISODE_NUMBER { get; set; }
    public string? EPISODE_OVERVIEW { get; set; }
    public int? RUNTIME { get; set; }
    public DateTime? AIR_DATE { get; set; }
    public string? IMAGE_URL { get; set; }
    public int API_TYPE { get; set; }
    public string API_ID { get; set; }
    public SL_TV_EPISODE_INFO ENTITY { get; set; }
}

public class MovieInfoImport
{
    public int MOVIE_INFO_ID { get; set; }
    public string MOVIE_NAME { get; set; }
    public string? MOVIE_OVERVIEW { get; set; }
    public int? RUNTIME { get; set; }
    public DateTime? AIR_DATE { get; set; }
    public DateTime LAST_DATA_REFRESH { get; set; }
    public DateTime LAST_UPDATED { get; set; }
    public string? IMAGE_URL { get; set; }
    public int API_TYPE { get; set; }
    public string API_ID { get; set; }
    public SL_MOVIE_INFO ENTITY { get; set; }

}
