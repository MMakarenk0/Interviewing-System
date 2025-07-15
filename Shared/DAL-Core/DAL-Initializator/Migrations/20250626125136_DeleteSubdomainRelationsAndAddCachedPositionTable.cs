using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Initializator.Migrations
{
    /// <inheritdoc />
    public partial class DeleteSubdomainRelationsAndAddCachedPositionTable : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Applications_Positions_PositionId",
                table: "Applications");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_AspNetUsers_InterviewerId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_Assessments_AspNetUsers_UserId",
                table: "Assessments");

            migrationBuilder.DropForeignKey(
                name: "FK_CandidateProfiles_AspNetUsers_UserId",
                table: "CandidateProfiles");

            migrationBuilder.DropForeignKey(
                name: "FK_Interviews_Applications_ApplicationId",
                table: "Interviews");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSlots_AspNetUsers_InterviewerId",
                table: "InterviewSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSlots_AspNetUsers_UserId",
                table: "InterviewSlots");

            migrationBuilder.DropForeignKey(
                name: "FK_InterviewSlots_Positions_PositionId1",
                table: "InterviewSlots");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSlots_InterviewerId",
                table: "InterviewSlots");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSlots_PositionId1",
                table: "InterviewSlots");

            migrationBuilder.DropIndex(
                name: "IX_InterviewSlots_UserId",
                table: "InterviewSlots");

            migrationBuilder.DropIndex(
                name: "IX_Interviews_ApplicationId",
                table: "Interviews");

            migrationBuilder.DropIndex(
                name: "IX_CandidateProfiles_UserId",
                table: "CandidateProfiles");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_InterviewerId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Assessments_UserId",
                table: "Assessments");

            migrationBuilder.DropIndex(
                name: "IX_Applications_PositionId",
                table: "Applications");

            migrationBuilder.DropColumn(
                name: "PositionId1",
                table: "InterviewSlots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "InterviewSlots");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "Assessments");

            migrationBuilder.AddColumn<Guid>(
                name: "InterviewId",
                table: "Applications",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "CachedPositions",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Title = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_CachedPositions", x => x.Id);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "CachedPositions");

            migrationBuilder.DropColumn(
                name: "InterviewId",
                table: "Applications");

            migrationBuilder.AddColumn<Guid>(
                name: "PositionId1",
                table: "InterviewSlots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "InterviewSlots",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "Assessments",
                type: "uniqueidentifier",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSlots_InterviewerId",
                table: "InterviewSlots",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSlots_PositionId1",
                table: "InterviewSlots",
                column: "PositionId1");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewSlots_UserId",
                table: "InterviewSlots",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Interviews_ApplicationId",
                table: "Interviews",
                column: "ApplicationId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_CandidateProfiles_UserId",
                table: "CandidateProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_InterviewerId",
                table: "Assessments",
                column: "InterviewerId");

            migrationBuilder.CreateIndex(
                name: "IX_Assessments_UserId",
                table: "Assessments",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_Applications_PositionId",
                table: "Applications",
                column: "PositionId");

            migrationBuilder.AddForeignKey(
                name: "FK_Applications_Positions_PositionId",
                table: "Applications",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_AspNetUsers_InterviewerId",
                table: "Assessments",
                column: "InterviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_Assessments_AspNetUsers_UserId",
                table: "Assessments",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_CandidateProfiles_AspNetUsers_UserId",
                table: "CandidateProfiles",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_Interviews_Applications_ApplicationId",
                table: "Interviews",
                column: "ApplicationId",
                principalTable: "Applications",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSlots_AspNetUsers_InterviewerId",
                table: "InterviewSlots",
                column: "InterviewerId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSlots_AspNetUsers_UserId",
                table: "InterviewSlots",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id");

            migrationBuilder.AddForeignKey(
                name: "FK_InterviewSlots_Positions_PositionId1",
                table: "InterviewSlots",
                column: "PositionId1",
                principalTable: "Positions",
                principalColumn: "Id");
        }
    }
}
