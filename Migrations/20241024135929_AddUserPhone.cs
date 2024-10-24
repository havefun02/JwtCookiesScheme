using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace JwtCookiesScheme.Migrations
{
    /// <inheritdoc />
    public partial class AddUserPhone : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "2ad4d6ef-79ac-45e8-9cc9-8b54a6595663");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "c97688cf-8d9a-4c16-bdcb-cf0e933095bb");

            migrationBuilder.AddColumn<string>(
                name: "UserPhone",
                table: "Users",
                type: "longtext",
                nullable: false)
                .Annotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserPhone", "UserRoleId" },
                values: new object[,]
                {
                    { "b2369548-d2ed-4303-8b2a-f5bce219d506", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEGyYBFZuOWMMHz6NjoG78w+Yi/40Uw5mxbI/1/Zh0hIxKkEe3iBQ8jwU2XUQ1j74lg==", "", "Owner" },
                    { "f35540c9-3ce3-43f8-ab44-850f08599bf2", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEGx+v66nN+jrFUaFb/5hnbdx/avmlVumL7B453t3djwv9KIoh21SSl/xsicF6KYrnQ==", "", "Admin" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "b2369548-d2ed-4303-8b2a-f5bce219d506");

            migrationBuilder.DeleteData(
                table: "Users",
                keyColumn: "UserId",
                keyValue: "f35540c9-3ce3-43f8-ab44-850f08599bf2");

            migrationBuilder.DropColumn(
                name: "UserPhone",
                table: "Users");

            migrationBuilder.InsertData(
                table: "Users",
                columns: new[] { "UserId", "UserEmail", "UserName", "UserPassword", "UserRoleId" },
                values: new object[,]
                {
                    { "2ad4d6ef-79ac-45e8-9cc9-8b54a6595663", "Owner@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEGRCCPLVNku/t9tGutPukHEWTFaDJxSmMFprAlooESq+tnU+/SUsWgDIfM5ASI6hYg==", "Owner" },
                    { "c97688cf-8d9a-4c16-bdcb-cf0e933095bb", "Admin@gmail.com", "Lapphan", "AQAAAAIAAYagAAAAEOsdJgMiiS277qB6m4bOuxuFqlDElzHactbvNkPLDByMbCrWAN7l7/sZ1g7SibkViw==", "Admin" }
                });
        }
    }
}
