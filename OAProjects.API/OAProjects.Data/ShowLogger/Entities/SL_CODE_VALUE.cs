namespace OAProjects.Data.ShowLogger.Entities;

public class SL_CODE_VALUE
{
    public int CODE_TABLE_ID { get; set; }

    public int CODE_VALUE_ID { get; set; }

    public string DECODE_TXT { get; set; }

    public string? EXTRA_INFO { get; set; }
}

public enum CodeTableIds
{
    SHOW_TYPE_ID = 1,
    TRANSACTION_TYPE_ID = 2,
}

public enum CodeValueIds
{
    TV = 1000,
    MOVIE = 1001,
    AMC = 1002,

    ALIST_TICKET = 2000,
    TICKET = 2001,
    PURCHASE = 2002,
    ALIST = 2003,

}
