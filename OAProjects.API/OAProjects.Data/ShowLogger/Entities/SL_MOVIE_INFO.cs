﻿using System.ComponentModel.DataAnnotations.Schema;

namespace OAProjects.Data.ShowLogger.Entities;
public class SL_MOVIE_INFO
{
    [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
    public int MOVIE_INFO_ID { get; set; }

    public string MOVIE_NAME { get; set; }

    public string? MOVIE_OVERVIEW { get; set; }

    public int? API_TYPE { get; set; }

    public string? API_ID { get; set; }

    public int? RUNTIME { get; set; }

    public DateTime? AIR_DATE { get; set; }

    public DateTime LAST_DATA_REFRESH { get; set; }

    public DateTime LAST_UPDATED { get; set; }

    public string? POSTER_URL { get; set; }

    public string? BACKDROP_URL { get; set; }

    public string? KEYWORDS { get; set; }
}
