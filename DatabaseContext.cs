using JwtCookiesScheme.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security;

namespace JwtCookiesScheme
{
    public class DatabaseContext: IdentityDbContext<User,Role,string,UserClaim,UserRole,UserLogin,RoleClaim,Token>
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<RefreshToken> RefreshTokens { get; set; }

        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            //AddTimestamps();
            return await base.SaveChangesAsync();
        }
        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries().Where(e => e.State == EntityState.Modified || e.State == EntityState.Added);

            foreach (var entry in entries)
            {

                var lastModifiedProperty = entry.Property("LastModified");
                if (lastModifiedProperty != null )
                {
                    entry.Property("LastModified").CurrentValue = DateTime.UtcNow;
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<User>(entity =>
            {
            });


            modelBuilder.Entity<Role>(entity =>
            {
            });
            modelBuilder.Entity<UserRole>(entity =>
            {

                entity.HasOne(ur => ur.User).WithMany(u => u.UserRoles).HasForeignKey(u => u.UserId);
                entity.HasOne(ur => ur.Role).WithMany(u => u.UserRoles).HasForeignKey(u => u.RoleId);

            });
            modelBuilder.Entity<RefreshToken>(entity =>
            {
                entity.HasKey(t => t.Id);
                entity.Property(t => t.isInvoked).HasDefaultValue(false);
                entity.HasOne(t => t.User).WithOne().HasForeignKey<RefreshToken>(t => t.UserId).OnDelete(DeleteBehavior.Cascade);
                entity.Property(rt => rt.TokenExpiredAt).IsRequired();
            });

        }

        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (!optionsBuilder.IsConfigured)
            {
                var configBuilder = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json")
                    .Build();

                var configSection = configBuilder.GetSection("ConnectionStrings");
                var connectionString = configSection["DefaultConnection"];
                optionsBuilder.UseMySql(connectionString, new MySqlServerVersion(new Version(8, 0, 0)));

            }
        }

    }
}
