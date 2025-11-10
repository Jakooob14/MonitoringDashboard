using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class AddStatusAndSeverityToMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "Severity",
                table: "Maintenances",
                type: "integer",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<int>(
                name: "Status",
                table: "Maintenances",
                type: "integer",
                nullable: false,
                defaultValue: 0);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Severity",
                table: "Maintenances");

            migrationBuilder.DropColumn(
                name: "Status",
                table: "Maintenances");
        }
    }
}
