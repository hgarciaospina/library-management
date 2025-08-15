using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace LibraryManagement.Infrastructure.Migrations
{
    /// <inheritdoc />
    public partial class AddLibraryIdToLoans : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<int>(
                name: "LibraryId",
                table: "Loans",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.CreateIndex(
                name: "IX_Loans_LibraryId",
                table: "Loans",
                column: "LibraryId");

            migrationBuilder.AddForeignKey(
                name: "FK_Loans_Libraries_LibraryId",
                table: "Loans",
                column: "LibraryId",
                principalTable: "Libraries",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_Loans_Libraries_LibraryId",
                table: "Loans");

            migrationBuilder.DropIndex(
                name: "IX_Loans_LibraryId",
                table: "Loans");

            migrationBuilder.DropColumn(
                name: "LibraryId",
                table: "Loans");
        }
    }
}
