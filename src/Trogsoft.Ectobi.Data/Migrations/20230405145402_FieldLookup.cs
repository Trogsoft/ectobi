using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class FieldLookup : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Schemas_ValuesFromSchemaId",
                table: "SchemaFields");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_Schemas_ValuesFromSchemaId",
                table: "SchemaFieldVersions");

            migrationBuilder.DropIndex(
                name: "IX_SchemaFields_ValuesFromSchemaId",
                table: "SchemaFields");

            migrationBuilder.DropColumn(
                name: "ValuesFromSchemaId",
                table: "SchemaFields");

            migrationBuilder.RenameColumn(
                name: "ValuesFromSchemaId",
                table: "SchemaFieldVersions",
                newName: "LookupSetId");

            migrationBuilder.RenameIndex(
                name: "IX_SchemaFieldVersions_ValuesFromSchemaId",
                table: "SchemaFieldVersions",
                newName: "IX_SchemaFieldVersions_LookupSetId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_LookupSets_LookupSetId",
                table: "SchemaFieldVersions",
                column: "LookupSetId",
                principalTable: "LookupSets",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_LookupSets_LookupSetId",
                table: "SchemaFieldVersions");

            migrationBuilder.RenameColumn(
                name: "LookupSetId",
                table: "SchemaFieldVersions",
                newName: "ValuesFromSchemaId");

            migrationBuilder.RenameIndex(
                name: "IX_SchemaFieldVersions_LookupSetId",
                table: "SchemaFieldVersions",
                newName: "IX_SchemaFieldVersions_ValuesFromSchemaId");

            migrationBuilder.AddColumn<long>(
                name: "ValuesFromSchemaId",
                table: "SchemaFields",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFields_ValuesFromSchemaId",
                table: "SchemaFields",
                column: "ValuesFromSchemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Schemas_ValuesFromSchemaId",
                table: "SchemaFields",
                column: "ValuesFromSchemaId",
                principalTable: "Schemas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_Schemas_ValuesFromSchemaId",
                table: "SchemaFieldVersions",
                column: "ValuesFromSchemaId",
                principalTable: "Schemas",
                principalColumn: "Id");
        }
    }
}
