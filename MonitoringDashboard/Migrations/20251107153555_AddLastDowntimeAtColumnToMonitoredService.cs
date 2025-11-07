using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddLastDowntimeAtColumnToMonitoredService : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "LastDowntimeAt",
                table: "MonitoredServices",
                type: "timestamp with time zone",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "LastDowntimeAt",
                table: "MonitoredServices");
        }
    }
}
