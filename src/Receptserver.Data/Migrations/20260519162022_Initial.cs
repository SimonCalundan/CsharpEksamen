using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace Receptserver.Data.Migrations
{
    /// <inheritdoc />
    public partial class Initial : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Apoteker",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Navn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Apoteker", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Laegehuse",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ydernummer = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    Navn = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Adresse = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Laegehuse", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Recepter",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Ydernummer = table.Column<string>(type: "TEXT", maxLength: 50, nullable: false),
                    CprNummer = table.Column<string>(type: "TEXT", maxLength: 11, nullable: false),
                    Oprettelsesdato = table.Column<DateTime>(type: "TEXT", nullable: false),
                    Lukket = table.Column<bool>(type: "INTEGER", nullable: false),
                    TilknyttetApotekId = table.Column<int>(type: "INTEGER", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Recepter", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Recepter_Apoteker_TilknyttetApotekId",
                        column: x => x.TilknyttetApotekId,
                        principalTable: "Apoteker",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "Ordinationer",
                columns: table => new
                {
                    Id = table.Column<int>(type: "INTEGER", nullable: false)
                        .Annotation("Sqlite:Autoincrement", true),
                    Laegemiddel = table.Column<string>(type: "TEXT", maxLength: 200, nullable: false),
                    Dosis = table.Column<string>(type: "TEXT", maxLength: 300, nullable: false),
                    AntalUdleveringer = table.Column<int>(type: "INTEGER", nullable: false),
                    AntalForetagneUdleveringer = table.Column<int>(type: "INTEGER", nullable: false),
                    ReceptId = table.Column<int>(type: "INTEGER", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ordinationer", x => x.Id);
                    table.ForeignKey(
                        name: "FK_Ordinationer_Recepter_ReceptId",
                        column: x => x.ReceptId,
                        principalTable: "Recepter",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.InsertData(
                table: "Apoteker",
                columns: new[] { "Id", "Adresse", "Navn" },
                values: new object[,]
                {
                    { 1, "Store Torv 3, 8000 Aarhus", "Aarhus Apotek" },
                    { 2, "Stadionalle 1, 8240 Risskov", "Risskov Apotek" },
                    { 3, "Skanderborgvej 220, 8260 Viby", "Viby Apotek" }
                });

            migrationBuilder.InsertData(
                table: "Laegehuse",
                columns: new[] { "Id", "Adresse", "Navn", "Ydernummer" },
                values: new object[,]
                {
                    { 1, "Banegårdsgade 1, 8000 Aarhus", "Lægerne i Aarhus C", "012345" },
                    { 2, "Skovvejen 12, 8240 Risskov", "Risskov Lægehus", "054321" },
                    { 3, "Skanderborgvej 200, 8260 Viby", "Viby Lægehus", "067890" }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Laegehuse_Ydernummer",
                table: "Laegehuse",
                column: "Ydernummer",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Ordinationer_ReceptId",
                table: "Ordinationer",
                column: "ReceptId");

            migrationBuilder.CreateIndex(
                name: "IX_Recepter_CprNummer",
                table: "Recepter",
                column: "CprNummer");

            migrationBuilder.CreateIndex(
                name: "IX_Recepter_TilknyttetApotekId",
                table: "Recepter",
                column: "TilknyttetApotekId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Laegehuse");

            migrationBuilder.DropTable(
                name: "Ordinationer");

            migrationBuilder.DropTable(
                name: "Recepter");

            migrationBuilder.DropTable(
                name: "Apoteker");
        }
    }
}
