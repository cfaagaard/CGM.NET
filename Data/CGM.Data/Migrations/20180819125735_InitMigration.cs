using Microsoft.EntityFrameworkCore.Migrations;

namespace CGM.Data.Migrations
{
    public partial class InitMigration : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Configuration",
                columns: table => new
                {
                    ConfigurationName = table.Column<string>(nullable: false),
                    ConfigurationValue = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Configuration", x => x.ConfigurationName);
                });

            migrationBuilder.CreateTable(
                name: "Device",
                columns: table => new
                {
                    SerialNumber = table.Column<string>(nullable: false),
                    SerialNumberFull = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    LinkMac = table.Column<string>(nullable: true),
                    PumpMac = table.Column<string>(nullable: true),
                    RadioChannel = table.Column<string>(nullable: true),
                    LinkKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Device", x => x.SerialNumber);
                });

            migrationBuilder.CreateTable(
                name: "History",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    EventType = table.Column<int>(nullable: false),
                    Rtc = table.Column<int>(nullable: false),
                    HistoryDataType = table.Column<int>(nullable: false),
                    HistoryBytes = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_History", x => x.Key);
                });

            migrationBuilder.CreateTable(
                name: "HistoryStatus",
                columns: table => new
                {
                    Key = table.Column<string>(nullable: false),
                    Status = table.Column<int>(nullable: false),
                    Comment = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_HistoryStatus", x => x.Key);
                });

            migrationBuilder.Sql("INSERT INTO [Configuration] ([ConfigurationName],[ConfigurationValue]) VALUES ('MinimedConfiguration', '{\"IntervalSeconds\":300,\"TimeoutSeconds\":10,\"IncludePumpSettings\":true,\"HistoryDaysBack\":2,\"LastRead\":[],\"IncludeHistory\":true}');");


        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Configuration");

            migrationBuilder.DropTable(
                name: "Device");

            migrationBuilder.DropTable(
                name: "History");

            migrationBuilder.DropTable(
                name: "HistoryStatus");
        }
    }
}
