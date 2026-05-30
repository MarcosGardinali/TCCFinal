using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutfitTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdatePostalCode : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "PostalCode",
                table: "cliente",
                newName: "cep");

            migrationBuilder.AlterColumn<string>(
                name: "cep",
                table: "cliente",
                type: "VARCHAR(8)",
                maxLength: 8,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "longtext",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "cep",
                table: "cliente",
                newName: "PostalCode");

            migrationBuilder.AlterColumn<string>(
                name: "PostalCode",
                table: "cliente",
                type: "longtext",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(8)",
                oldMaxLength: 8,
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
