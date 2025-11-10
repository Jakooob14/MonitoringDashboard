using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class RemoveIsCompletedFromMaintenance : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "IsCompleted",
                table: "Maintenances");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<bool>(
                name: "IsCompleted",
                table: "Maintenances",
                type: "boolean",
                nullable: false,
                defaultValue: false);
        }
    }
}
