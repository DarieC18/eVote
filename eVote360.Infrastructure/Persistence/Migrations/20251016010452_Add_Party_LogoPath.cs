using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Add_Party_LogoPath : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ScheduledAt",
                table: "Elections");

            migrationBuilder.RenameColumn(
                name: "State",
                table: "Elections",
                newName: "Year");

            migrationBuilder.AlterColumn<string>(
                name: "Siglas",
                table: "Parties",
                type: "nvarchar(12)",
                maxLength: 12,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(20)",
                oldMaxLength: 20);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Parties",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Parties",
                type: "nvarchar(400)",
                maxLength: 400,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(500)",
                oldMaxLength: 500,
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "LogoPath",
                table: "Parties",
                type: "nvarchar(260)",
                maxLength: 260,
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Elections",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(150)",
                oldMaxLength: 150);

            migrationBuilder.AddColumn<DateTime>(
                name: "FinishedAt",
                table: "Elections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "StartedAt",
                table: "Elections",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Elections",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Elections_Year_Name",
                table: "Elections",
                columns: new[] { "Year", "Name" },
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropIndex(
                name: "IX_Elections_Year_Name",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "LogoPath",
                table: "Parties");

            migrationBuilder.DropColumn(
                name: "FinishedAt",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "StartedAt",
                table: "Elections");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Elections");

            migrationBuilder.RenameColumn(
                name: "Year",
                table: "Elections",
                newName: "State");

            migrationBuilder.AlterColumn<string>(
                name: "Siglas",
                table: "Parties",
                type: "nvarchar(20)",
                maxLength: 20,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(12)",
                oldMaxLength: 12);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Parties",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AlterColumn<string>(
                name: "Description",
                table: "Parties",
                type: "nvarchar(500)",
                maxLength: 500,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(400)",
                oldMaxLength: 400,
                oldNullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "Elections",
                type: "nvarchar(150)",
                maxLength: 150,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<DateTime>(
                name: "ScheduledAt",
                table: "Elections",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }
    }
}
