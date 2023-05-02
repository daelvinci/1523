using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    public partial class AppUserIDCONVERTEDTOSTRING : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_AspNetUsers_AppUserId1",
                table: "BasketItems");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_AppUserId1",
                table: "BasketItems");

            migrationBuilder.DropColumn(
                name: "AppUserId1",
                table: "BasketItems");

            migrationBuilder.AlterColumn<string>(
                name: "AppUserId",
                table: "BasketItems",
                type: "nvarchar(450)",
                nullable: true,
                oldClrType: typeof(int),
                oldType: "int");

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_AppUserId",
                table: "BasketItems",
                column: "AppUserId");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_AspNetUsers_AppUserId",
                table: "BasketItems",
                column: "AppUserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BasketItems_AspNetUsers_AppUserId",
                table: "BasketItems");

            migrationBuilder.DropIndex(
                name: "IX_BasketItems_AppUserId",
                table: "BasketItems");

            migrationBuilder.AlterColumn<int>(
                name: "AppUserId",
                table: "BasketItems",
                type: "int",
                nullable: false,
                defaultValue: 0,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)",
                oldNullable: true);

            migrationBuilder.AddColumn<string>(
                name: "AppUserId1",
                table: "BasketItems",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_BasketItems_AppUserId1",
                table: "BasketItems",
                column: "AppUserId1");

            migrationBuilder.AddForeignKey(
                name: "FK_BasketItems_AspNetUsers_AppUserId1",
                table: "BasketItems",
                column: "AppUserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");
        }
    }
}
