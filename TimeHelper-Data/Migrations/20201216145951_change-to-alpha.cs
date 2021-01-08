using Microsoft.EntityFrameworkCore.Migrations;

namespace GSK_TimeAssistant_Data.Migrations
{
    public partial class changetoalpha : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -1,
                column: "Name",
                value: "Alpha");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -1,
                column: "Name",
                value: "Aquarius");
        }
    }
}
