﻿using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.WatchList;

namespace OAProjects.API.Responses.WatchList;

public class WatchListLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> ShowTypeIds { get; set; }

    public IEnumerable<WatchListModel> WatchLists { get; set; }

    public int Count { get; set; }
}
