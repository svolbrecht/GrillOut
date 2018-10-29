using Microsoft.EntityFrameworkCore.Migrations;

namespace GrillOut.Data.Migrations
{
    public partial class changedpackagemodel : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "BasicGrillOut",
                table: "GrillOutPackages",
                newName: "GrillOutPackage");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "GrillOutPackage",
                table: "GrillOutPackages",
                newName: "BasicGrillOut");
        }
    }
}
