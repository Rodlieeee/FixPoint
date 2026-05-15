using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace FixPoint.Data.Migrations
{
    /// <inheritdoc />
    public partial class AddFeedbackToAssignment : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<string>(
                name: "FeedbackNotes",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "FeedbackSubmittedAt",
                table: "Assignments",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "ProofPhotoPath",
                table: "Assignments",
                type: "nvarchar(max)",
                nullable: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "FeedbackNotes",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "FeedbackSubmittedAt",
                table: "Assignments");

            migrationBuilder.DropColumn(
                name: "ProofPhotoPath",
                table: "Assignments");
        }
    }
}
