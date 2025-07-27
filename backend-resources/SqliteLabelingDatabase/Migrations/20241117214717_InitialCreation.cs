using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SqliteLabelingDatabase.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreation : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AccessoriesTable",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Created = table.Column<DateTime>(type: "TEXT", nullable: false),
                    LastModified = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Code = table.Column<string>(type: "TEXT", nullable: false),
                    DescriptionGreek = table.Column<string>(type: "TEXT", nullable: false),
                    DescriptionEnglish = table.Column<string>(type: "TEXT", nullable: false),
                    Photo = table.Column<byte[]>(type: "BLOB", nullable: false),
                    FinishGreek = table.Column<string>(type: "TEXT", nullable: false),
                    FinishEnglish = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryTypeGreek = table.Column<string>(type: "TEXT", nullable: false),
                    PrimaryTypeEnglish = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryTypeGreek = table.Column<string>(type: "TEXT", nullable: false),
                    SecondaryTypeEnglish = table.Column<string>(type: "TEXT", nullable: false),
                    SeriesGreek = table.Column<string>(type: "TEXT", nullable: false),
                    SeriesEnglish = table.Column<string>(type: "TEXT", nullable: false),
                    MountingTypeGreek = table.Column<string>(type: "TEXT", nullable: false),
                    MountingTypeEnglish = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AccessoriesTable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AccessoriesTable");
        }
    }
}
