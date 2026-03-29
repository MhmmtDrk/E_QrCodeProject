using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrCode.Infrastructure.Migrations
{
    public partial class AddTag_IsLoss : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "isLoss",
                table: "Tags",
                type: "bit",
                nullable: false,
                defaultValue: false);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "isLoss",
                table: "Tags");
        }
    }
}
