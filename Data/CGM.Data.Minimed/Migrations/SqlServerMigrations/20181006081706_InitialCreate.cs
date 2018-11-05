using System;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;

namespace CGM.Data.Minimed.Migrations.SqlServerMigrations
{
    public partial class InitialCreate : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "BayerStick",
                columns: table => new
                {
                    BayerStickId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                name: "DataLogger",
                columns: table => new
                {
                    DataLoggerId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataLoggerName = table.Column<string>(nullable: true),
                    DataLoggerKey = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLogger", x => x.DataLoggerId);
                });

            migrationBuilder.CreateTable(
                name: "EventType",
                columns: table => new
                {
                    EventTypeId = table.Column<int>(nullable: false),
                    EventTypeName = table.Column<string>(nullable: true),
                    EventTypeFullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EventType", x => x.EventTypeId);
                });

            migrationBuilder.CreateTable(
                name: "Pump",
                columns: table => new
                {
                    PumpId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                name: "PumpEventAlert",
                columns: table => new
                {
                    PumpEventAlertId = table.Column<int>(nullable: false),
                    PumpEventAlertName = table.Column<string>(nullable: true),
                    PumpEventAlertFullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpEventAlert", x => x.PumpEventAlertId);
                });

            migrationBuilder.CreateTable(
                name: "SensorReadingAlert",
                columns: table => new
                {
                    SensorReadingAlertId = table.Column<int>(nullable: false),
                    SensorReadingAlertName = table.Column<string>(nullable: true),
                    SensorReadingAlertFullName = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReadingAlert", x => x.SensorReadingAlertId);
                });

            migrationBuilder.CreateTable(
                name: "DataLoggerReading",
                columns: table => new
                {
                    DataLoggerReadingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    DataLoggerId = table.Column<int>(nullable: false),
                    BayerStickId = table.Column<int>(nullable: false),
                    ReadingDateTime = table.Column<DateTime>(nullable: false),
                    NextReadingDateTime = table.Column<DateTime>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DataLoggerReading", x => x.DataLoggerReadingId);
                    table.ForeignKey(
                        name: "FK_DataLoggerReading_BayerStick_BayerStickId",
                        column: x => x.BayerStickId,
                        principalTable: "BayerStick",
                        principalColumn: "BayerStickId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DataLoggerReading_DataLogger_DataLoggerId",
                        column: x => x.DataLoggerId,
                        principalTable: "DataLogger",
                        principalColumn: "DataLoggerId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "PumpStatus",
                columns: table => new
                {
                    PumpStatusId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
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
                    SgvDateTime_Rtc = table.Column<int>(nullable: false),
                    SgvDateTime_Offset = table.Column<int>(nullable: false),
                    SgvDateTime_Date = table.Column<DateTime>(nullable: false),
                    SgvDateTime_DateTimeEpoch = table.Column<double>(nullable: false),
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
                    AlertDateTime_Rtc = table.Column<int>(nullable: false),
                    AlertDateTime_Offset = table.Column<int>(nullable: false),
                    AlertDateTime_Date = table.Column<DateTime>(nullable: false),
                    AlertDateTime_DateTimeEpoch = table.Column<double>(nullable: false),
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
                name: "PumpEvent",
                columns: table => new
                {
                    PumpId = table.Column<int>(nullable: false),
                    DataLoggerReadingId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    EventTypeId = table.Column<int>(nullable: false),
                    Source = table.Column<byte>(nullable: false),
                    Length = table.Column<byte>(nullable: false),
                    EventDate_Rtc = table.Column<int>(nullable: false),
                    EventDate_Offset = table.Column<int>(nullable: false),
                    EventDate_Date = table.Column<DateTime>(nullable: false),
                    EventDate_DateTimeEpoch = table.Column<double>(nullable: false),
                    PumpEventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BytesAsString = table.Column<string>(nullable: true),
                    PumpEventAlertId = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_PumpEvent", x => x.PumpEventId);
                    table.ForeignKey(
                        name: "FK_PumpEvent_DataLoggerReading_DataLoggerReadingId",
                        column: x => x.DataLoggerReadingId,
                        principalTable: "DataLoggerReading",
                        principalColumn: "DataLoggerReadingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpEvent_EventType_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventType",
                        principalColumn: "EventTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpEvent_PumpEventAlert_PumpEventAlertId",
                        column: x => x.PumpEventAlertId,
                        principalTable: "PumpEventAlert",
                        principalColumn: "PumpEventAlertId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_PumpEvent_Pump_PumpId",
                        column: x => x.PumpId,
                        principalTable: "Pump",
                        principalColumn: "PumpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorEvent",
                columns: table => new
                {
                    PumpId = table.Column<int>(nullable: false),
                    DataLoggerReadingId = table.Column<int>(nullable: false),
                    Title = table.Column<string>(nullable: true),
                    EventTypeId = table.Column<int>(nullable: false),
                    Source = table.Column<byte>(nullable: false),
                    Length = table.Column<byte>(nullable: false),
                    EventDate_Rtc = table.Column<int>(nullable: false),
                    EventDate_Offset = table.Column<int>(nullable: false),
                    EventDate_Date = table.Column<DateTime>(nullable: false),
                    EventDate_DateTimeEpoch = table.Column<double>(nullable: false),
                    SensorEventId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    BytesAsString = table.Column<string>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorEvent", x => x.SensorEventId);
                    table.ForeignKey(
                        name: "FK_SensorEvent_DataLoggerReading_DataLoggerReadingId",
                        column: x => x.DataLoggerReadingId,
                        principalTable: "DataLoggerReading",
                        principalColumn: "DataLoggerReadingId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SensorEvent_EventType_EventTypeId",
                        column: x => x.EventTypeId,
                        principalTable: "EventType",
                        principalColumn: "EventTypeId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SensorEvent_Pump_PumpId",
                        column: x => x.PumpId,
                        principalTable: "Pump",
                        principalColumn: "PumpId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Calibration",
                columns: table => new
                {
                    CalibrationID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PumpEventId = table.Column<int>(nullable: false),
                    CalibrationFactor = table.Column<int>(nullable: false),
                    BG_BG_RAW = table.Column<short>(nullable: false),
                    BG_BG = table.Column<double>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Calibration", x => x.CalibrationID);
                    table.ForeignKey(
                        name: "FK_Calibration_PumpEvent_PumpEventId",
                        column: x => x.PumpEventId,
                        principalTable: "PumpEvent",
                        principalColumn: "PumpEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "DailyTotal",
                columns: table => new
                {
                    DailyTotalId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    PumpEventId = table.Column<int>(nullable: false),
                    Date_Rtc = table.Column<int>(nullable: false),
                    Date_Offset = table.Column<int>(nullable: false),
                    Date_Date = table.Column<DateTime>(nullable: false),
                    Date_DateTimeEpoch = table.Column<double>(nullable: false),
                    Duration = table.Column<short>(nullable: false),
                    METER_BG_COUNT = table.Column<byte>(nullable: false),
                    METER_BG_AVERAGE_BG_RAW = table.Column<short>(nullable: false),
                    METER_BG_AVERAGE_BG = table.Column<double>(nullable: false),
                    LOW_METER_BG_BG_RAW = table.Column<short>(nullable: false),
                    LOW_METER_BG_BG = table.Column<double>(nullable: false),
                    HIGH_METER_BG_BG_RAW = table.Column<short>(nullable: false),
                    HIGH_METER_BG_BG = table.Column<double>(nullable: false),
                    MANUALLY_ENTERED_BG_COUNT = table.Column<byte>(nullable: false),
                    MANUALLY_ENTERED_BG_AVERAGE_BG_RAW = table.Column<short>(nullable: false),
                    MANUALLY_ENTERED_BG_AVERAGE_BG = table.Column<double>(nullable: false),
                    LOW_MANUALLY_ENTERED_BG_BG_RAW = table.Column<short>(nullable: false),
                    LOW_MANUALLY_ENTERED_BG_BG = table.Column<double>(nullable: false),
                    HIGH_MANUALLY_ENTERED_BG_BG_RAW = table.Column<short>(nullable: false),
                    HIGH_MANUALLY_ENTERED_BG_BG = table.Column<double>(nullable: false),
                    BG_AVERAGE_BG_RAW = table.Column<short>(nullable: false),
                    BG_AVERAGE_BG = table.Column<double>(nullable: false),
                    TOTAL_INSULIN_InsulinRaw = table.Column<int>(nullable: false),
                    TOTAL_INSULIN_Insulin = table.Column<double>(nullable: false),
                    BASAL_INSULIN_InsulinRaw = table.Column<int>(nullable: false),
                    BASAL_INSULIN_Insulin = table.Column<double>(nullable: false),
                    BASAL_PERCENT = table.Column<byte>(nullable: false),
                    BOLUS_INSULIN_InsulinRaw = table.Column<int>(nullable: false),
                    BOLUS_INSULIN_Insulin = table.Column<double>(nullable: false),
                    BOLUS_PERCENT = table.Column<byte>(nullable: false),
                    CARB_UNITS = table.Column<byte>(nullable: false),
                    TOTAL_FOOD_INPUT = table.Column<short>(nullable: false),
                    BOLUS_WIZARD_USAGE_COUNT = table.Column<byte>(nullable: false),
                    TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_ONLY_BOLUS_InsulinRaw = table.Column<int>(nullable: false),
                    TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_ONLY_BOLUS_Insulin = table.Column<double>(nullable: false),
                    TOTAL_BOLUS_WIZARD_INSULIN_AS_CORRECTION_ONLY_BOLUS_InsulinRaw = table.Column<int>(nullable: false),
                    TOTAL_BOLUS_WIZARD_INSULIN_AS_CORRECTION_ONLY_BOLUS_Insulin = table.Column<double>(nullable: false),
                    TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_AND_CORRECTION_InsulinRaw = table.Column<int>(nullable: false),
                    TOTAL_BOLUS_WIZARD_INSULIN_AS_FOOD_AND_CORRECTION_Insulin = table.Column<double>(nullable: false),
                    TOTAL_MANUAL_BOLUS_INSULIN_InsulinRaw = table.Column<int>(nullable: false),
                    TOTAL_MANUAL_BOLUS_INSULIN_Insulin = table.Column<double>(nullable: false),
                    BOLUS_WIZARD_FOOD_ONLY_BOLUS_COUNT = table.Column<byte>(nullable: false),
                    BOLUS_WIZARD_CORRECTION_ONLY_BOLUS_COUNT = table.Column<byte>(nullable: false),
                    BOLUS_WIZARD_FOOD_AND_CORRECTION_BOLUS_COUNT = table.Column<byte>(nullable: false),
                    MANUAL_BOLUS_COUNT = table.Column<byte>(nullable: false),
                    SG_COUNT = table.Column<short>(nullable: false),
                    SG_AVERAGE_SG_RAW = table.Column<short>(nullable: false),
                    SG_AVERAGE_SG = table.Column<double>(nullable: false),
                    SG_STDDEV_SG_RAW = table.Column<short>(nullable: false),
                    SG_STDDEV_SG = table.Column<double>(nullable: false),
                    SG_DURATION_ABOVE_HIGH = table.Column<short>(nullable: false),
                    PERCENT_ABOVE_HIGH = table.Column<byte>(nullable: false),
                    SG_DURATION_WITHIN_LIMIT = table.Column<short>(nullable: false),
                    PERCENT_WITHIN_LIMIT = table.Column<byte>(nullable: false),
                    SG_DURATION_BELOW_LOW = table.Column<short>(nullable: false),
                    PERCENT_BELOW_LOW = table.Column<byte>(nullable: false),
                    LGS_SUSPENSION_DURATION = table.Column<short>(nullable: false),
                    HIGH_PREDICTIVE_ALERTS = table.Column<short>(nullable: false),
                    LOW_PREDICTIVE_ALERTS = table.Column<short>(nullable: false),
                    LOW_BG_ALERTS = table.Column<short>(nullable: false),
                    HIGH_BG_ALERTS = table.Column<short>(nullable: false),
                    RISING_RATE_ALERTS = table.Column<short>(nullable: false),
                    FALLING_RATE_ALERTS = table.Column<short>(nullable: false),
                    LOW_GLUCOSE_SUSPEND_ALERTS = table.Column<short>(nullable: false),
                    PREDICTIVE_LOW_GLUCOSE_SUSPEND_ALERTS = table.Column<short>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyTotal", x => x.DailyTotalId);
                    table.ForeignKey(
                        name: "FK_DailyTotal_PumpEvent_PumpEventId",
                        column: x => x.PumpEventId,
                        principalTable: "PumpEvent",
                        principalColumn: "PumpEventId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "SensorReading",
                columns: table => new
                {
                    SensorReadingId = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    SensorEventId = table.Column<int>(nullable: false),
                    ReadingDateTime = table.Column<DateTime>(nullable: false),
                    SgvRaw1 = table.Column<byte>(nullable: false),
                    SgvRaw2 = table.Column<byte>(nullable: false),
                    Amount = table.Column<int>(nullable: true),
                    IsigRaw = table.Column<short>(nullable: false),
                    RateOfChangeRaw = table.Column<short>(nullable: false),
                    SensorReadingAlertId = table.Column<int>(nullable: false),
                    SensorStatus = table.Column<byte>(nullable: false),
                    PredictedSg = table.Column<int>(nullable: true),
                    PredictedSg_Alert = table.Column<int>(nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SensorReading", x => x.SensorReadingId);
                    table.ForeignKey(
                        name: "FK_SensorReading_SensorEvent_SensorEventId",
                        column: x => x.SensorEventId,
                        principalTable: "SensorEvent",
                        principalColumn: "SensorEventId",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_SensorReading_SensorReadingAlert_SensorReadingAlertId",
                        column: x => x.SensorReadingAlertId,
                        principalTable: "SensorReadingAlert",
                        principalColumn: "SensorReadingAlertId",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Calibration_PumpEventId",
                table: "Calibration",
                column: "PumpEventId");

            migrationBuilder.CreateIndex(
                name: "IX_DailyTotal_PumpEventId",
                table: "DailyTotal",
                column: "PumpEventId");

            migrationBuilder.CreateIndex(
                name: "IX_DataLoggerReading_BayerStickId",
                table: "DataLoggerReading",
                column: "BayerStickId");

            migrationBuilder.CreateIndex(
                name: "IX_DataLoggerReading_DataLoggerId",
                table: "DataLoggerReading",
                column: "DataLoggerId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpEvent_DataLoggerReadingId",
                table: "PumpEvent",
                column: "DataLoggerReadingId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpEvent_EventTypeId",
                table: "PumpEvent",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpEvent_PumpEventAlertId",
                table: "PumpEvent",
                column: "PumpEventAlertId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpEvent_PumpId",
                table: "PumpEvent",
                column: "PumpId");

            migrationBuilder.CreateIndex(
                name: "IX_PumpStatus_PumpId",
                table: "PumpStatus",
                column: "PumpId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorEvent_DataLoggerReadingId",
                table: "SensorEvent",
                column: "DataLoggerReadingId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorEvent_EventTypeId",
                table: "SensorEvent",
                column: "EventTypeId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorEvent_PumpId",
                table: "SensorEvent",
                column: "PumpId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReading_SensorEventId",
                table: "SensorReading",
                column: "SensorEventId");

            migrationBuilder.CreateIndex(
                name: "IX_SensorReading_SensorReadingAlertId",
                table: "SensorReading",
                column: "SensorReadingAlertId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Calibration");

            migrationBuilder.DropTable(
                name: "DailyTotal");

            migrationBuilder.DropTable(
                name: "PumpStatus");

            migrationBuilder.DropTable(
                name: "SensorReading");

            migrationBuilder.DropTable(
                name: "PumpEvent");

            migrationBuilder.DropTable(
                name: "SensorEvent");

            migrationBuilder.DropTable(
                name: "SensorReadingAlert");

            migrationBuilder.DropTable(
                name: "PumpEventAlert");

            migrationBuilder.DropTable(
                name: "DataLoggerReading");

            migrationBuilder.DropTable(
                name: "EventType");

            migrationBuilder.DropTable(
                name: "Pump");

            migrationBuilder.DropTable(
                name: "BayerStick");

            migrationBuilder.DropTable(
                name: "DataLogger");
        }
    }
}
