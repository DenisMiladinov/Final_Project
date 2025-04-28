using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AddCategoryToVacationSpot : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "VacationSpots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_VacationSpots_CategoryId",
                table: "VacationSpots",
                column: "CategoryId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationSpots_Categories_CategoryId",
                table: "VacationSpots",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_VacationSpots_Categories_CategoryId",
                table: "VacationSpots");

            migrationBuilder.DropIndex(
                name: "IX_VacationSpots_CategoryId",
                table: "VacationSpots");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "VacationSpots");
        }
    }
}
