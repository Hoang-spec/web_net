using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Buoi2.Migrations
{
    /// <summary>
    /// Initial migration for creating the Products table
    /// </summary>
    public partial class InitialCreate : Migration
    {
        /// <summary>
        /// Creates the initial database schema
        /// </summary>
        /// <param name="migrationBuilder">The migration builder instance</param>
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Products",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    Price = table.Column<int>(type: "INTEGER", nullable: false),
                    Image = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Products", x => x.Id);
                });
        }

        /// <summary>
        /// Reverts the database schema changes
        /// </summary>
        /// <param name="migrationBuilder">The migration builder instance</param>
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Products");
        }
    }
}
