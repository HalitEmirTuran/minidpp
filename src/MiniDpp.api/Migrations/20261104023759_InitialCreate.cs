using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MiniDpp.api.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "ProductModels",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Gtin = table.Column<string>(type: "TEXT", maxLength: 14, nullable: false),
                    Name = table.Column<string>(type: "TEXT", nullable: false),
                    CottonPercent = table.Column<decimal>(type: "TEXT", nullable: false),
                    ElastanePercent = table.Column<decimal>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ProductModels", x => x.ID);
                });

            migrationBuilder.CreateTable(
                name: "Lots",
                columns: table => new
                {
                    ID = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    LotNum = table.Column<string>(type: "TEXT", nullable: false),
                    ProductionPlace = table.Column<string>(type: "TEXT", nullable: false),
                    CarbonKG = table.Column<decimal>(type: "TEXT", nullable: false),
                    ProductModelId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Lots", x => x.ID);
                    table.ForeignKey(
                        name: "FK_Lots_ProductModels_ProductModelId",
                        column: x => x.ProductModelId,
                        principalTable: "ProductModels",
                        principalColumn: "ID",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Lots_ProductModelId_LotNum",
                table: "Lots",
                columns: new[] { "ProductModelId", "LotNum" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ProductModels_Gtin",
                table: "ProductModels",
                column: "Gtin",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Lots");

            migrationBuilder.DropTable(
                name: "ProductModels");
        }
    }
}
