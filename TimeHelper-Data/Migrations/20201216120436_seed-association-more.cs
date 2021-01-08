using Microsoft.EntityFrameworkCore.Migrations;

namespace GSK_TimeAssistant_Data.Migrations
{
    public partial class seedassociationmore : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Project",
                columns: new[] { "ProjectId", "Description", "ImageUrl", "Name" },
                values: new object[] { -2, "Project Armageddon", null, "Armageddon" });

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[] { -3, "armageddon", -2, 0, "keith2@nikkh.net" });

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[] { -4, "AGN404", -2, 1, "keith2@nikkh.net" });

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[] { -5, "wolverine", -2, 0, "nick@nikkh.net" });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -3);

            migrationBuilder.DeleteData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -2);
        }
    }
}
