using Microsoft.EntityFrameworkCore.Migrations;

namespace GrillOut.Data.Migrations
{
    public partial class alteredpackagemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "AverageGrillOut",
                table: "GrillOutPackages");

            migrationBuilder.DropColumn(
                name: "EliteGrillOut",
                table: "GrillOutPackages");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "AverageGrillOut",
                table: "GrillOutPackages",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "EliteGrillOut",
                table: "GrillOutPackages",
                nullable: true);
        }
    }
}
