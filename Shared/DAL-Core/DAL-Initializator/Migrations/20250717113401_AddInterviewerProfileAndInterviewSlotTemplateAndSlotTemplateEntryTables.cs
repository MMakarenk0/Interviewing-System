using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace DAL_Initializator.Migrations
{
    /// <inheritdoc />
    public partial class AddInterviewerProfileAndInterviewSlotTemplateAndSlotTemplateEntryTables : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "InterviewSlotTemplates",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    Name = table.Column<string>(type: "nvarchar(100)", maxLength: 100, nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewSlotTemplates", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "InterviewerProfiles",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    UserId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    SlotTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_InterviewerProfiles", x => x.Id);
                    table.ForeignKey(
                        name: "FK_InterviewerProfiles_InterviewSlotTemplates_SlotTemplateId",
                        column: x => x.SlotTemplateId,
                        principalTable: "InterviewSlotTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.SetNull);
                });

            migrationBuilder.CreateTable(
                name: "SlotTemplateEntries",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    InterviewSlotTemplateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    DayOfWeek = table.Column<int>(type: "int", nullable: false),
                    StartTime = table.Column<TimeOnly>(type: "time", nullable: false),
                    EndTime = table.Column<TimeOnly>(type: "time", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SlotTemplateEntries", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SlotTemplateEntries_InterviewSlotTemplates_InterviewSlotTemplateId",
                        column: x => x.InterviewSlotTemplateId,
                        principalTable: "InterviewSlotTemplates",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerProfiles_SlotTemplateId",
                table: "InterviewerProfiles",
                column: "SlotTemplateId");

            migrationBuilder.CreateIndex(
                name: "IX_InterviewerProfiles_UserId",
                table: "InterviewerProfiles",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_SlotTemplateEntries_InterviewSlotTemplateId",
                table: "SlotTemplateEntries",
                column: "InterviewSlotTemplateId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "InterviewerProfiles");

            migrationBuilder.DropTable(
                name: "SlotTemplateEntries");

            migrationBuilder.DropTable(
                name: "InterviewSlotTemplates");
        }
    }
}
