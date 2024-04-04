using OAProjects.Models.ShowLogger.Models.Info;
using OAProjects.Models.ShowLogger.Models.UnlinkedShow;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Store.ShowLogger.Stores.Interfaces;
public interface IInfoStore
{
    Task<DownloadResultModel> Download(int userId, InfoApiDownloadModel downloadInfo);

    Task<ApiResultModel<IEnumerable<ApiSearchResultModel>>> Search(int userId, InfoApiSearchModel searchInfo);

    IEnumerable<TvInfoModel> GetTvInfos(Expression<Func<TvInfoModel, bool>>? predicate = null);

    IEnumerable<TvInfoSeasonModel> GetTvInfoSeasons(int tvInfoId);

    IEnumerable<TvEpisodeInfoModel> GetTvEpisodeInfos(Expression<Func<TvEpisodeInfoModel, bool>>? predicate = null);

    long RefreshTvInfo(TvInfoModel model);

    long RefreshTvInfo(int infoId);

    Task<DownloadResultModel> RefreshInfo(int userId, int infoId, INFO_TYPE type);

    bool DeleteTvInfo(int userId, int tvInfoId);

    IEnumerable<MovieInfoModel> GetMovieInfos(Expression<Func<MovieInfoModel, bool>>? predicate = null);

    long RefreshMovieInfo(MovieInfoModel model);

    bool DeleteMovieInfo(int userId, int tvInfoId);
}
