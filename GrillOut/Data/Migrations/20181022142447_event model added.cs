using Microsoft.EntityFrameworkCore.Migrations;

namespace GrillOut.Data.Migrations
{
    public partial class eventmodeladded : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsAssigned",
                table: "Employees");

            migrationBuilder.DropColumn(
                name: "IsDelivered",
                table: "Customers");

            migrationBuilder.DropColumn(
                name: "IsPickedUp",
                table: "Customers");

            migrationBuilder.RenameColumn(
                name: "CustomerID",
                table: "Customers",
                newName: "CustomerId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "CustomerId",
                table: "Customers",
                newName: "CustomerID");

            migrationBuilder.AddColumn<bool>(
                name: "IsAssigned",
                table: "Employees",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsDelivered",
                table: "Customers",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<bool>(
                name: "IsPickedUp",
                table: "Customers",
                nullable: false,
                defaultValue: false);
        }
    }
}
