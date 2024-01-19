using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ibricks_mqtt_broker.Migrations
{
    /// <inheritdoc />
    public partial class _1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cellos",
                columns: table => new
                {
                    Mac = table.Column<string>(type: "TEXT", nullable: false),
                    Ip = table.Column<string>(type: "TEXT", nullable: false),
                    Description = table.Column<string>(type: "TEXT", nullable: false),
                    HardwareInfo = table.Column<string>(type: "TEXT", nullable: true),
                    RelayStates = table.Column<string>(type: "TEXT", nullable: false),
                    DimmerStates = table.Column<string>(type: "TEXT", nullable: false),
                    MeteoStates = table.Column<string>(type: "TEXT", nullable: false),
                    ClimateStates = table.Column<string>(type: "TEXT", nullable: false),
                    CoverStates = table.Column<string>(type: "TEXT", nullable: false),
                    EventStates = table.Column<string>(type: "TEXT", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cellos", x => x.Mac);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Cellos");
        }
    }
}
