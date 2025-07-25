﻿using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace KanjiReader.Migrations
{
    /// <inheritdoc />
    public partial class AddEventsUserId : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "Event",
                type: "text",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Event");
        }
    }
}
