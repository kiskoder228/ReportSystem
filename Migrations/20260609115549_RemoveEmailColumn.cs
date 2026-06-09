using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class RemoveEmailColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Email",
                table: "Users");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$WQ83lKL4NLmBlEaQ1FF0ROcYauHTKvudcA2ROqZv5Q3zvlXQYeRyO");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Email",
                table: "Users",
                type: "text",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Email", "PasswordHash" },
                values: new object[] { null, "$2a$11$GKoS5eCC1t92HkFIJlOQaOgCxXWmCsl9Pl3ax8gg603ct37wjdgXy" });
        }
    }
}
