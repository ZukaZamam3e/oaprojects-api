using Microsoft.EntityFrameworkCore;
using OAProjects.Data.ShowLogger.Entities;

namespace OAProjects.Data.ShowLogger.Context;

public class ShowLoggerDbContext : DbContext
{
    public ShowLoggerDbContext(DbContextOptions<ShowLoggerDbContext> options)
        : base(options)
    {
    }

    public DbSet<SL_SHOW> SL_SHOW { get; set; }
    public DbSet<SL_CODE_VALUE> SL_CODE_VALUE { get; set; }
    public DbSet<SL_USER_PREF> SL_USER_PREF { get; set; }
    public DbSet<SL_FRIEND> SL_FRIEND { get; set; }
    public DbSet<SL_FRIEND_REQUEST> SL_FRIEND_REQUEST { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SL_SHOW>().HasKey(m => m.SHOW_ID);
        modelBuilder.Entity<SL_CODE_VALUE>().HasKey(m => m.CODE_VALUE_ID);
        modelBuilder.Entity<SL_USER_PREF>().HasKey(m => m.USER_PREF_ID);
        modelBuilder.Entity<SL_FRIEND_REQUEST>().HasKey(m => m.FRIEND_REQUEST_ID);
        modelBuilder.Entity<SL_FRIEND>().HasKey(m => m.FRIEND_ID);

        modelBuilder.Entity<SL_CODE_VALUE>(entity =>
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

        modelBuilder.Entity<SL_CODE_VALUE>().HasData(
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.SHOW_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.TV, DECODE_TXT = "TV" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.SHOW_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.MOVIE, DECODE_TXT = "Movie" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.SHOW_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.AMC, DECODE_TXT = "AMC" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.ALIST_TICKET, DECODE_TXT = "A-list Ticket" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.TICKET, DECODE_TXT = "Ticket" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.PURCHASE, DECODE_TXT = "Purchase" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.ALIST, DECODE_TXT = "AMC A-list" }
        );

        modelBuilder.Entity<SL_SHOW>(entity =>
        {
            entity.Property(e => e.SHOW_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SHOW_NAME)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.SHOW_NOTES)
                .HasMaxLength(250);
        });

        modelBuilder.Entity<SL_USER_PREF>(entity =>
        {
            entity.Property(e => e.USER_PREF_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.DEFAULT_AREA)
                .HasMaxLength(20)
                .IsRequired();
        });

        modelBuilder.Entity<SL_FRIEND_REQUEST>(entity =>
        {
            entity.Property(e => e.FRIEND_REQUEST_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SENT_USER_ID)
                .IsRequired();

            entity.Property(e => e.RECEIVED_USER_ID)
                .IsRequired();

            entity.Property(e => e.DATE_SENT)
                .IsRequired();
        });

        modelBuilder.Entity<SL_FRIEND>(entity =>
        {
            entity.Property(e => e.FRIEND_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.FRIEND_USER_ID)
                .IsRequired();

            entity.Property(e => e.CREATED_DATE)
                .IsRequired();
        });
    }
}
