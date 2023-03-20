using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class ProcessAndPopulator : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<long>(
                name: "IntegerValue",
                table: "Values",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AlterColumn<double>(
                name: "DecimalValue",
                table: "Values",
                type: "float",
                nullable: true,
                oldClrType: typeof(double),
                oldType: "float");

            migrationBuilder.AlterColumn<bool>(
                name: "BoolValue",
                table: "Values",
                type: "bit",
                nullable: true,
                oldClrType: typeof(bool),
                oldType: "bit");

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

            migrationBuilder.CreateTable(
                name: "Populator",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Populator", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Processes",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Processes", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "ProcessElements",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ProcessId = table.Column<long>(type: "bigint", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    TextId = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProcessElements", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ProcessElements_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFields_PopulatorId",
                table: "SchemaFields",
                column: "PopulatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFields_ProcessId",
                table: "SchemaFields",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_ProcessElements_ProcessId",
                table: "ProcessElements",
                column: "ProcessId");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Populator_PopulatorId",
                table: "SchemaFields",
                column: "PopulatorId",
                principalTable: "Populator",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_SchemaFields_Processes_ProcessId",
                table: "SchemaFields",
                column: "ProcessId",
                principalTable: "Processes",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Populator_PopulatorId",
                table: "SchemaFields");

            migrationBuilder.DropForeignKey(
                name: "FK_SchemaFields_Processes_ProcessId",
                table: "SchemaFields");

            migrationBuilder.DropTable(
                name: "Populator");

            migrationBuilder.DropTable(
                name: "ProcessElements");

            migrationBuilder.DropTable(
                name: "Processes");

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

            migrationBuilder.AlterColumn<long>(
                name: "IntegerValue",
                table: "Values",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AlterColumn<double>(
                name: "DecimalValue",
                table: "Values",
                type: "float",
                nullable: false,
                defaultValue: 0.0,
                oldClrType: typeof(double),
                oldType: "float",
                oldNullable: true);

            migrationBuilder.AlterColumn<bool>(
                name: "BoolValue",
                table: "Values",
                type: "bit",
                nullable: false,
                defaultValue: false,
                oldClrType: typeof(bool),
                oldType: "bit",
                oldNullable: true);
        }
    }
}
