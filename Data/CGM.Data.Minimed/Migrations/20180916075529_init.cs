using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CGM.Data.Minimed.Migrations
{
    public partial class init : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BayerStick",
                columns: table => new
                {
                    BayerStickId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Value = table.Column<string>(nullable: true),
                    SerialNumberFull = table.Column<string>(nullable: true),
                    RFID = table.Column<string>(nullable: true),
                    ModelNumber = table.Column<string>(nullable: true),
                    SerialNumber = table.Column<string>(nullable: true),
                    HMACbyte = table.Column<string>(nullable: true),
                    DigitalEngineVersion = table.Column<string>(nullable: true),
                    AnalogEngineVersion = table.Column<string>(nullable: true),
                    GameBoardVersion = table.Column<string>(nullable: true),
                    Name = table.Column<string>(nullable: true),
                    Manufacturer = table.Column<string>(nullable: true),
                    SerialNum = table.Column<string>(nullable: true),
                    SerialNumSmall = table.Column<string>(nullable: true),
                    SkuIdentifier = table.Column<string>(nullable: true),
                    AccessPassword = table.Column<string>(nullable: true),
                    MeterLanguage = table.Column<string>(nullable: true),
                    TestReminderInterval = table.Column<string>(nullable: true),
                    AllBytesAsString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_BayerStick", x => x.BayerStickId);
                });

            migrationBuilder.CreateTable(
                name: "Pump",
                columns: table => new
                {
                    PumpId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SerialNumber = table.Column<string>(nullable: true),
                    Mac = table.Column<string>(nullable: true),
                    FirmwareMajorNumber = table.Column<byte>(nullable: false),
                    FirmwareMinorNumber = table.Column<byte>(nullable: false),
                    FirmwareAlphaNumber = table.Column<string>(nullable: true),
                    Firmware = table.Column<string>(nullable: true),
                    MotorMajorNumber = table.Column<byte>(nullable: false),
                    MotorMinorNumber = table.Column<byte>(nullable: false),
                    MotorAlphaNumber = table.Column<string>(nullable: true),
                    Motor = table.Column<string>(nullable: true),
                    BgUnitRaw = table.Column<byte>(nullable: false),
                    BytesAsString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Pump", x => x.PumpId);
                });

            migrationBuilder.CreateTable(
                name: "PumpEvent",
                columns: table => new
                {
                    BayerStickId = table.Column<int>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false),
                    EventTypeRaw = table.Column<byte>(nullable: false),
                    Length = table.Column<byte>(nullable: false),
                    Offset = table.Column<int>(nullable: false),
                    PumpId = table.Column<int>(nullable: false),
                    Rtc = table.Column<int>(nullable: false),
                    Source = table.Column<byte>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    BytesAsString = table.Column<string>(nullable: true),
                    PumpEventId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpEvent", x => x.PumpEventId);
                    table.ForeignKey(
                        name: "FK_PumpEvent_BayerStick_BayerStickId",
                        column: x => x.BayerStickId,
                        principalTable: "BayerStick",
                        principalColumn: "BayerStickId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpEvent_Pump_PumpId",
                        column: x => x.PumpId,
                        principalTable: "Pump",
                        principalColumn: "PumpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PumpStatus",
                columns: table => new
                {
                    PumpStatusId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    PumpId = table.Column<int>(nullable: false),
                    PumpStatusDateTime = table.Column<DateTime>(nullable: false),
                    StatusFlag = table.Column<byte>(nullable: false),
                    NowBolusingAmountDelivered = table.Column<int>(nullable: false),
                    Unknown1 = table.Column<int>(nullable: false),
                    NowBolusingMinutesRemaining = table.Column<short>(nullable: false),
                    NowBolusingReference = table.Column<short>(nullable: false),
                    LastBolusAmount = table.Column<int>(nullable: false),
                    LastBolusTime = table.Column<int>(nullable: false),
                    LastBolusDateTime = table.Column<DateTime>(nullable: true),
                    LastBolusReference = table.Column<short>(nullable: false),
                    ActiveBasalPattern = table.Column<byte>(nullable: false),
                    NormalBasalRaw = table.Column<int>(nullable: false),
                    TempBasal = table.Column<int>(nullable: false),
                    TempBasalPercentage = table.Column<byte>(nullable: false),
                    TempBasalMinutesRemaining = table.Column<short>(nullable: false),
                    BasalUnitsDeliveredTodayRaw = table.Column<int>(nullable: false),
                    BatteryPercentage = table.Column<byte>(nullable: false),
                    ReservoirAmountRaw = table.Column<int>(nullable: false),
                    InsulinHours = table.Column<byte>(nullable: false),
                    InsulinMinutes = table.Column<byte>(nullable: false),
                    ActiveInsulinRaw = table.Column<int>(nullable: false),
                    ActiveInsulin = table.Column<double>(nullable: false),
                    SgvRaw = table.Column<short>(nullable: false),
                    Sgv = table.Column<int>(nullable: false),
                    SgvDateTime = table.Column<DateTime>(nullable: false),
                    LowSuspendActive = table.Column<byte>(nullable: false),
                    CgmTrend = table.Column<byte>(nullable: false),
                    SensorStatusFlag = table.Column<byte>(nullable: false),
                    Unknown3 = table.Column<byte>(nullable: false),
                    SensorCalibrationMinutesRemaining = table.Column<short>(nullable: false),
                    SensorCalibrationDateTime = table.Column<DateTime>(nullable: true),
                    SensorBatteryRaw = table.Column<byte>(nullable: false),
                    SensorBattery = table.Column<int>(nullable: false),
                    SensorRateOfChangeRaw = table.Column<short>(nullable: false),
                    BolusWizardRecent = table.Column<byte>(nullable: false),
                    BolusWizardBGLRaw = table.Column<short>(nullable: false),
                    BolusWizardBGL = table.Column<int>(nullable: false),
                    Alert = table.Column<short>(nullable: false),
                    AlertDateTime = table.Column<DateTime>(nullable: true),
                    Unknown6 = table.Column<byte[]>(nullable: true),
                    SgvMmol = table.Column<double>(nullable: false),
                    NormalBasal = table.Column<double>(nullable: false),
                    BolusEstimate = table.Column<double>(nullable: false),
                    BasalUnitsDeliveredToday = table.Column<double>(nullable: false),
                    ReservoirAmount = table.Column<double>(nullable: false),
                    AlertName = table.Column<string>(nullable: true),
                    PS_Suspended = table.Column<bool>(nullable: false),
                    PS_BolusingNormal = table.Column<bool>(nullable: false),
                    PS_BolusingSquare = table.Column<bool>(nullable: false),
                    PS_BolusingDual = table.Column<bool>(nullable: false),
                    PS_DeliveringInsulin = table.Column<bool>(nullable: false),
                    PS_CgmActive = table.Column<bool>(nullable: false),
                    PS_TempBasalActive = table.Column<bool>(nullable: false),
                    SS_Calibrating = table.Column<bool>(nullable: false),
                    SS_CalibrationComplete = table.Column<bool>(nullable: false),
                    SS_Exception = table.Column<bool>(nullable: false),
                    BytesAsString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpStatus", x => x.PumpStatusId);
                    table.ForeignKey(
                        name: "FK_PumpStatus_Pump_PumpId",
                        column: x => x.PumpId,
                        principalTable: "Pump",
                        principalColumn: "PumpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorEvent",
                columns: table => new
                {
                    BayerStickId = table.Column<int>(nullable: false),
                    EventDate = table.Column<DateTime>(nullable: false),
                    EventTypeRaw = table.Column<byte>(nullable: false),
                    Length = table.Column<byte>(nullable: false),
                    Offset = table.Column<int>(nullable: false),
                    PumpId = table.Column<int>(nullable: false),
                    Rtc = table.Column<int>(nullable: false),
                    Source = table.Column<byte>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    BytesAsString = table.Column<string>(nullable: true),
                    SensorEventId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorEvent", x => x.SensorEventId);
                    table.ForeignKey(
                        name: "FK_SensorEvent_BayerStick_BayerStickId",
                        column: x => x.BayerStickId,
                        principalTable: "BayerStick",
                        principalColumn: "BayerStickId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SensorEvent_Pump_PumpId",
                        column: x => x.PumpId,
                        principalTable: "Pump",
                        principalColumn: "PumpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReading",
                columns: table => new
                {
                    SgReadingId = table.Column<int>(nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    SensorEventId = table.Column<int>(nullable: false),
                    ReadingDateTime = table.Column<DateTime>(nullable: false),
                    SgvRaw1 = table.Column<byte>(nullable: false),
                    SgvRaw2 = table.Column<byte>(nullable: false),
                    Amount = table.Column<int>(nullable: false),
                    IsigRaw = table.Column<short>(nullable: false),
                    RateOfChangeRaw = table.Column<short>(nullable: false),
                    SensorStatus = table.Column<byte>(nullable: false),
                    PredictedSg = table.Column<ushort>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReading", x => x.SgReadingId);
                    table.ForeignKey(
                        name: "FK_SensorReading_SensorEvent_SensorEventId",
                        column: x => x.SensorEventId,
                        principalTable: "SensorEvent",
                        principalColumn: "SensorEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_PumpEvent_BayerStickId",
                table: "PumpEvent",
                column: "BayerStickId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpEvent_PumpId",
                table: "PumpEvent",
                column: "PumpId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpStatus_PumpId",
                table: "PumpStatus",
                column: "PumpId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorEvent_BayerStickId",
                table: "SensorEvent",
                column: "BayerStickId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorEvent_PumpId",
                table: "SensorEvent",
                column: "PumpId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReading_SensorEventId",
                table: "SensorReading",
                column: "SensorEventId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "PumpEvent");

            migrationBuilder.DropTable(
                name: "PumpStatus");

            migrationBuilder.DropTable(
                name: "SensorReading");

            migrationBuilder.DropTable(
                name: "SensorEvent");

            migrationBuilder.DropTable(
                name: "BayerStick");

            migrationBuilder.DropTable(
                name: "Pump");
        }
    }
}
