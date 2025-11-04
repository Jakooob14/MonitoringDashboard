using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddServiceCheckTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MonitoredServices",
                type: "character varying(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "text");

            migrationBuilder.AddColumn<int>(
                name: "CheckIntervalSeconds",
                table: "MonitoredServices",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<bool>(
                name: "IsActive",
                table: "MonitoredServices",
                type: "boolean",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<string>(
                name: "Url",
                table: "MonitoredServices",
                type: "character varying(1024)",
                maxLength: 1024,
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "ServiceChecks",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoredServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    CheckedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    IsSuccessful = table.Column<bool>(type: "boolean", nullable: false),
                    ResponseTimeMilliseconds = table.Column<int>(type: "integer", nullable: false),
                    StatusCode = table.Column<int>(type: "integer", nullable: false),
                    ResponseContentSnippet = table.Column<string>(type: "character varying(2048)", maxLength: 2048, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ServiceChecks", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ServiceChecks_MonitoredServices_MonitoredServiceId",
                        column: x => x.MonitoredServiceId,
                        principalTable: "MonitoredServices",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ServiceChecks_MonitoredServiceId",
                table: "ServiceChecks",
                column: "MonitoredServiceId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ServiceChecks");

            migrationBuilder.DropColumn(
                name: "CheckIntervalSeconds",
                table: "MonitoredServices");

            migrationBuilder.DropColumn(
                name: "IsActive",
                table: "MonitoredServices");

            migrationBuilder.DropColumn(
                name: "Url",
                table: "MonitoredServices");

            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "MonitoredServices",
                type: "text",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "character varying(128)",
                oldMaxLength: 128);
        }
    }
}
