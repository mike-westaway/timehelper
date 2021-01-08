using Microsoft.EntityFrameworkCore.Migrations;

namespace GSK_TimeAssistant_Data.Migrations
{
    public partial class seedassociation : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Association_Project_ProjectId",
                table: "Association");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Project",
                newName: "ProjectId");

            migrationBuilder.RenameColumn(
                name: "Id",
                table: "Association",
                newName: "AssociationId");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Association",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(int),
                oldType: "int",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Association",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[] { -1, "xtc", -1, 0, "nick@nikkh.net" });

            migrationBuilder.InsertData(
                table: "Association",
                columns: new[] { "AssociationId", "Criteria", "ProjectId", "Type", "UserId" },
                values: new object[] { -2, "AQ37", -1, 1, "nick@nikkh.net" });

            migrationBuilder.AddForeignKey(
                name: "FK_Association_Project_ProjectId",
                table: "Association",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "ProjectId",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Association_Project_ProjectId",
                table: "Association");

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -2);

            migrationBuilder.DeleteData(
                table: "Association",
                keyColumn: "AssociationId",
                keyValue: -1);

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Association");

            migrationBuilder.RenameColumn(
                name: "ProjectId",
                table: "Project",
                newName: "Id");

            migrationBuilder.RenameColumn(
                name: "AssociationId",
                table: "Association",
                newName: "Id");

            migrationBuilder.AlterColumn<int>(
                name: "ProjectId",
                table: "Association",
                type: "int",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.AddForeignKey(
                name: "FK_Association_Project_ProjectId",
                table: "Association",
                column: "ProjectId",
                principalTable: "Project",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
