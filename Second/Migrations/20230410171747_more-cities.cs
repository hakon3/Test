using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Second.Migrations
{
    /// <inheritdoc />
    public partial class morecities : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.InsertData(
                table: "Cities",
                columns: new[] { "Id", "Country", "Latitude", "Longitude", "Name" },
                values: new object[,]
                {
                    { 6, "USA", "40.71", "-74.01", "New York" },
                    { 7, "South Africa", "-33.93", "18.42", "New York" },
                    { 8, "Japan", "35.69", "139.69", "Tokyo" },
                    { 9, "Afghanistan", "34.53", "69.17", "Kabul" },
                    { 10, "Brazil", "-22.91", "-43.18", "Rio de Janeiro" }
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 6);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 7);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 8);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 9);

            migrationBuilder.DeleteData(
                table: "Cities",
                keyColumn: "Id",
                keyValue: 10);
        }
    }
}
