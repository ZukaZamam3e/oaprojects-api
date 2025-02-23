using Microsoft.EntityFrameworkCore;
using OAProjects.Data.FinanceTracker.Entities;
using OAProjects.Data.ShowLogger.Entities;

namespace OAProjects.Data.FinanceTracker.Context;
public class FinanceTrackerDbContext(DbContextOptions<FinanceTrackerDbContext> options) : DbContext(options)
{
    public DbSet<FT_CODE_VALUE> FT_CODE_VALUE { get; set; }
    public DbSet<FT_ACCOUNT> FT_ACCOUNT { get; set; }
    public DbSet<FT_TRANSACTION> FT_TRANSACTION { get; set; }
    public DbSet<FT_TRANSACTION_OFFSET> FT_TRANSACTION_OFFSET { get; set; }
    public DbSet<FT_ID_XREF> FT_ID_XREF { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<FT_ACCOUNT>().HasKey(m => m.ACCOUNT_ID);
        modelBuilder.Entity<FT_TRANSACTION>().HasKey(m => m.TRANSACTION_ID);
        modelBuilder.Entity<FT_TRANSACTION_OFFSET>().HasKey(m => m.TRANSACTION_OFFSET_ID);
        modelBuilder.Entity<FT_CODE_VALUE>().HasKey(m => m.CODE_VALUE_ID);
        modelBuilder.Entity<FT_ID_XREF>().HasKey(m => m.ID_XREF_ID);

        modelBuilder.Entity<FT_CODE_VALUE>().HasData(
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.HARDSET, DECODE_TXT = "Hardset" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.SINGLE, DECODE_TXT = "Single" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.DAILY, DECODE_TXT = "Daily" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.WEEKLY, DECODE_TXT = "Weekly" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.BIWEEKLY, DECODE_TXT = "Bi-Weekly" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.EVERFOURWEEKS, DECODE_TXT = "Every Four Weeks" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.MONTHLY, DECODE_TXT = "Monthly" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.QUARTERLY, DECODE_TXT = "Quarterly" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.YEARLY, DECODE_TXT = "Yearly" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.EVERY_N_DAYS, DECODE_TXT = "Every N Days" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.EVERY_N_WEEKS, DECODE_TXT = "Every N Weeks" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.FREQUENCY_TYPES, CODE_VALUE_ID = (int)FT_CodeValueIds.EVERY_N_MONTHS, DECODE_TXT = "Every N Months" },
            new FT_CODE_VALUE { CODE_TABLE_ID = (int)FT_CodeTableIds.CONDITIONAL, CODE_VALUE_ID = (int)FT_CodeValueIds.OCCURS_THREE_MONTH, DECODE_TXT = "3/Month" }
        );

        modelBuilder.Entity<FT_CODE_VALUE>(entity =>
        {
            entity.Property(e => e.CODE_TABLE_ID)
                .ValueGeneratedNever();

            entity.Property(e => e.CODE_VALUE_ID)
                .ValueGeneratedNever();

            entity.Property(e => e.DECODE_TXT)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.EXTRA_INFO)
                .HasMaxLength(100);
        });

        modelBuilder.Entity<FT_ACCOUNT>(entity =>
        {
            entity.Property(e => e.ACCOUNT_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.ACCOUNT_NAME)
                .HasMaxLength(30);

            entity.Property(e => e.DEFAULT_INDC)
                .IsRequired();
        });

        modelBuilder.Entity<FT_TRANSACTION>(entity =>
        {
            entity.Property(e => e.TRANSACTION_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ACCOUNT_ID)
                .IsRequired();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.TRANSACTION_NAME)
                .IsRequired()
                .HasMaxLength(30);

            entity.Property(e => e.START_DATE)
                .IsRequired();

            entity.Property(e => e.TRANSACTION_AMOUNT)
                .IsRequired();

            entity.Property(e => e.FREQUENCY_TYPE_ID)
                .IsRequired();

            entity.Property(e => e.TRANSACTION_NOTES)
                .HasMaxLength(250);

            entity.Property(e => e.TRANSACTION_URL)
                .HasMaxLength(250);

            entity.Property(e => e.CATEGORIES)
                .HasMaxLength(250);
        });

        modelBuilder.Entity<FT_TRANSACTION_OFFSET>(entity =>
        {
            entity.Property(e => e.TRANSACTION_OFFSET_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.ACCOUNT_ID)
                .IsRequired();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.TRANSACTION_ID)
                .IsRequired();

            entity.Property(e => e.OFFSET_AMOUNT)
                .IsRequired();

            entity.Property(e => e.OFFSET_DATE)
                .IsRequired();
        });

        modelBuilder.Entity<FT_ID_XREF>(entity =>
        {
            entity.Property(e => e.ID_XREF_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.OLD_ID)
                .IsRequired();

            entity.Property(e => e.TABLE_ID)
                .IsRequired();

            entity.Property(e => e.NEW_ID)
                .IsRequired();
        });
    }
}
