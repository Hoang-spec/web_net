using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buoi2.Migrations
{
    /// <inheritdoc />
    public partial class AddLogoColumnToCategory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Logo",
                table: "Categories",
                type: "TEXT",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Logo",
                table: "Categories");
        }
    }
}
