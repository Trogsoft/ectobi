using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Trogsoft.Ectobi.Data.Migrations
{
    /// <inheritdoc />
    public partial class SchemaVersions : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Schemas_SchemaId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_SchemaFields_SchemaFieldId",
                table: "Values");

            migrationBuilder.RenameColumn(
                name: "SchemaFieldId",
                table: "Values",
                newName: "SchemaFieldVersionId");

            migrationBuilder.RenameIndex(
                name: "IX_Values_SchemaFieldId",
                table: "Values",
                newName: "IX_Values_SchemaFieldVersionId");

            migrationBuilder.AddColumn<int>(
                name: "DisplayOrder",
                table: "SchemaFields",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<long>(
                name: "SchemaId",
                table: "Batches",
                type: "bigint",
                nullable: true,
                oldClrType: typeof(long),
                oldType: "bigint");

            migrationBuilder.AddColumn<long>(
                name: "SchemaVersionId",
                table: "Batches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L);

            migrationBuilder.CreateTable(
                name: "SchemaVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    Version = table.Column<int>(type: "int", nullable: false),
                    Created = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Description = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    SchemaId = table.Column<long>(type: "bigint", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaVersions_Schemas_SchemaId",
                        column: x => x.SchemaId,
                        principalTable: "Schemas",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SchemaFieldVersions",
                columns: table => new
                {
                    Id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    SchemaFieldId = table.Column<long>(type: "bigint", nullable: false),
                    ValuesFromSchemaId = table.Column<long>(type: "bigint", nullable: true),
                    Type = table.Column<int>(type: "int", nullable: false),
                    Flags = table.Column<int>(type: "int", nullable: false),
                    PopulatorId = table.Column<long>(type: "bigint", nullable: true),
                    ProcessId = table.Column<long>(type: "bigint", nullable: true),
                    DisplayOrder = table.Column<int>(type: "int", nullable: false),
                    SchemaVersionId = table.Column<long>(type: "bigint", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SchemaFieldVersions", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SchemaFieldVersions_Populators_PopulatorId",
                        column: x => x.PopulatorId,
                        principalTable: "Populators",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchemaFieldVersions_Processes_ProcessId",
                        column: x => x.ProcessId,
                        principalTable: "Processes",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchemaFieldVersions_SchemaFields_SchemaFieldId",
                        column: x => x.SchemaFieldId,
                        principalTable: "SchemaFields",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SchemaFieldVersions_SchemaVersions_SchemaVersionId",
                        column: x => x.SchemaVersionId,
                        principalTable: "SchemaVersions",
                        principalColumn: "Id");
                    table.ForeignKey(
                        name: "FK_SchemaFieldVersions_Schemas_ValuesFromSchemaId",
                        column: x => x.ValuesFromSchemaId,
                        principalTable: "Schemas",
                        principalColumn: "Id");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Batches_SchemaVersionId",
                table: "Batches",
                column: "SchemaVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFieldVersions_PopulatorId",
                table: "SchemaFieldVersions",
                column: "PopulatorId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFieldVersions_ProcessId",
                table: "SchemaFieldVersions",
                column: "ProcessId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFieldVersions_SchemaFieldId",
                table: "SchemaFieldVersions",
                column: "SchemaFieldId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFieldVersions_SchemaVersionId",
                table: "SchemaFieldVersions",
                column: "SchemaVersionId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaFieldVersions_ValuesFromSchemaId",
                table: "SchemaFieldVersions",
                column: "ValuesFromSchemaId");

            migrationBuilder.CreateIndex(
                name: "IX_SchemaVersions_SchemaId",
                table: "SchemaVersions",
                column: "SchemaId");

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_SchemaVersions_SchemaVersionId",
                table: "Batches",
                column: "SchemaVersionId",
                principalTable: "SchemaVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Schemas_SchemaId",
                table: "Batches",
                column: "SchemaId",
                principalTable: "Schemas",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Values_SchemaFieldVersions_SchemaFieldVersionId",
                table: "Values",
                column: "SchemaFieldVersionId",
                principalTable: "SchemaFieldVersions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Batches_SchemaVersions_SchemaVersionId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Batches_Schemas_SchemaId",
                table: "Batches");

            migrationBuilder.DropForeignKey(
                name: "FK_Values_SchemaFieldVersions_SchemaFieldVersionId",
                table: "Values");

            migrationBuilder.DropTable(
                name: "SchemaFieldVersions");

            migrationBuilder.DropTable(
                name: "SchemaVersions");

            migrationBuilder.DropIndex(
                name: "IX_Batches_SchemaVersionId",
                table: "Batches");

            migrationBuilder.DropColumn(
                name: "DisplayOrder",
                table: "SchemaFields");

            migrationBuilder.DropColumn(
                name: "SchemaVersionId",
                table: "Batches");

            migrationBuilder.RenameColumn(
                name: "SchemaFieldVersionId",
                table: "Values",
                newName: "SchemaFieldId");

            migrationBuilder.RenameIndex(
                name: "IX_Values_SchemaFieldVersionId",
                table: "Values",
                newName: "IX_Values_SchemaFieldId");

            migrationBuilder.AlterColumn<long>(
                name: "SchemaId",
                table: "Batches",
                type: "bigint",
                nullable: false,
                defaultValue: 0L,
                oldClrType: typeof(long),
                oldType: "bigint",
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_Batches_Schemas_SchemaId",
                table: "Batches",
                column: "SchemaId",
                principalTable: "Schemas",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Values_SchemaFields_SchemaFieldId",
                table: "Values",
                column: "SchemaFieldId",
                principalTable: "SchemaFields",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
