﻿using OAProjects.Models.ShowLogger.Models.CodeValue;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Responses.ShowLogger.Show;

public class ShowLoadResponse
{
    public IEnumerable<SLCodeValueSimpleModel> ShowTypeIds { get; set; }

    public IEnumerable<ShowModel> Shows { get; set; }

    public int Count { get; set; }
}