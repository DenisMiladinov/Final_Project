using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Models.Migrations
{
    /// <inheritdoc />
    public partial class AddVacationSpotCategoryTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Categories_VacationSpots_VacationSpotSpotId",
                table: "Categories");

            migrationBuilder.DropForeignKey(
                name: "FK_VacationSpots_Categories_CategoryId",
                table: "VacationSpots");

            migrationBuilder.DropIndex(
                name: "IX_VacationSpots_CategoryId",
                table: "VacationSpots");

            migrationBuilder.DropIndex(
                name: "IX_Categories_VacationSpotSpotId",
                table: "Categories");

            migrationBuilder.DropColumn(
                name: "CategoryId",
                table: "VacationSpots");

            migrationBuilder.DropColumn(
                name: "VacationSpotSpotId",
                table: "Categories");

            migrationBuilder.CreateTable(
                name: "VacationSpotCategory",
                columns: table => new
                {
                    VacationSpotId = table.Column<int>(type: "int", nullable: false),
                    CategoryId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_VacationSpotCategory", x => new { x.VacationSpotId, x.CategoryId });
                    table.ForeignKey(
                        name: "FK_VacationSpotCategory_Categories_CategoryId",
                        column: x => x.CategoryId,
                        principalTable: "Categories",
                        principalColumn: "CategoryId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_VacationSpotCategory_VacationSpots_VacationSpotId",
                        column: x => x.VacationSpotId,
                        principalTable: "VacationSpots",
                        principalColumn: "SpotId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VacationSpotCategory_CategoryId",
                table: "VacationSpotCategory",
                column: "CategoryId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "VacationSpotCategory");

            migrationBuilder.AddColumn<int>(
                name: "CategoryId",
                table: "VacationSpots",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "VacationSpotSpotId",
                table: "Categories",
                type: "int",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VacationSpots_CategoryId",
                table: "VacationSpots",
                column: "CategoryId");

            migrationBuilder.CreateIndex(
                name: "IX_Categories_VacationSpotSpotId",
                table: "Categories",
                column: "VacationSpotSpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_Categories_VacationSpots_VacationSpotSpotId",
                table: "Categories",
                column: "VacationSpotSpotId",
                principalTable: "VacationSpots",
                principalColumn: "SpotId");

            migrationBuilder.AddForeignKey(
                name: "FK_VacationSpots_Categories_CategoryId",
                table: "VacationSpots",
                column: "CategoryId",
                principalTable: "Categories",
                principalColumn: "CategoryId",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
