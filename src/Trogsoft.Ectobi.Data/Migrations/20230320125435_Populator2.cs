using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Populator2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Populator_PopulatorId",
                table: "SchemaFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Populator",
                table: "Populator");

            migrationBuilder.RenameTable(
                name: "Populator",
                newName: "Populators");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Populators",
                table: "Populators",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Populators_PopulatorId",
                table: "SchemaFields",
                column: "PopulatorId",
                principalTable: "Populators",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Populators_PopulatorId",
                table: "SchemaFields");

            migrationBuilder.DropPrimaryKey(
                name: "PK_Populators",
                table: "Populators");

            migrationBuilder.RenameTable(
                name: "Populators",
                newName: "Populator");

            migrationBuilder.AddPrimaryKey(
                name: "PK_Populator",
                table: "Populator",
                column: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Populator_PopulatorId",
                table: "SchemaFields",
                column: "PopulatorId",
                principalTable: "Populator",
                principalColumn: "Id");
        }
    }
}
