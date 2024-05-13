using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OAProjects.Data.ShowLogger.Entities;

public enum TableIds
{
    SL_CODE_VALUE = 1,
    SL_FRIEND,
    SL_FRIEND_REQUEST,
    SL_MOVIE_INFO,
    SL_SHOW,
    SL_TRANSACTION,
    SL_TV_EPISODE_INFO,
    SL_TV_INFO,
    SL_USER_PREF,
    SL_WATCHLIST,
    SL_BOOK,
    SL_TV_EPISODE_ORDER
}

public class SL_ID_XREF
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int ID_XREF_ID { get; set; }

    public int TABLE_ID { get; set; }

    public int OLD_ID { get; set; }

    public int NEW_ID { get; set; }
}
