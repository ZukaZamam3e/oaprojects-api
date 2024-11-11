﻿using OAProjects.Models.FinanceTracker.Models;

namespace OAProjects.Models.FinanceTracker.Responses.Transaction;
public class FTTransactionGetResponse
{
    public IEnumerable<FTTransactionViewModel> Transactions { get; set; }

    public int Count { get; set; }
}
