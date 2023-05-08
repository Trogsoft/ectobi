using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class LookupSetSchemaLink : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SchemaId",
                table: "LookupSets",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_LookupSets_SchemaId",
                table: "LookupSets",
                column: "SchemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_LookupSets_Schemas_SchemaId",
                table: "LookupSets",
                column: "SchemaId",
                principalTable: "Schemas",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_LookupSets_Schemas_SchemaId",
                table: "LookupSets");

            migrationBuilder.DropIndex(
                name: "IX_LookupSets_SchemaId",
                table: "LookupSets");

            migrationBuilder.DropColumn(
                name: "SchemaId",
                table: "LookupSets");
        }
    }
}
