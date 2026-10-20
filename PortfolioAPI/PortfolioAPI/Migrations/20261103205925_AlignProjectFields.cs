using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace PortfolioAPI.Migrations
{
    /// <inheritdoc />
    public partial class AlignProjectFields : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Datum",
                table: "Projects");

            migrationBuilder.DropColumn(
                name: "GitHubUrl",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Titel",
                table: "Projects",
                newName: "Title");

            migrationBuilder.RenameColumn(
                name: "Categorie",
                table: "Projects",
                newName: "Category");

            migrationBuilder.RenameColumn(
                name: "Beschrijving",
                table: "Projects",
                newName: "Description");

            migrationBuilder.AddColumn<string>(
                name: "Technologies",
                table: "Projects",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "[]");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Technologies",
                table: "Projects");

            migrationBuilder.RenameColumn(
                name: "Title",
                table: "Projects",
                newName: "Titel");

            migrationBuilder.RenameColumn(
                name: "Description",
                table: "Projects",
                newName: "Beschrijving");

            migrationBuilder.RenameColumn(
                name: "Category",
                table: "Projects",
                newName: "Categorie");

            migrationBuilder.AddColumn<DateTime>(
                name: "Datum",
                table: "Projects",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<string>(
                name: "GitHubUrl",
                table: "Projects",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: false,
                defaultValue: "");
        }
    }
}
