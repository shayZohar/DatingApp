using API.Entities;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : DbContext
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // "Users" is the name of the table
        public DbSet<AppUser> Users { get; set; }
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            builder.Entity<UserLike>()
            .HasKey(keyExpression=> new {keyExpression.SourceUserId,keyExpression.LikedUserId});

            //source user can like many users
            builder.Entity<UserLike>().HasOne(s =>s.SourceUser)
            .WithMany(l => l.LikedUsers)
            .HasForeignKey(s =>s.SourceUserId)
            .OnDelete(DeleteBehavior.Cascade); // if we delete the user, we delete the related entities

            //source user can be liked by many users
            builder.Entity<UserLike>().HasOne(s =>s.LikedUser)
            .WithMany(l => l.LikedByUsers)
            .HasForeignKey(s =>s.LikedUserId)
            .OnDelete(DeleteBehavior.Cascade); // if we delete the user, we delete the related entities

            //for the receiver
            builder.Entity<Message>()
            .HasOne(u => u.Recipent)
            .WithMany(m => m.MessagesReceived)
            .OnDelete(DeleteBehavior.Restrict);

            //for the sender
            builder.Entity<Message>()
            .HasOne(u => u.Sender)
            .WithMany(m => m.MessagesSent)
            .OnDelete(DeleteBehavior.Restrict);
        }
    }
}