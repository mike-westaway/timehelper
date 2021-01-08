using Microsoft.EntityFrameworkCore.Migrations;

namespace GSK_TimeAssistant_Data.Migrations
{
    public partial class changetobeta : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -2,
                column: "Name",
                value: "Beta");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -2,
                column: "Name",
                value: "Armageddon");
        }
    }
}
