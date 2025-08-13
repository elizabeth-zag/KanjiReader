using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanjiReader.Migrations
{
    /// <inheritdoc />
    public partial class AddUnknownKanji : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<char[]>(
                name: "UnknownKanji",
                table: "ProcessingResults",
                type: "character(1)[]",
                nullable: false,
                defaultValue: new char[0]);

            migrationBuilder.AddColumn<double>(
                name: "UnknownKanjiRatio",
                table: "ProcessingResults",
                type: "double precision",
                nullable: false,
                defaultValue: 0.0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UnknownKanji",
                table: "ProcessingResults");

            migrationBuilder.DropColumn(
                name: "UnknownKanjiRatio",
                table: "ProcessingResults");
        }
    }
}
