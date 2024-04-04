using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Context;
using OAProjects.Data.ShowLogger.Entities;
using OAProjects.Models.ShowLogger.Models.Config;
using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;
using OAProjects.Store.ShowLogger.Stores.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores;
public class UnlinkedShowStore : IUnlinkedShowStore
{
    private readonly ShowLoggerDbContext _context;

    public UnlinkedShowStore(ShowLoggerDbContext context,
        ApisConfig apisConfig)
    {
        _context = context;
    }

    public IEnumerable<UnlinkedShowModel> GetUnlinkedShows(Expression<Func<UnlinkedShowModel, bool>>? predicate = null)
    {
        Dictionary<int, string> showTypeIds = _context.SL_CODE_VALUE.Where(m => m.CODE_TABLE_ID == (int)CodeTableIds.SHOW_TYPE_ID).ToDictionary(m => m.CODE_VALUE_ID, m => m.DECODE_TXT);
        SL_SHOW[] data = _context.SL_SHOW.Where(m => m.INFO_ID == null).ToArray();
        SL_TV_INFO[] tvData = _context.SL_TV_INFO.ToArray();
        SL_MOVIE_INFO[] movieData = _context.SL_MOVIE_INFO.ToArray();

        List<UnlinkedShowModel> query = new List<UnlinkedShowModel>();

        IEnumerable<UnlinkedShowModel> groupData = (from d in data
                                                    group d by new { d.SHOW_NAME, d.SHOW_TYPE_ID } into grp
                                                    select new UnlinkedShowModel
                                                    {
                                                        ShowName = grp.Key.SHOW_NAME,
                                                        ShowTypeId = grp.Key.SHOW_TYPE_ID,
                                                        ShowTypeIdZ = showTypeIds[grp.Key.SHOW_TYPE_ID],
                                                        WatchCount = grp.Count(),
                                                        LastWatched = grp.Max(m => m.DATE_WATCHED),
                                                        //InfoId = grp.Key.SHOW_TYPE_ID == (int)CodeValueIds.TV ? tvData.ContainsKey(grp.Key.SHOW_NAME) ? tvData[grp.Key.SHOW_NAME] : -1 : movieData.ContainsKey(grp.Key.SHOW_NAME) ? movieData[grp.Key.SHOW_NAME] : -1,
                                                        //InShowLoggerIndc = grp.Key.SHOW_TYPE_ID == (int)CodeValueIds.TV ? tvData.ContainsKey(grp.Key.SHOW_NAME) : movieData.ContainsKey(grp.Key.SHOW_NAME)
                                                    });


        foreach (UnlinkedShowModel group in groupData)
        {
            if (group.ShowTypeId == (int)CodeValueIds.TV)
            {
                IEnumerable<UnlinkedShowModel> unlinked = tvData.Where(m => m.SHOW_NAME == group.ShowName).Select(m => new UnlinkedShowModel
                {
                    ShowName = group.ShowName,
                    ShowTypeId = group.ShowTypeId,
                    ShowTypeIdZ = group.ShowTypeIdZ,
                    WatchCount = group.WatchCount,
                    LastWatched = group.LastWatched,
                    LastDataRefresh = m.LAST_DATA_REFRESH,
                    InfoId = m.TV_INFO_ID,
                    InShowLoggerIndc = true
                });

                if (unlinked.Any())
                {
                    query.AddRange(unlinked);
                }
                else
                {
                    query.Add(new UnlinkedShowModel
                    {
                        ShowName = group.ShowName,
                        ShowTypeId = group.ShowTypeId,
                        ShowTypeIdZ = group.ShowTypeIdZ,
                        WatchCount = group.WatchCount,
                        LastWatched = group.LastWatched,
                        InShowLoggerIndc = false
                    });
                }
            }
            else
            {
                IEnumerable<UnlinkedShowModel> unlinked = movieData.Where(m => m.MOVIE_NAME == group.ShowName).Select(m => new UnlinkedShowModel
                {
                    ShowName = group.ShowName,
                    ShowTypeId = group.ShowTypeId,
                    ShowTypeIdZ = group.ShowTypeIdZ,
                    WatchCount = group.WatchCount,
                    LastWatched = group.LastWatched,
                    AirDate = m.AIR_DATE,
                    LastDataRefresh = m.LAST_DATA_REFRESH,
                    InfoId = m.MOVIE_INFO_ID,
                    InShowLoggerIndc = true
                });

                if (unlinked.Any())
                {
                    query.AddRange(unlinked);
                }
                else
                {
                    query.Add(new UnlinkedShowModel
                    {
                        ShowName = group.ShowName,
                        ShowTypeId = group.ShowTypeId,
                        ShowTypeIdZ = group.ShowTypeIdZ,
                        WatchCount = group.WatchCount,
                        LastWatched = group.LastWatched,
                        InShowLoggerIndc = false
                    });
                }
            }
        }

        if (predicate != null)
        {
            query = query.AsQueryable().Where(predicate).ToList();
        }

        return query;
    }

