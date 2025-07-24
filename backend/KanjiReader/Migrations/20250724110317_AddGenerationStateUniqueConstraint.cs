using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanjiReader.Migrations
{
    /// <inheritdoc />
    public partial class AddGenerationStateUniqueConstraint : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "UserGenerationStates",
                type: "text",
                nullable: false,
                defaultValue: "");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "Events",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "jsonb");

            migrationBuilder.CreateIndex(
                name: "IX_UserGenerationStates_UserId_SourceType",
                table: "UserGenerationStates",
                columns: new[] { "UserId", "SourceType" },
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_UserGenerationStates_AspNetUsers_UserId",
                table: "UserGenerationStates",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_UserGenerationStates_AspNetUsers_UserId",
                table: "UserGenerationStates");

            migrationBuilder.DropIndex(
                name: "IX_UserGenerationStates_UserId_SourceType",
                table: "UserGenerationStates");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "UserGenerationStates");

            migrationBuilder.AlterColumn<string>(
                name: "Data",
                table: "Events",
                type: "jsonb",
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
