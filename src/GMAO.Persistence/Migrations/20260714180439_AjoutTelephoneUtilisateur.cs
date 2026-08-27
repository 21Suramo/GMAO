using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace GMAO.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AjoutTelephoneUtilisateur : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "Telephone",
                table: "Utilisateurs",
                type: "TEXT",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "Telephone",
                table: "Utilisateurs");
        }
    }
}
