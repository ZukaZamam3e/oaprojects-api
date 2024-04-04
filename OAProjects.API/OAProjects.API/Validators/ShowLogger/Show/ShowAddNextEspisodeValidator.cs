﻿using FluentValidation;
using OAProjects.API.Requests.Show;

namespace OAProjects.API.Validators.ShowLogger.Show;

public class ShowAddNextEspisodeValidator: AbstractValidator<ShowAddNextEpisodeRequest>
{
    public ShowAddNextEspisodeValidator()
    {
        RuleFor(m => m.ShowId).GreaterThan(0);
        RuleFor(m => m.DateWatched).NotEmpty();
    }
}
