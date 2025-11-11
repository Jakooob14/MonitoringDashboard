using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class UpdateIncidentsTableColumnNames : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartedAt",
                table: "Incidents",
                newName: "StartTime");

            migrationBuilder.RenameColumn(
                name: "EndedAt",
                table: "Incidents",
                newName: "EndTime");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "StartTime",
                table: "Incidents",
                newName: "StartedAt");

            migrationBuilder.RenameColumn(
                name: "EndTime",
                table: "Incidents",
                newName: "EndedAt");
        }
    }
}
