using System;
using Microsoft.EntityFrameworkCore.Migrations;
using System.Diagnostics.CodeAnalysis;

#nullable disable

namespace ComfyBot.Data.Migrations
{
    /// <inheritdoc />
    [ExcludeFromCodeCoverage]
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageResponse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Users = table.Column<string>(type: "TEXT", nullable: true),
                    LooseKeywords = table.Column<string>(type: "TEXT", nullable: true),
                    AllKeywords = table.Column<string>(type: "TEXT", nullable: true),
                    ExactKeywords = table.Column<string>(type: "TEXT", nullable: true),
                    Replies = table.Column<string>(type: "TEXT", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    TimeoutInSeconds = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 30),
                    UseCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    Priority = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    AlwaysReply = table.Column<bool>(type: "INTEGER", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "TEXT", nullable: false),
                    Replies = table.Column<string>(type: "TEXT", nullable: true),
                    Commands = table.Column<string>(type: "TEXT", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "TEXT", nullable: true),
                    UseCount = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    TimeoutInSeconds = table.Column<int>(type: "INTEGER", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextCommand", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageResponse");

            migrationBuilder.DropTable(
                name: "TextCommand");
        }
    }
}
