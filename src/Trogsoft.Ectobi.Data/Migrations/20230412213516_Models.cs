using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class Models : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "ModelField",
                table: "SchemaFieldVersions",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<long>(
                name: "ModelId",
                table: "SchemaFieldVersions",
                type: "bigint",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Models",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Handler = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    ModelType = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Models", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFieldVersions_ModelId",
                table: "SchemaFieldVersions",
                column: "ModelId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFieldVersions_Models_ModelId",
                table: "SchemaFieldVersions",
                column: "ModelId",
                principalTable: "Models",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFieldVersions_Models_ModelId",
                table: "SchemaFieldVersions");

            migrationBuilder.DropTable(
                name: "Models");

            migrationBuilder.DropIndex(
                name: "IX_SchemaFieldVersions_ModelId",
                table: "SchemaFieldVersions");

            migrationBuilder.DropColumn(
                name: "ModelField",
                table: "SchemaFieldVersions");

            migrationBuilder.DropColumn(
                name: "ModelId",
                table: "SchemaFieldVersions");
        }
    }
}
