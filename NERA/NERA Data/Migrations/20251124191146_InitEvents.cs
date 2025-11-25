using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Data.Migrations
{
    /// <inheritdoc />
    public partial class InitEvents : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "RegisteredAtUtc",
                table: "EventRegistration");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "EventRegistration");

            migrationBuilder.AddColumn<bool>(
                name: "Attandance",
                table: "EventRegistration",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<byte[]>(
                name: "Qr",
                table: "EventRegistration",
                type: "varbinary(max)",
                nullable: false,
                defaultValue: new byte[0]);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Attandance",
                table: "EventRegistration");

            migrationBuilder.DropColumn(
                name: "Qr",
                table: "EventRegistration");

            migrationBuilder.AddColumn<DateTime>(
                name: "RegisteredAtUtc",
                table: "EventRegistration",
                type: "datetime2(0)",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "EventRegistration",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }
    }
}
