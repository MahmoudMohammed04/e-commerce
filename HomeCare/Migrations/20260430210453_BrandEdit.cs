using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace HomeCare.Migrations
{
    /// <inheritdoc />
    public partial class BrandEdit : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryID",
                table: "Brands",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Brands_CategoryID",
                table: "Brands",
                column: "CategoryID");

            migrationBuilder.AddForeignKey(
                name: "FK_Brands_Categories_CategoryID",
                table: "Brands",
                column: "CategoryID",
                principalTable: "Categories",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Brands_Categories_CategoryID",
                table: "Brands");

            migrationBuilder.DropIndex(
                name: "IX_Brands_CategoryID",
                table: "Brands");

            migrationBuilder.DropColumn(
                name: "CategoryID",
                table: "Brands");
        }
    }
}
