using Microsoft.EntityFrameworkCore.Migrations;

namespace CGM.Data.Migrations
{
    public partial class DeviceStatus : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DeviceStatus",
                columns: table => new
                {
                    DeviceStatusKey = table.Column<string>(nullable: false),
                    DeviceStatusBytes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DeviceStatus", x => x.DeviceStatusKey);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DeviceStatus");
        }
    }
}
