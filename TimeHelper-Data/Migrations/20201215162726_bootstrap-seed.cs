using Microsoft.EntityFrameworkCore.Migrations;

namespace GSK_TimeAssistant_Data.Migrations
{
    public partial class bootstrapseed : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Project",
                columns: new[] { "Id", "Description", "ImageUrl", "Name" },
                values: new object[] { -1, "Project Aquarius", null, "Aquarius" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Project",
                keyColumn: "Id",
                keyValue: -1);
        }
    }
}
