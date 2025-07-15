using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Initializator.Migrations
{
    /// <inheritdoc />
    public partial class RenameResumeFilePathRowName : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileResumeFileName",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "ProfileResumeBlobPath",
                table: "CandidateProfiles",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeBlobPath",
                table: "Applications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                defaultValue: "");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileResumeBlobPath",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeBlobPath",
                table: "Applications");

            migrationBuilder.AddColumn<string>(
                name: "ProfileResumeFileName",
                table: "CandidateProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ResumeFileName",
                table: "Applications",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);
        }
    }
}
