﻿// <auto-generated />
using System;
using JwtCookiesScheme;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace JwtCookiesScheme.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    [Migration("20241024075520_Init")]
    partial class Init
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "8.0.0")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("JwtCookiesScheme.Permission", b =>
                {
                    b.Property<string>("PermissionId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PermissionName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("PermissionId");

                    b.ToTable("Permissions");

                    b.HasData(
                        new
                        {
                            PermissionId = "Read",
                            PermissionName = "Read"
                        },
                        new
                        {
                            PermissionId = "Write",
                            PermissionName = "Write"
                        },
                        new
                        {
                            PermissionId = "Delete",
                            PermissionName = "Delete"
                        },
                        new
                        {
                            PermissionId = "Execute",
                            PermissionName = "Execute"
                        },
                        new
                        {
                            PermissionId = "FullPermissions",
                            PermissionName = "FullPermissions"
                        });
                });

            modelBuilder.Entity("JwtCookiesScheme.ResetToken", b =>
                {
                    b.Property<string>("TokenId")
                        .HasColumnType("varchar(255)");

                    b.Property<DateTime>("TokenExpiredAt")
                        .HasColumnType("datetime(6)");

                    b.Property<string>("TokenSerect")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("TokenId");

                    b.HasIndex("UserId")
                        .IsUnique();

                    b.ToTable("ResetTokens");
                });

            modelBuilder.Entity("JwtCookiesScheme.Role", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("RoleName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.HasKey("RoleId");

                    b.HasIndex("RoleName")
                        .IsUnique();

                    b.ToTable("Roles");

                    b.HasData(
                        new
                        {
                            RoleId = "Admin",
                            RoleName = "Admin"
                        },
                        new
                        {
                            RoleId = "Owner",
                            RoleName = "Owner"
                        },
                        new
                        {
                            RoleId = "Guest",
                            RoleName = "Guest"
                        },
                        new
                        {
                            RoleId = "User",
                            RoleName = "User"
                        });
                });

            modelBuilder.Entity("JwtCookiesScheme.RolePermissions", b =>
                {
                    b.Property<string>("RoleId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("PermissionId")
                        .HasColumnType("varchar(255)");

                    b.HasKey("RoleId", "PermissionId");

                    b.HasIndex("PermissionId");

                    b.ToTable("RolePermissions");

                    b.HasData(
                        new
                        {
                            RoleId = "Admin",
                            PermissionId = "Read"
                        },
                        new
                        {
                            RoleId = "Admin",
                            PermissionId = "Write"
                        },
                        new
                        {
                            RoleId = "Admin",
                            PermissionId = "Delete"
                        },
                        new
                        {
                            RoleId = "Guest",
                            PermissionId = "Read"
                        },
                        new
                        {
                            RoleId = "User",
                            PermissionId = "Read"
                        },
                        new
                        {
                            RoleId = "User",
                            PermissionId = "Write"
                        },
                        new
                        {
                            RoleId = "Owner",
                            PermissionId = "FullPermissions"
                        });
                });

            modelBuilder.Entity("JwtCookiesScheme.User", b =>
                {
                    b.Property<string>("UserId")
                        .HasColumnType("varchar(255)");

                    b.Property<string>("UserEmail")
                        .IsRequired()
                        .HasMaxLength(100)
                        .HasColumnType("varchar(100)");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasMaxLength(50)
                        .HasColumnType("varchar(50)");

                    b.Property<string>("UserPassword")
                        .IsRequired()
                        .HasColumnType("longtext");

                    b.Property<string>("UserRoleId")
                        .IsRequired()
                        .HasColumnType("varchar(255)");

                    b.HasKey("UserId");

                    b.HasIndex("UserRoleId");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            UserId = "c97688cf-8d9a-4c16-bdcb-cf0e933095bb",
                            UserEmail = "Admin@gmail.com",
                            UserName = "Lapphan",
                            UserPassword = "AQAAAAIAAYagAAAAEOsdJgMiiS277qB6m4bOuxuFqlDElzHactbvNkPLDByMbCrWAN7l7/sZ1g7SibkViw==",
                            UserRoleId = "Admin"
                        },
                        new
                        {
                            UserId = "2ad4d6ef-79ac-45e8-9cc9-8b54a6595663",
                            UserEmail = "Owner@gmail.com",
                            UserName = "Lapphan",
                            UserPassword = "AQAAAAIAAYagAAAAEGRCCPLVNku/t9tGutPukHEWTFaDJxSmMFprAlooESq+tnU+/SUsWgDIfM5ASI6hYg==",
                            UserRoleId = "Owner"
                        });
                });

            modelBuilder.Entity("JwtCookiesScheme.ResetToken", b =>
                {
                    b.HasOne("JwtCookiesScheme.User", "User")
                        .WithOne("UserToken")
                        .HasForeignKey("JwtCookiesScheme.ResetToken", "UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("JwtCookiesScheme.RolePermissions", b =>
                {
                    b.HasOne("JwtCookiesScheme.Permission", "Permission")
                        .WithMany("RolePermissions")
                        .HasForeignKey("PermissionId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("JwtCookiesScheme.Role", "Role")
                        .WithMany("RolePermissions")
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Permission");

                    b.Navigation("Role");
                });

            modelBuilder.Entity("JwtCookiesScheme.User", b =>
                {
                    b.HasOne("JwtCookiesScheme.Role", "UserRole")
                        .WithMany("Users")
                        .HasForeignKey("UserRoleId")
                        .OnDelete(DeleteBehavior.Restrict)
                        .IsRequired();

                    b.Navigation("UserRole");
                });

            modelBuilder.Entity("JwtCookiesScheme.Permission", b =>
                {
                    b.Navigation("RolePermissions");
                });

            modelBuilder.Entity("JwtCookiesScheme.Role", b =>
                {
                    b.Navigation("RolePermissions");

                    b.Navigation("Users");
                });

            modelBuilder.Entity("JwtCookiesScheme.User", b =>
                {
                    b.Navigation("UserToken");
                });
#pragma warning restore 612, 618
        }
    }
}
