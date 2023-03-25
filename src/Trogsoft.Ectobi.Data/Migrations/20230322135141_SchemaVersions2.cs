using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemaVersions2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Populators_PopulatorId",
                table: "SchemaFields");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Processes_ProcessId",
                table: "SchemaFields");

            migrationBuilder.DropIndex(
                name: "IX_SchemaFields_PopulatorId",
                table: "SchemaFields");

            migrationBuilder.DropIndex(
                name: "IX_SchemaFields_ProcessId",
                table: "SchemaFields");

            migrationBuilder.DropColumn(
                name: "PopulatorId",
                table: "SchemaFields");

            migrationBuilder.DropColumn(
                name: "ProcessId",
                table: "SchemaFields");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "SchemaFieldVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "Name",
                table: "SchemaFieldVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TextId",
                table: "SchemaFieldVersions",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Description",
                table: "SchemaFieldVersions");

            migrationBuilder.DropColumn(
                name: "Name",
                table: "SchemaFieldVersions");

            migrationBuilder.DropColumn(
                name: "TextId",
                table: "SchemaFieldVersions");

            migrationBuilder.AddColumn<long>(
                name: "PopulatorId",
                table: "SchemaFields",
                type: "bigint",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ProcessId",
                table: "SchemaFields",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFields_PopulatorId",
                table: "SchemaFields",
                column: "PopulatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFields_ProcessId",
                table: "SchemaFields",
                column: "ProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Populators_PopulatorId",
                table: "SchemaFields",
                column: "PopulatorId",
                principalTable: "Populators",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Processes_ProcessId",
                table: "SchemaFields",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id");
        }
    }
}
