using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace OutfitTrack.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSomeProperties : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "variação",
                table: "pedido_item",
                newName: "variacao");

            migrationBuilder.RenameColumn(
                name: "quantidade",
                table: "pedido_item",
                newName: "item");

            migrationBuilder.AlterColumn<string>(
                name: "variacao",
                table: "pedido_item",
                type: "VARCHAR(50)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.AlterColumn<string>(
                name: "observacao",
                table: "pedido",
                type: "VARCHAR(500)",
                nullable: true,
                oldClrType: typeof(string),
                oldType: "VARCHAR(150)")
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.RenameColumn(
                name: "variacao",
                table: "pedido_item",
                newName: "variação");

            migrationBuilder.RenameColumn(
                name: "item",
                table: "pedido_item",
                newName: "quantidade");

            migrationBuilder.UpdateData(
                table: "pedido_item",
                keyColumn: "variação",
                keyValue: null,
                column: "variação",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "variação",
                table: "pedido_item",
                type: "VARCHAR(50)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(50)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");

            migrationBuilder.UpdateData(
                table: "pedido",
                keyColumn: "observacao",
                keyValue: null,
                column: "observacao",
                value: "");

            migrationBuilder.AlterColumn<string>(
                name: "observacao",
                table: "pedido",
                type: "VARCHAR(150)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "VARCHAR(500)",
                oldNullable: true)
                .Annotation("MySql:CharSet", "utf8mb4")
                .OldAnnotation("MySql:CharSet", "utf8mb4");
        }
    }
}
