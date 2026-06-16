using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ReportSystem.Migrations
{
    /// <inheritdoc />
    public partial class AddViolatorAndScores : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Address",
                table: "Users",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<int>(
                name: "Score",
                table: "Users",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ReliabilityScore",
                table: "Reports",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "ViolatorId",
                table: "Reports",
                type: "integer",
                nullable: true);

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "Ожидает приговора");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "В разработке");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Виновен (Принято)");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Оправдан (Отклонено)");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                columns: new[] { "Address", "PasswordHash", "Score" },
                values: new object[] { "", "$2a$11$j5g3HarcNfrPE9lb.WTjVeLnA3eCrihb.6WL9HDKB.uuvDR4yYvtS", 0 });

            migrationBuilder.CreateIndex(
                name: "IX_Reports_ViolatorId",
                table: "Reports",
                column: "ViolatorId");

            migrationBuilder.AddForeignKey(
                name: "FK_Reports_Users_ViolatorId",
                table: "Reports",
                column: "ViolatorId",
                principalTable: "Users",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Reports_Users_ViolatorId",
                table: "Reports");

            migrationBuilder.DropIndex(
                name: "IX_Reports_ViolatorId",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "Address",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "Score",
                table: "Users");

            migrationBuilder.DropColumn(
                name: "ReliabilityScore",
                table: "Reports");

            migrationBuilder.DropColumn(
                name: "ViolatorId",
                table: "Reports");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 1,
                column: "Name",
                value: "New");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 2,
                column: "Name",
                value: "InProgress");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 3,
                column: "Name",
                value: "Resolved");

            migrationBuilder.UpdateData(
                table: "ReportStatuses",
                keyColumn: "Id",
                keyValue: 4,
                column: "Name",
                value: "Rejected");

            migrationBuilder.UpdateData(
                table: "Users",
                keyColumn: "Id",
                keyValue: 1,
                column: "PasswordHash",
                value: "$2a$11$WQ83lKL4NLmBlEaQ1FF0ROcYauHTKvudcA2ROqZv5Q3zvlXQYeRyO");
        }
    }
}
