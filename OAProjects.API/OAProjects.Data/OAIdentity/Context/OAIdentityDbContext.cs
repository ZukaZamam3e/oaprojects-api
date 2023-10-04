using Microsoft.EntityFrameworkCore;
using OAProjects.Data.OAIdentity.Entities;

namespace OAProjects.Data.OAIdentity.Context;
public class OAIdentityDbContext : DbContext
{
    public OAIdentityDbContext(DbContextOptions<OAIdentityDbContext> options)
        : base(options)
    {
    }

    public DbSet<OA_USER> OA_USER { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<OA_USER>().HasKey(m => m.USER_ID);

        modelBuilder.Entity<OA_USER>(entity =>
        {
            entity.Property(e => e.USER_ID)
                .ValueGeneratedOnAdd();

            entity.Property(e => e.USER_GUID)
                .IsRequired();

            entity.Property(e => e.DATE_ADDED)
                .IsRequired();
        });
    }
}
