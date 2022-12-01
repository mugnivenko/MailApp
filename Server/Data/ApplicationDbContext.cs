using Microsoft.EntityFrameworkCore;

using MailApp.Models;

namespace MailApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Mail> Mails { get; set; }
    public DbSet<User> Users { get; set; }
    public DbSet<MailFromUserToUser> MailFromUserToUser { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        builder.Entity<MailFromUserToUser>(entity =>
        {
            entity
                .HasOne(bc => bc.FromUser)
                .WithMany(bc => bc.MailUsersToUser)
                .HasForeignKey(bc => bc.FromUserId);
            entity
                .HasOne(bc => bc.ToUser)
                .WithMany(bc => bc.MailFromUsersUser)
                .HasForeignKey(bc => bc.ToUserId);
            entity
                .HasOne(bc => bc.Mail);
        });

        base.OnModelCreating(builder);
    }
}
