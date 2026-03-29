using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace QrCode.Infrastructure.Migrations
{
    public partial class Add_DogLeashColumn : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "DogLeash",
                table: "Tags",
                type: "nvarchar(max)",
                nullable: true);

         
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "View_TagUsers");

            migrationBuilder.DropColumn(
                name: "DogLeash",
                table: "Tags");
        }
    }
}
