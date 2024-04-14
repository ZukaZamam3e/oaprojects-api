using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Import.Imports;

public static class ImportFiles
{
    public const string sl_book = "sl_book.json";
    public const string sl_friend = "sl_friend.json";
    public const string sl_friend_request = "sl_friend_request.json";
    public const string sl_movie_info = "sl_movie_info.json";
    public const string sl_show = "sl_show.json";
    public const string sl_transaction = "sl_transaction.json";
    public const string sl_tv_episode_info = "sl_tv_episode_info.json";
    public const string sl_tv_info = "sl_tv_info.json";
    public const string sl_user_pref = "sl_user_pref.json";
    public const string sl_watchlist = "sl_watchlist.json";

    public const string oa_users = "oa_users.json";
}

public interface IImport
{
    void RunImport();
}
