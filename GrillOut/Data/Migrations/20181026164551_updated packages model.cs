using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace GrillOut.Data.Migrations
{
    public partial class updatedpackagesmodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "GrillOutPackages",
                columns: table => new
                {
                    PackageId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BasicGrillOut = table.Column<string>(nullable: true),
                    choseBasicGrillOut = table.Column<bool>(nullable: false),
                    AverageGrillOut = table.Column<string>(nullable: true),
                    choseAverageGrillOut = table.Column<bool>(nullable: false),
                    EliteGrillOut = table.Column<string>(nullable: true),
                    choseEliteGrillOut = table.Column<bool>(nullable: false),
                    CustomerId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_GrillOutPackages", x => x.PackageId);
                    table.ForeignKey(
                        name: "FK_GrillOutPackages_Customers_CustomerId",
                        column: x => x.CustomerId,
                        principalTable: "Customers",
                        principalColumn: "CustomerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_GrillOutPackages_CustomerId",
                table: "GrillOutPackages",
                column: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "GrillOutPackages");
        }
    }
}
