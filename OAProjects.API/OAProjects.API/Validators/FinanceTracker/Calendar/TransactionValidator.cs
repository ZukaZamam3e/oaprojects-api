﻿using FluentValidation;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Models.FinanceTracker.Models;
using OAProjects.Models.ShowLogger.Models.Show;

namespace OAProjects.API.Validators.FinanceTracker.Calendar;

public class TransactionValidator : AbstractValidator<FTTransactionModel>
{
    public TransactionValidator()
    {
        RuleFor(m => m.Name).NotNull();

        RuleFor(m => m.Amount).NotEqual(0);
        RuleFor(x => x.StartDate).GreaterThan(DateTime.MinValue);

        RuleFor(m => m.StartDate).LessThan(m => m.EndDate).When(m => m.EndDate.HasValue);

        RuleFor(m => m.FrequencyTypeId).GreaterThan(0);

        RuleFor(m => m.OffsetDate).GreaterThanOrEqualTo(m => m.StartDate);
        RuleFor(m => m.OffsetDate).LessThanOrEqualTo(m => m.EndDate).When(m => m.EndDate.HasValue);

        RuleFor(m => m.Interval).NotNull()
            .When(m => m.FrequencyTypeId == (int)FT_CodeValueIds.EVERY_N_DAYS
                || m.FrequencyTypeId == (int)FT_CodeValueIds.EVERY_N_WEEKS
                || m.FrequencyTypeId == (int)FT_CodeValueIds.EVERY_N_MONTHS);

        RuleFor(m => m.ConditionalAmount).NotNull()
            .When(m => m.Conditional != null);

        RuleFor(m => m.Conditional).NotNull()
            .When(m => m.ConditionalAmount != null);
    }
}
