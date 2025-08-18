using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanjiReader.Migrations
{
    /// <inheritdoc />
    public partial class AddWaniKaniStages : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int[]>(
                name: "WaniKaniStages",
                table: "AspNetUsers",
                type: "integer[]",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "WaniKaniStages",
                table: "AspNetUsers");
        }
    }
}
