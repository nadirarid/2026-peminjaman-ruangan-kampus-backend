using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

#pragma warning disable CA1814 // Prefer jagged arrays over multidimensional

namespace PeminjamanRuangan.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class InitialCreate : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "Ruangan",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRuangan = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    NamaRuangan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    Kapasitas = table.Column<int>(type: "int", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Tersedia"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true),
                    IsDeleted = table.Column<bool>(type: "bit", nullable: false, defaultValue: false),
                    DeletedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Ruangan", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Peminjaman",
                columns: table => new
                {
                    IdPeminjaman = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    IdRuangan = table.Column<int>(type: "int", nullable: false),
                    NamaUser = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IdUser = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    Keterangan = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    StartTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    EndTime = table.Column<DateTime>(type: "datetime2", nullable: false),
                    Status = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false, defaultValue: "Diproses"),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Peminjaman", x => x.IdPeminjaman);
                    table.ForeignKey(
                        name: "FK_Peminjaman_Ruangan_IdRuangan",
                        column: x => x.IdRuangan,
                        principalTable: "Ruangan",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.InsertData(
                table: "Ruangan",
                columns: new[] { "Id", "CreatedAt", "DeletedAt", "IdRuangan", "Kapasitas", "NamaRuangan", "Status", "UpdatedAt" },
                values: new object[,]
                {
                    { 1, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "A001", 30, "Lab Jaringan", "Tersedia", null },
                    { 2, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "A002", 30, "Lab Database", "Tersedia", null },
                    { 3, new DateTime(2026, 2, 17, 0, 0, 0, 0, DateTimeKind.Utc), null, "A003", 30, "Lab Perangkat Lunak", "Tersedia", null }
                });

            migrationBuilder.CreateIndex(
                name: "IX_Peminjaman_IdRuangan",
                table: "Peminjaman",
                column: "IdRuangan");

            migrationBuilder.CreateIndex(
                name: "IX_Ruangan_IdRuangan",
                table: "Ruangan",
                column: "IdRuangan",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "Peminjaman");

            migrationBuilder.DropTable(
                name: "Ruangan");
        }
    }
}
