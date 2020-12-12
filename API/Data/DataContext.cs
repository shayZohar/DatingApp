using API.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace API.Data
{
    public class DataContext : IdentityDbContext<AppUser,AppRole, int, 
            IdentityUserClaim<int>, AppUserRole, IdentityUserLogin<int>,IdentityRoleClaim<int>,IdentityUserToken<int>>
    {
        public DataContext(DbContextOptions options) : base(options)
        {

        }

        // the name of the table. The Users table is provided by indentitydbcontext so we dont need to declare it here
        public DbSet<UserLike> Likes { get; set; }
        public DbSet<Message> Messages { get; set; }

        protected override void OnModelCreating(ModelBuilder builder)
        {
            base.OnModelCreating(builder);

            // setting the relations of many to many for user and role fro, the IdentityDbContext
            builder.Entity<AppUser>().HasMany(ur => ur.UserRoles).WithOne(u => u.User).HasForeignKey(ur => ur.UserId).IsRequired();
            builder.Entity<AppRole>().HasMany(ur => ur.UserRoles).WithOne(u => u.Role).HasForeignKey(ur => ur.RoleId).IsRequired();

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