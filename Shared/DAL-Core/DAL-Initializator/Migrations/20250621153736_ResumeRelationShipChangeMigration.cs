using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Initializator.Migrations
{
    /// <inheritdoc />
    public partial class ResumeRelationShipChangeMigration : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Positions_PositionId1",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Resumes_ResumeId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Resumes_ResumeId1",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_Interviews_InterviewId1",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_AspNetUsers_UserId1",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewSlots_SlotId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSlots_Interviews_InterviewId",
                table: "InterviewSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_Resumes_AspNetUsers_UserId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_Resumes_UserId",
                table: "Resumes");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSlots_InterviewId",
                table: "InterviewSlots");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_SlotId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_UserId1",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_InterviewId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_InterviewId1",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Applications_PositionId1",
                table: "Applications");

            migrationBuilder.DropIndex(
                name: "IX_Applications_ResumeId1",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Resumes");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "InterviewSlots");

            migrationBuilder.DropColumn(
                name: "InterviewId1",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "PositionId1",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "ResumeId1",
                table: "Applications");

            migrationBuilder.RenameColumn(
                name: "UserId1",
                table: "CandidateProfiles",
                newName: "ProfileResumeId");

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Interviews",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AddColumn<string>(
                name: "CurrentPosition",
                table: "CandidateProfiles",
                type: "nvarchar(100)",
                maxLength: 100,
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "TechStack",
                table: "CandidateProfiles",
                type: "nvarchar(255)",
                maxLength: 255,
                nullable: true);

            migrationBuilder.AddColumn<int>(
                name: "YearsOfExperience",
                table: "CandidateProfiles",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Applications",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(50)",
                oldMaxLength: 50);

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true,
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_SlotId",
                table: "Interviews",
                column: "SlotId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_ProfileResumeId",
                table: "CandidateProfiles",
                column: "ProfileResumeId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_InterviewId",
                table: "Assessments",
                column: "InterviewId",
                unique: true);

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

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewSlots_SlotId",
                table: "Interviews",
                column: "SlotId",
                principalTable: "InterviewSlots",
                principalColumn: "Id");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Resumes_ResumeId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_Resumes_ProfileResumeId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_InterviewSlots_SlotId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_SlotId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_ProfileResumeId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_InterviewId",
                table: "Assessments");

            migrationBuilder.DropColumn(
                name: "CurrentPosition",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "TechStack",
                table: "CandidateProfiles");

            migrationBuilder.DropColumn(
                name: "YearsOfExperience",
                table: "CandidateProfiles");

            migrationBuilder.RenameColumn(
                name: "ProfileResumeId",
                table: "CandidateProfiles",
                newName: "UserId1");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Resumes",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<Guid>(
                name: "InterviewId",
                table: "InterviewSlots",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Interviews",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "InterviewId1",
                table: "Assessments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AlterColumn<string>(
                name: "Status",
                table: "Applications",
                type: "nvarchar(50)",
                maxLength: 50,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AlterColumn<Guid>(
                name: "ResumeId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"),
                oldClrType: typeof(Guid),
                oldType: "uniqueidentifier",
                oldNullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId1",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "ResumeId1",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_Resumes_UserId",
                table: "Resumes",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSlots_InterviewId",
                table: "InterviewSlots",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_SlotId",
                table: "Interviews",
                column: "SlotId");

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_UserId1",
                table: "CandidateProfiles",
                column: "UserId1",
                unique: true,
                filter: "[UserId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_InterviewId",
                table: "Assessments",
                column: "InterviewId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_InterviewId1",
                table: "Assessments",
                column: "InterviewId1",
                unique: true,
                filter: "[InterviewId1] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_PositionId1",
                table: "Applications",
                column: "PositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_ResumeId1",
                table: "Applications",
                column: "ResumeId1");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Positions_PositionId1",
                table: "Applications",
                column: "PositionId1",
                principalTable: "Positions",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Resumes_ResumeId",
                table: "Applications",
                column: "ResumeId",
                principalTable: "Resumes",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Resumes_ResumeId1",
                table: "Applications",
                column: "ResumeId1",
                principalTable: "Resumes",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_Interviews_InterviewId1",
                table: "Assessments",
                column: "InterviewId1",
                principalTable: "Interviews",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_AspNetUsers_UserId1",
                table: "CandidateProfiles",
                column: "UserId1",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_InterviewSlots_SlotId",
                table: "Interviews",
                column: "SlotId",
                principalTable: "InterviewSlots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSlots_Interviews_InterviewId",
                table: "InterviewSlots",
                column: "InterviewId",
                principalTable: "Interviews",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Resumes_AspNetUsers_UserId",
                table: "Resumes",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }
    }
}