    public bool UpdateShowNames(UpdateUnlinkedNameModel model)
    {
        IEnumerable<SL_SHOW> shows = _context.SL_SHOW.Where(m => m.SHOW_NAME == model.ShowName && m.SHOW_TYPE_ID == model.ShowTypeId);

        foreach (SL_SHOW show in shows)
        {
            show.SHOW_NAME = model.NewShowName;
        }

        _context.SaveChanges();
        return true;
    }

    public bool LinkShows(LinkShowModel model)
    {
        IEnumerable<SL_SHOW> shows = _context.SL_SHOW.Where(m => m.SHOW_NAME == model.ShowName && m.SHOW_TYPE_ID == model.ShowTypeId).ToList();

        if (model.ShowTypeId == (int)CodeValueIds.TV)
        {
            foreach (SL_SHOW show in shows)
            {
                int? infoId = GetTvEpisodeInfoId(model.ShowName, show.SEASON_NUMBER, show.EPISODE_NUMBER);

                show.INFO_ID = infoId;
            }
        }
        else
        {
            foreach (SL_SHOW show in shows)
            {
                show.INFO_ID = model.InfoId;
            }
        }

        _context.SaveChanges();
        return true;
    }

    private int? GetTvEpisodeInfoId(string name, int? seasonNumber, int? episodeNumber)
    {
        int? infoId = null;
        Dictionary<string, SL_TV_INFO> lookUp = new();

        SL_TV_INFO[] shows = _context.SL_TV_INFO.ToArray();

        foreach (SL_TV_INFO info in shows)
        {
            lookUp.Add(info.SHOW_NAME.ToLower(), info);
        }

        //SL_TV_INFO? show = _context.SL_TV_INFO.FirstOrDefault(m => m.SHOW_NAME.ToLower() == name.ToLower() || m.)

        SL_TV_INFO found;

        if (lookUp.TryGetValue(name.ToLower(), out found))
        {
            SL_TV_EPISODE_INFO? episodeInfo = _context.SL_TV_EPISODE_INFO.FirstOrDefault(m => m.TV_INFO_ID == found.TV_INFO_ID && m.SEASON_NUMBER == seasonNumber && m.EPISODE_NUMBER == episodeNumber);

            if (episodeInfo == null && episodeNumber != null)
            {
                // Somes shows I track are by episode and ignoring season. 
                // So get all episodes and then find the row that matches the episode
                episodeInfo = _context.SL_TV_EPISODE_INFO.Where(m => m.TV_INFO_ID == found.TV_INFO_ID && m.SEASON_NUMBER > 0)
                    .OrderBy(m => m.SEASON_NUMBER).ThenBy(m => m.EPISODE_NUMBER)
                    .Skip(episodeNumber.Value - 1).FirstOrDefault();
            }

            if (episodeInfo != null)
            {
                infoId = episodeInfo.TV_EPISODE_INFO_ID;
            }
        }

        return infoId;
    }
}
