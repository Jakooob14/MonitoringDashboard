using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class UpdatedMonitoredServiceToManyInMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Maintenances_MonitoredServices_MonitoredServiceId",
                table: "Maintenances");

            migrationBuilder.DropIndex(
                name: "IX_Maintenances_MonitoredServiceId",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "MonitoredServiceId",
                table: "Maintenances");

            migrationBuilder.AddColumn<Guid>(
                name: "MaintenanceId",
                table: "MonitoredServices",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_MonitoredServices_MaintenanceId",
                table: "MonitoredServices",
                column: "MaintenanceId");

            migrationBuilder.AddForeignKey(
                name: "FK_MonitoredServices_Maintenances_MaintenanceId",
                table: "MonitoredServices",
                column: "MaintenanceId",
                principalTable: "Maintenances",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_MonitoredServices_Maintenances_MaintenanceId",
                table: "MonitoredServices");

            migrationBuilder.DropIndex(
                name: "IX_MonitoredServices_MaintenanceId",
                table: "MonitoredServices");

            migrationBuilder.DropColumn(
                name: "MaintenanceId",
                table: "MonitoredServices");

            migrationBuilder.AddColumn<Guid>(
                name: "MonitoredServiceId",
                table: "Maintenances",
                type: "uuid",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Maintenances_MonitoredServiceId",
                table: "Maintenances",
                column: "MonitoredServiceId");

            migrationBuilder.AddForeignKey(
                name: "FK_Maintenances_MonitoredServices_MonitoredServiceId",
                table: "Maintenances",
                column: "MonitoredServiceId",
                principalTable: "MonitoredServices",
                principalColumn: "Id");
        }
    }
}
