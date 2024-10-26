using JwtCookiesScheme.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using System;
using System.Security;

namespace JwtCookiesScheme
{
    public class DatabaseContext:DbContext
    {
        public DatabaseContext(DbContextOptions<DatabaseContext> options) : base(options) { }
        public DbSet<User> Users { get; set; }
        public DbSet<Role> Roles { get; set; }
        public DbSet<Lockout> Lockouts { get; set; }

        public DbSet<Permission> Permissions {  get; set; }
        public DbSet<RolePermissions> RolePermissions { get; set; }
        public DbSet<ResetToken> ResetTokens { get; set; }
        public override async Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        {
            AddTimestamps();
            return await base.SaveChangesAsync();
        }
        private void AddTimestamps()
        {
            var entries = ChangeTracker.Entries<TimeBase>();

            foreach (var entry in entries)
            {
                if (entry.State == EntityState.Added)
                {
                    entry.Entity.CreatedAt = DateTime.UtcNow;
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                }
                else if (entry.State == EntityState.Modified)
                {
                    entry.Entity.LastModifiedAt = DateTime.UtcNow;
                }
            }
        }
        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);
            modelBuilder.Entity<Lockout>(entity =>
            {
                entity.HasKey(x => x.Id);
                entity.HasIndex(x => x.SsId).IsUnique();
            });
            modelBuilder.Entity<User>(entity =>
            {
                entity.HasKey(u => u.UserId);


                entity.Property(u => u.UserId)
                    .IsRequired();

                entity.Property(u => u.UserName)
                    .HasMaxLength(50)
                    .IsRequired();

                entity.Property(u => u.UserEmail)
                    .HasMaxLength(100)
                    .IsRequired();


                entity.HasOne(u => u.UserRole)
                    .WithMany(r => r.Users)
                    .HasForeignKey(u => u.UserRoleId) // Foreign key for UserRole
                    .OnDelete(DeleteBehavior.Restrict); // Configure delete behavior

                entity.HasOne(u => u.UserToken).WithOne(t => t.User).HasForeignKey<ResetToken>(t => t.UserId);

                var user = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserEmail = "Admin@gmail.com",
                    UserName = "Lapphan",
                    UserRoleId = "Admin",
                };
                var ownerUser = new User
                {
                    UserId = Guid.NewGuid().ToString(),
                    UserEmail = "Owner@gmail.com",
                    UserName = "Lapphan",
                    UserRoleId = "Owner",
                };
                user.UserPassword = new PasswordHasher<User>().HashPassword(user, "adminpassword");
                ownerUser.UserPassword = new PasswordHasher<User>().HashPassword(ownerUser, "adminpassword");
                entity.HasData([user, ownerUser]);
            });
            modelBuilder.Entity<Role>(entity =>
            {
                entity.HasKey(r => r.RoleId);

                entity.Property(r => r.RoleId)
                      .IsRequired();

                entity.Property(r => r.RoleName)
                      .HasMaxLength(50)
                      .IsRequired();
                entity.HasIndex(r => r.RoleName)
                    .IsUnique();
                var roles = new List<Role>();
                roles.Add(new Role { RoleId = "Admin", RoleName = "Admin" });
                roles.Add(new Role { RoleId = "Owner", RoleName = "Owner" });
                roles.Add(new Role { RoleId = "Guest", RoleName = "Guest" });
                roles.Add(new Role { RoleId = "User", RoleName = "User" });
                entity.HasData(roles);
            });
            modelBuilder.Entity<Permission>(entity =>
            {
                entity.HasKey(p => p.PermissionId);
                entity.Property(p => p.PermissionName).HasMaxLength(50).IsRequired();
                var permisisons = new List<Permission>();
                permisisons.Add(new Permission { PermissionId = "Read", PermissionName = "Read" });
                permisisons.Add(new Permission { PermissionId = "Write", PermissionName = "Write" });
                permisisons.Add(new Permission { PermissionId = "Delete", PermissionName = "Delete" });
                permisisons.Add(new Permission { PermissionId = "Execute", PermissionName = "Execute" });
                permisisons.Add(new Permission { PermissionId = "FullPermissions", PermissionName = "FullPermissions" });
                entity.HasData(permisisons);
            });
            modelBuilder.Entity<RolePermissions>(entity => {
                entity.HasKey(rp => new { rp.RoleId, rp.PermissionId });
                entity.HasOne(rp => rp.Role).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.RoleId);
                entity.HasOne(rp => rp.Permission).WithMany(r => r.RolePermissions).HasForeignKey(rp => rp.PermissionId);
                var rolePermissions = new List<RolePermissions>();
                rolePermissions.Add(new RolePermissions { RoleId = "Admin", PermissionId = "Read" });
                rolePermissions.Add(new RolePermissions { RoleId = "Admin", PermissionId = "Write" });
                rolePermissions.Add(new RolePermissions { RoleId = "Admin", PermissionId = "Delete" });

                rolePermissions.Add(new RolePermissions { RoleId = "Guest", PermissionId = "Read" });
                rolePermissions.Add(new RolePermissions { RoleId = "User", PermissionId = "Read" });
                rolePermissions.Add(new RolePermissions { RoleId = "User", PermissionId = "Write" });
                rolePermissions.Add(new RolePermissions { RoleId = "Owner", PermissionId = "FullPermissions" });
                entity.HasData(rolePermissions);
            });
            modelBuilder.Entity<ResetToken>(entity =>
            {
                entity.HasKey(t => t.TokenId);
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
