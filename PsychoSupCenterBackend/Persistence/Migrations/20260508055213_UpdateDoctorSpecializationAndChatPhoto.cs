using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Persistence.Migrations
{
    /// <inheritdoc />
    public partial class UpdateDoctorSpecializationAndChatPhoto : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfileSpecializations_DoctorProfiles_DoctorProfileId",
                table: "DoctorProfileSpecializations");

            migrationBuilder.RenameColumn(
                name: "DoctorProfileId",
                table: "DoctorProfileSpecializations",
                newName: "DoctorProfilesId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ChatMessages",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: true,
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000);

            migrationBuilder.AddColumn<string>(
                name: "PhotoUrl",
                table: "ChatMessages",
                type: "nvarchar(2048)",
                maxLength: 2048,
                nullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfileSpecializations_DoctorProfiles_DoctorProfilesId",
                table: "DoctorProfileSpecializations",
                column: "DoctorProfilesId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_DoctorProfileSpecializations_DoctorProfiles_DoctorProfilesId",
                table: "DoctorProfileSpecializations");

            migrationBuilder.DropColumn(
                name: "PhotoUrl",
                table: "ChatMessages");

            migrationBuilder.RenameColumn(
                name: "DoctorProfilesId",
                table: "DoctorProfileSpecializations",
                newName: "DoctorProfileId");

            migrationBuilder.AlterColumn<string>(
                name: "Content",
                table: "ChatMessages",
                type: "nvarchar(4000)",
                maxLength: 4000,
                nullable: false,
                defaultValue: "",
                oldClrType: typeof(string),
                oldType: "nvarchar(4000)",
                oldMaxLength: 4000,
                oldNullable: true);

            migrationBuilder.AddForeignKey(
                name: "FK_DoctorProfileSpecializations_DoctorProfiles_DoctorProfileId",
                table: "DoctorProfileSpecializations",
                column: "DoctorProfileId",
                principalTable: "DoctorProfiles",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
