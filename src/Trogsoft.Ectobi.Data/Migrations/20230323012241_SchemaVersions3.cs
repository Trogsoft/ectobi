using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemaVersions3 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_SchemaFields_SchemaFieldId",
                table: "SchemaFieldVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_SchemaVersions_SchemaVersionId",
                table: "SchemaFieldVersions");

            migrationBuilder.AlterColumn<long>(
                name: "SchemaVersionId",
                table: "SchemaFieldVersions",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_SchemaFields_SchemaFieldId",
                table: "SchemaFieldVersions",
                column: "SchemaFieldId",
                principalTable: "SchemaFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_SchemaVersions_SchemaVersionId",
                table: "SchemaFieldVersions",
                column: "SchemaVersionId",
                principalTable: "SchemaVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_SchemaFields_SchemaFieldId",
                table: "SchemaFieldVersions");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_SchemaVersions_SchemaVersionId",
                table: "SchemaFieldVersions");

            migrationBuilder.AlterColumn<long>(
                name: "SchemaVersionId",
                table: "SchemaFieldVersions",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_SchemaFields_SchemaFieldId",
                table: "SchemaFieldVersions",
                column: "SchemaFieldId",
                principalTable: "SchemaFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_SchemaVersions_SchemaVersionId",
                table: "SchemaFieldVersions",
                column: "SchemaVersionId",
                principalTable: "SchemaVersions",
                principalColumn: "Id");
        }
    }
}
