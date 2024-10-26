using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JwtCookiesScheme.Migrations
{
    /// <inheritdoc />
    public partial class AddTimeBase : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "b2369548-d2ed-4303-8b2a-f5bce219d506");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "f35540c9-3ce3-43f8-ab44-850f08599bf2");

            migrationBuilder.CreateTable(
                name: "Lockouts",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("MySql:ValueGenerationStrategy", MySqlValueGenerationStrategy.IdentityColumn),
                    SsId = table.Column<string>(type: "varchar(255)", nullable: true)
                        .Annotation("MySql:CharSet", "utf8mb4"),
                    FailedLoginAttempts = table.Column<int>(type: "int", nullable: false),
                    LockoutEndTime = table.Column<DateTime>(type: "datetime(6)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lockouts", x => x.Id);
                })
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "2ddbb5e0-3cfc-4a59-a8e4-53657668242e", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEHlPgvN5vulLJXBhFmlD3WOG+0uiMylIpd6gKYDNSxE4F0NRzKv3YEazElbg7lOa2w==", "", "Admin" },
                    { "377c21f5-3891-4c58-a03d-22cfe839bd45", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAELXmnhmZ4XBaXa9chKmvJEgW6V80ZvPy6ttVWTggW8DR+gDfidP8aUt6kKv5aNyGvQ==", "", "Owner" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lockouts_SsId",
                table: "Lockouts",
                column: "SsId",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lockouts");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "2ddbb5e0-3cfc-4a59-a8e4-53657668242e");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "377c21f5-3891-4c58-a03d-22cfe839bd45");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "b2369548-d2ed-4303-8b2a-f5bce219d506", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEGyYBFZuOWMMHz6NjoG78w+Yi/40Uw5mxbI/1/Zh0hIxKkEe3iBQ8jwU2XUQ1j74lg==", "", "Owner" },
                    { "f35540c9-3ce3-43f8-ab44-850f08599bf2", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEGx+v66nN+jrFUaFb/5hnbdx/avmlVumL7B453t3djwv9KIoh21SSl/xsicF6KYrnQ==", "", "Admin" }
                });
        }
    }
}
