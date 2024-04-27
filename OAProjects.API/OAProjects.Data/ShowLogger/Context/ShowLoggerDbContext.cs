using Microsoft.EntityFrameworkCore;
using OAProjects.Data.OAIdentity.Entities;
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
    public DbSet<SL_WATCHLIST> SL_WATCHLIST { get; set; }
    public DbSet<SL_TV_INFO> SL_TV_INFO { get; set; }
    public DbSet<SL_TV_EPISODE_INFO> SL_TV_EPISODE_INFO { get; set; }
    public DbSet<SL_MOVIE_INFO> SL_MOVIE_INFO { get; set; }
    public DbSet<SL_TRANSACTION> SL_TRANSACTION { get; set; }
    public DbSet<SL_ID_XREF> SL_ID_XREF { get; set; }
    public DbSet<SL_BOOK> SL_BOOK { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<SL_SHOW>().HasKey(m => m.SHOW_ID);
        modelBuilder.Entity<SL_CODE_VALUE>().HasKey(m => m.CODE_VALUE_ID);
        modelBuilder.Entity<SL_USER_PREF>().HasKey(m => m.USER_PREF_ID);
        modelBuilder.Entity<SL_FRIEND_REQUEST>().HasKey(m => m.FRIEND_REQUEST_ID);
        modelBuilder.Entity<SL_FRIEND>().HasKey(m => m.FRIEND_ID);
        modelBuilder.Entity<SL_WATCHLIST>().HasKey(m => m.WATCHLIST_ID);
        modelBuilder.Entity<SL_TV_INFO>().HasKey(m => m.TV_INFO_ID);
        modelBuilder.Entity<SL_TV_EPISODE_INFO>().HasKey(m => m.TV_EPISODE_INFO_ID);
        modelBuilder.Entity<SL_MOVIE_INFO>().HasKey(m => m.MOVIE_INFO_ID);
        modelBuilder.Entity<SL_TRANSACTION>().HasKey(m => m.TRANSACTION_ID);
        modelBuilder.Entity<SL_ID_XREF>().HasKey(m => m.ID_XREF_ID);
        modelBuilder.Entity<SL_BOOK>().HasKey(m => m.BOOK_ID);

        modelBuilder.Entity<SL_TV_INFO>().HasMany(m => m.EPISODE_INFOS)
            .WithOne(m => m.TV_INFO)
            .HasForeignKey(m => m.TV_INFO_ID);

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
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.ALIST, DECODE_TXT = "AMC A-list" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.BENEFITS, DECODE_TXT = "Benefits" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.REWARDS, DECODE_TXT = "Rewards" },
            new SL_CODE_VALUE { CODE_TABLE_ID = (int)CodeTableIds.TRANSACTION_TYPE_ID, CODE_VALUE_ID = (int)CodeValueIds.TAX, DECODE_TXT = "Tax" }
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

        modelBuilder.Entity<SL_WATCHLIST>(entity =>
        {
            entity.Property(e => e.WATCHLIST_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SHOW_NAME)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.SHOW_NOTES)
                .HasMaxLength(250);
        });

        modelBuilder.Entity<SL_TV_INFO>(entity =>
        {
            entity.Property(e => e.TV_INFO_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SHOW_NAME)
                .IsRequired()
                .HasMaxLength(100);

            entity.Property(e => e.SHOW_OVERVIEW)
                .HasMaxLength(4000)
                .IsRequired();

            entity.Property(e => e.LAST_DATA_REFRESH)
                .IsRequired();

            entity.Property(e => e.LAST_UPDATED)
                .IsRequired();

            entity.Property(e => e.API_ID)
                .HasMaxLength(25);

            entity.Property(e => e.STATUS)
                .HasMaxLength(25);
        });

        modelBuilder.Entity<SL_TV_EPISODE_INFO>(entity =>
        {
            entity.Property(e => e.TV_EPISODE_INFO_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.SEASON_NAME)
                .HasMaxLength(100);

            entity.Property(e => e.EPISODE_NAME)
                .HasMaxLength(500);

            entity.Property(e => e.EPISODE_OVERVIEW)
                .HasMaxLength(4000);

            entity.Property(e => e.API_ID)
                .HasMaxLength(25);

        });

        modelBuilder.Entity<SL_MOVIE_INFO>(entity =>
        {
            entity.Property(e => e.MOVIE_INFO_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.MOVIE_NAME)
                .HasMaxLength(500);

            entity.Property(e => e.MOVIE_OVERVIEW)
                .HasMaxLength(4000)
                .IsRequired();

            entity.Property(e => e.API_ID)
                .HasMaxLength(25);

            entity.Property(e => e.LAST_DATA_REFRESH)
                .IsRequired();

            entity.Property(e => e.LAST_UPDATED)
                .IsRequired();
        });

        modelBuilder.Entity<SL_TRANSACTION>(entity =>
        {
            entity.Property(e => e.TRANSACTION_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.TRANSACTION_TYPE_ID)
                .IsRequired();

            entity.Property(e => e.ITEM)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.COST_AMT)
                .IsRequired();

            entity.Property(e => e.TRANSACTION_NOTES)
                .HasMaxLength(250);

            entity.Property(e => e.TRANSACTION_DATE)
                .IsRequired();
        });

        modelBuilder.Entity<SL_ID_XREF>(entity =>
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

        modelBuilder.Entity<SL_BOOK>(entity =>
        {
            entity.Property(e => e.BOOK_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.BOOK_NAME)
                .HasMaxLength(100)
                .IsRequired();

            entity.Property(e => e.BOOK_NOTES)
                .HasMaxLength(250);
        });
    }
}
