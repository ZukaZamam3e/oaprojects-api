﻿namespace OAProjects.Data.FinanceTracker.Entities;
public class FT_CODE_VALUE
{
    public int CODE_TABLE_ID { get; set; }

    public int CODE_VALUE_ID { get; set; }

    public string DECODE_TXT { get; set; }

    public string? EXTRA_INFO { get; set; }
}

public enum FT_CodeTableIds
{
    FREQUENCY_TYPES = 1,
    CONDITIONAL = 2,
}

public enum FT_CodeValueIds
{
    HARDSET = 1000,
    SINGLE = 1001,
    DAILY = 1002,
    WEEKLY = 1003,
    BIWEEKLY = 1004,
    EVERFOURWEEKS = 1005,
    MONTHLY = 1006,
    QUARTERLY = 1007,
    YEARLY = 1008,
    EVERY_N_DAYS = 1009,
    EVERY_N_WEEKS = 1010,
    EVERY_N_MONTHS = 1011,
    OCCURS_THREE_MONTH = 2000

}