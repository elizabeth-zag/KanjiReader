using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanjiReader.Migrations
{
    /// <inheritdoc />
    public partial class AddKanjiInfo : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "KunReading",
                table: "Kanji",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "Meaning",
                table: "Kanji",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AddColumn<string>(
                name: "OnReading",
                table: "Kanji",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "KunReading",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "Meaning",
                table: "Kanji");

            migrationBuilder.DropColumn(
                name: "OnReading",
                table: "Kanji");
        }
    }
}
