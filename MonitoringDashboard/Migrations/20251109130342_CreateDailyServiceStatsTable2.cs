using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MonitoringDashboard.Migrations
{
    /// <inheritdoc />
    public partial class CreateDailyServiceStatsTable2 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "DailyServiceStats",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    MonitoredServiceId = table.Column<Guid>(type: "uuid", nullable: false),
                    Date = table.Column<DateTime>(type: "timestamp with time zone", nullable: false),
                    TotalChecks = table.Column<int>(type: "integer", nullable: false),
                    SuccessfulChecks = table.Column<int>(type: "integer", nullable: false),
                    FailedChecks = table.Column<int>(type: "integer", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DailyServiceStats", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DailyServiceStats");
        }
    }
}
