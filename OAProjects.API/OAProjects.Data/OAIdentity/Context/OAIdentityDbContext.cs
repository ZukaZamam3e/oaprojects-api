using Microsoft.EntityFrameworkCore;
using OAProjects.Data.OAIdentity.Entities;
using OAProjects.Data.ShowLogger.Entities;

namespace OAProjects.Data.OAIdentity.Context;
public class OAIdentityDbContext : DbContext
{
    public OAIdentityDbContext(DbContextOptions<OAIdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<OA_USER> OA_USER { get; set; }
    public DbSet<OA_USER_TOKEN> OA_USER_TOKEN { get; set; }
    public DbSet<OA_ID_XREF> OA_ID_XREF { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OA_USER>().HasKey(m => m.USER_ID);
        modelBuilder.Entity<OA_USER_TOKEN>().HasKey(m => m.USER_TOKEN_ID);
        modelBuilder.Entity<OA_ID_XREF>().HasKey(m => m.ID_XREF_ID);

        modelBuilder.Entity<OA_USER>(entity =>
        {
            entity.Property(e => e.USER_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.DATE_ADDED)
                .IsRequired();
        });

        modelBuilder.Entity<OA_USER_TOKEN>(entity =>
        {
            entity.Property(e => e.USER_TOKEN_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.USER_ID)
                .IsRequired();

            entity.Property(e => e.TOKEN)
                .IsRequired();

            entity.Property(e => e.EXPIRY_TIME)
                .IsRequired();

            entity.Property(e => e.ISSUED_AT)
                .IsRequired();

            entity.Property(e => e.EXPIRY_DATE_UTC)
                .IsRequired();

            entity.Property(e => e.ISSUED_AT_DATE_UTC)
                .IsRequired();
        });

        modelBuilder.Entity<OA_ID_XREF>(entity =>
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
