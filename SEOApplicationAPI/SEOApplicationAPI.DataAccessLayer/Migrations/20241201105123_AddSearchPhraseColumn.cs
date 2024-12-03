using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace SEOApplicationAPI.DataAccessLayer.Migrations
{
    /// <inheritdoc />
    public partial class AddSearchPhraseColumn : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "SearchPhrase",
                table: "Rankings",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "SearchPhrase",
                table: "Rankings");
        }
    }
}
