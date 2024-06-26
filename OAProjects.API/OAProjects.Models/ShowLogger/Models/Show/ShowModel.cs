﻿namespace OAProjects.Models.ShowLogger.Models.Show;

public class ShowModel
{
    public int ShowId { get; set; }

    public int UserId { get; set; }

    public string ShowName { get; set; }

    public int ShowTypeId { get; set; }

    public string? ShowTypeIdZ { get; set; }

    public int? SeasonNumber { get; set; }

    public int? EpisodeNumber { get; set; }

    public DateTime DateWatched { get; set; }

    public string? ShowNotes { get; set; }

    public bool RestartBinge { get; set; }

    public int? InfoId { get; set; }

    public IEnumerable<ShowTransactionModel>? Transactions { get; set; }
}
