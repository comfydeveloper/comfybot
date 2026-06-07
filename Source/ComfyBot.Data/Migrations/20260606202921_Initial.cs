using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ComfyBot.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MessageResponse",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Users = table.Column<List<string>>(type: "text[]", nullable: true),
                    LooseKeywords = table.Column<List<string>>(type: "text[]", nullable: true),
                    AllKeywords = table.Column<List<string>>(type: "text[]", nullable: true),
                    ExactKeywords = table.Column<List<string>>(type: "text[]", nullable: true),
                    Replies = table.Column<List<string>>(type: "text[]", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    TimeoutInSeconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 30),
                    UseCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    Priority = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    AlwaysReply = table.Column<bool>(type: "boolean", nullable: false, defaultValue: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MessageResponse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "TextCommand",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Replies = table.Column<List<string>>(type: "text[]", nullable: true),
                    Commands = table.Column<List<string>>(type: "text[]", nullable: true),
                    LastUsedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: true),
                    UseCount = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    TimeoutInSeconds = table.Column<int>(type: "integer", nullable: false, defaultValue: 0),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_TextCommand", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Variable",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Name = table.Column<string>(type: "text", nullable: true),
                    Value = table.Column<string>(type: "text", nullable: true),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Variable", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MessageResponse");

            migrationBuilder.DropTable(
                name: "TextCommand");

            migrationBuilder.DropTable(
                name: "Variable");
        }
    }
}
