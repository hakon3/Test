using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Second.Migrations
{
    /// <inheritdoc />
    public partial class initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Cities",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Country = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Latitude = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Longitude = table.Column<string>(type: "nvarchar(max)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Cities", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "CityWeather",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Time = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false),
                    Temperature = table.Column<double>(type: "float", nullable: false),
                    WindSpeed = table.Column<double>(type: "float", nullable: false),
                    Cloudiness = table.Column<int>(type: "int", nullable: false),
                    CityNavigationId = table.Column<int>(type: "int", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CityWeather", x => x.Id);
                    table.ForeignKey(
                        name: "FK_CityWeather_Cities_CityNavigationId",
                        column: x => x.CityNavigationId,
                        principalTable: "Cities",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 1, "Norway", "59.91", "10.75", "Oslo" },
                    { 2, "Sweden", "59.33", "18.07", "Stockholm" },
                    { 3, "Denmark", "55.68", "12.57", "Copenhagen" },
                    { 4, "Finland", "60.17", "24.94", "Helsinki" },
                    { 5, "Spain", "40.42", "-3.70", "Madrid" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_CityWeather_CityNavigationId",
                table: "CityWeather",
                column: "CityNavigationId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CityWeather");

            migrationBuilder.DropTable(
                name: "Cities");
        }
    }
}
