﻿namespace OAProjects.Models.FinanceTracker.Models;
public class DayTransactionModel
{
    public TransactionModel Transaction { get; set; }

    public TransactionOffsetModel TransactionOffset { get; set; }
}