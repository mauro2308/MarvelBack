using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class prueba1 : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.EnsureSchema(
                name: "dbo");

            migrationBuilder.CreateTable(
                name: "Usuario",
                schema: "dbo",
                columns: table => new
                {
                    ID_Usuario = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "varchar(250)", nullable: false),
                    Identificacion = table.Column<string>(type: "varchar(250)", nullable: false),
                    Contrasena = table.Column<string>(type: "varchar(250)", nullable: false),
                    Correo = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuario", x => x.ID_Usuario);
                });

            migrationBuilder.CreateTable(
                name: "Comic_Favorito",
                schema: "dbo",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    ID_Usuario = table.Column<int>(type: "int", nullable: false),
                    ID_Comic = table.Column<int>(type: "int", nullable: false),
                    Titulo_Comic = table.Column<string>(type: "varchar(250)", nullable: false),
                    Descripcion_Comic = table.Column<string>(type: "varchar(250)", nullable: true),
                    Url_Comic_Imagen = table.Column<string>(type: "varchar(250)", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Id", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Comic_Favorito_Usuarios",
                        column: x => x.ID_Usuario,
                        principalSchema: "dbo",
                        principalTable: "Usuario",
                        principalColumn: "ID_Usuario");
                });

            migrationBuilder.CreateIndex(
                name: "IX_Comic_Favorito_ID_Usuario",
                schema: "dbo",
                table: "Comic_Favorito",
                column: "ID_Usuario");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Comic_Favorito",
                schema: "dbo");

            migrationBuilder.DropTable(
                name: "Usuario",
                schema: "dbo");
        }
    }
}
