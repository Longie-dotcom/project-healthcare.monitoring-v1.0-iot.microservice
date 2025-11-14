using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "AuditLogs",
                columns: table => new
                {
                    AuditLogID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EntityName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Action = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    PerformedBy = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Timestamp = table.Column<DateTime>(type: "datetime2", nullable: false),
                    OldValue = table.Column<string>(type: "nvarchar(max)", nullable: true),
                    NewValue = table.Column<string>(type: "nvarchar(max)", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_AuditLogs", x => x.AuditLogID);
                });

            migrationBuilder.CreateTable(
                name: "EdgeDevices",
                columns: table => new
                {
                    EdgeDeviceID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    EdgeKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    RoomName = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_EdgeDevices", x => x.EdgeDeviceID);
                    table.UniqueConstraint("AK_EdgeDevices_EdgeKey", x => x.EdgeKey);
                });

            migrationBuilder.CreateTable(
                name: "Controllers",
                columns: table => new
                {
                    ControllerID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    ControllerKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    EdgeKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    BedNumber = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    IpAddress = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    FirmwareVersion = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Status = table.Column<string>(type: "nvarchar(30)", maxLength: 30, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Controllers", x => x.ControllerID);
                    table.UniqueConstraint("AK_Controllers_ControllerKey", x => x.ControllerKey);
                    table.ForeignKey(
                        name: "FK_Controllers_EdgeDevices_EdgeKey",
                        column: x => x.EdgeKey,
                        principalTable: "EdgeDevices",
                        principalColumn: "EdgeKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateTable(
                name: "Sensors",
                columns: table => new
                {
                    SensorID = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SensorKey = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    ControllerKey = table.Column<string>(type: "nvarchar(450)", nullable: false),
                    Type = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Unit = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Description = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false, defaultValue: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Sensors", x => x.SensorID);
                    table.ForeignKey(
                        name: "FK_Sensors_Controllers_ControllerKey",
                        column: x => x.ControllerKey,
                        principalTable: "Controllers",
                        principalColumn: "ControllerKey",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_Controllers_EdgeKey",
                table: "Controllers",
                column: "EdgeKey");

            migrationBuilder.CreateIndex(
                name: "IX_Sensors_ControllerKey",
                table: "Sensors",
                column: "ControllerKey");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "AuditLogs");

            migrationBuilder.DropTable(
                name: "Sensors");

            migrationBuilder.DropTable(
                name: "Controllers");

            migrationBuilder.DropTable(
                name: "EdgeDevices");
        }
    }
}
