using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateSpecializationsToManyToMany : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorSpecializations_DoctorProfiles_DoctorProfileId",
                table: "DoctorSpecializations");

            migrationBuilder.DropIndex(
                name: "IX_DoctorSpecializations_DoctorProfileId",
                table: "DoctorSpecializations");

            migrationBuilder.DropColumn(
                name: "DoctorProfileId",
                table: "DoctorSpecializations");

            migrationBuilder.AddColumn<string>(
                name: "Description",
                table: "DoctorSpecializations",
                type: "nvarchar(max)",
                nullable: false,
                defaultValue: "");

            migrationBuilder.CreateTable(
                name: "DoctorProfileSpecializations",
                columns: table => new
                {
                    DoctorProfileId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SpecializationsId = table.Column<Guid>(type: "uniqueidentifier", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_DoctorProfileSpecializations", x => new { x.DoctorProfileId, x.SpecializationsId });
                    table.ForeignKey(
                        name: "FK_DoctorProfileSpecializations_DoctorProfiles_DoctorProfileId",
                        column: x => x.DoctorProfileId,
                        principalTable: "DoctorProfiles",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_DoctorProfileSpecializations_DoctorSpecializations_SpecializationsId",
                        column: x => x.SpecializationsId,
                        principalTable: "DoctorSpecializations",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_DoctorProfileSpecializations_SpecializationsId",
                table: "DoctorProfileSpecializations",
                column: "SpecializationsId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "DoctorProfileSpecializations");

            migrationBuilder.DropColumn(
                name: "Description",
                table: "DoctorSpecializations");

            migrationBuilder.AddColumn<Guid>(
                name: "DoctorProfileId",
                table: "DoctorSpecializations",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_DoctorSpecializations_DoctorProfileId",
                table: "DoctorSpecializations",
                column: "DoctorProfileId");

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorSpecializations_DoctorProfiles_DoctorProfileId",
                table: "DoctorSpecializations",
                column: "DoctorProfileId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
