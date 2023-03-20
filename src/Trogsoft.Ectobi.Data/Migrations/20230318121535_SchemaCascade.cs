using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemaCascade : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<long>(
                name: "SchemaId",
                table: "SchemaFields",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFields_SchemaId",
                table: "SchemaFields",
                column: "SchemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Schemas_SchemaId",
                table: "SchemaFields",
                column: "SchemaId",
                principalTable: "Schemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Schemas_SchemaId",
                table: "SchemaFields");

            migrationBuilder.DropIndex(
                name: "IX_SchemaFields_SchemaId",
                table: "SchemaFields");

            migrationBuilder.DropColumn(
                name: "SchemaId",
                table: "SchemaFields");
        }
    }
}
