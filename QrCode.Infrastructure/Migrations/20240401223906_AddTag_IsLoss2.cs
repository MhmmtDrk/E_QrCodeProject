using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrCode.Infrastructure.Migrations
{
    public partial class AddTag_IsLoss2 : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "isLoss",
                table: "Tags",
                newName: "IsLoss");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "IsLoss",
                table: "Tags",
                newName: "isLoss");
        }
    }
}
