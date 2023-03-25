using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class RemoveRecordTextId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TextId",
                table: "Records");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "TextId",
                table: "Records",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }
    }
}
