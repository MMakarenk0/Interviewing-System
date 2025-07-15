using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Initializator.Migrations
{
    /// <inheritdoc />
    public partial class ResumeTableDeleteMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Resumes_ResumeId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Resumes_ProfileResumeId",
                table: "CandidateProfiles");

            migrationBuilder.DropTable(
                name: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_ProfileResumeId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ResumeId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ProfileResumeId",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeId",
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

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "ProfileResumeFileName",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "ResumeFileName",
                table: "Applications");

            migrationBuilder.AddColumn<Guid>(
                name: "ProfileResumeId",
                table: "CandidateProfiles",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateTable(
                name: "Resumes",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    FileName = table.Column<string>(type: "nvarchar(255)", maxLength: 255, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Resumes", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ProfileResumeId",
                table: "CandidateProfiles",
                column: "ProfileResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ResumeId",
                table: "Applications",
                column: "ResumeId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Resumes_ResumeId",
                table: "Applications",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_Resumes_ProfileResumeId",
                table: "CandidateProfiles",
                column: "ProfileResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.SetNull);
        }
    }
}
