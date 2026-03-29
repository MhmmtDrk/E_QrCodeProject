using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrCode.Infrastructure.Migrations
{
    public partial class Add_TagTypeColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "TagType",
                table: "Tags",
                type: "int",
                nullable: false,
                defaultValue: 0);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "TagType",
                table: "Tags");
        }
    }
}
