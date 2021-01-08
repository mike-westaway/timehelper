using Microsoft.EntityFrameworkCore.Migrations;

namespace GSK_TimeAssistant_Data.Migrations
{
    public partial class changetogreeklettersforprojects : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -5,
                column: "Criteria",
                value: "beta");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -4,
                column: "Criteria",
                value: "beta");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -3,
                column: "Criteria",
                value: "beta");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -2,
                column: "Criteria",
                value: "alpha");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -1,
                column: "Criteria",
                value: "alpha");

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[] { -6, "beta", -2, 1, "nick@nikkh.net" });

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -2,
                column: "Description",
                value: "Project Beta");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -1,
                column: "Description",
                value: "Project Alpha");

            migrationBuilder.InsertData(
                table: "Project",
                columns: new[] { "ProjectId", "Description", "ImageUrl", "Name" },
                values: new object[,]
                {
                    { -3, "Project Gamma", null, "Gamma" },
                    { -4, "Project Delta", null, "Delta" },
                    { -5, "Project Epsilon", null, "Epsilon" }
                });

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[,]
                {
                    { -7, "gamma", -3, 0, "keith2@nikkh.net" },
                    { -8, "gamma", -3, 1, "keith2@nikkh.net" },
                    { -9, "gamma", -3, 0, "nick@nikkh.net" },
                    { -10, "gamma", -3, 1, "nick@nikkh.net" },
                    { -11, "delta", -4, 0, "keith2@nikkh.net" },
                    { -12, "delta", -4, 1, "keith2@nikkh.net" },
                    { -13, "delta", -4, 0, "nick@nikkh.net" },
                    { -14, "delta", -4, 1, "nick@nikkh.net" },
                    { -15, "epsilon", -5, 0, "keith2@nikkh.net" },
                    { -16, "epsilon", -5, 1, "keith2@nikkh.net" },
                    { -17, "epsilon", -5, 0, "nick@nikkh.net" },
                    { -18, "epsilon", -5, 1, "nick@nikkh.net" }
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -18);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -17);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -16);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -15);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -14);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -13);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -12);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -11);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -10);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -9);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -8);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -7);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -6);

            migrationBuilder.DeleteData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -5);

            migrationBuilder.DeleteData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -4);

            migrationBuilder.DeleteData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -3);

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -5,
                column: "Criteria",
                value: "wolverine");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -4,
                column: "Criteria",
                value: "AGN404");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -3,
                column: "Criteria",
                value: "armageddon");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -2,
                column: "Criteria",
                value: "AQ37");

            migrationBuilder.UpdateData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -1,
                column: "Criteria",
                value: "xtc");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -2,
                column: "Description",
                value: "Project Armageddon");

            migrationBuilder.UpdateData(
                table: "Project",
                keyColumn: "ProjectId",
                keyValue: -1,
                column: "Description",
                value: "Project Aquarius");
        }
    }
}
