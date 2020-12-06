using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class FixedSelectedFilmLists : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilmSelectionList_AspNetUsers_UserId",
                table: "FilmSelectionList");

            migrationBuilder.DropIndex(
                name: "IX_FilmSelectionList_UserId",
                table: "FilmSelectionList");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "FilmSelectionList");

            migrationBuilder.AddColumn<Guid>(
                name: "FilmId",
                table: "FilmSelectionList",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateIndex(
                name: "IX_FilmSelectionList_FilmId",
                table: "FilmSelectionList",
                column: "FilmId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilmSelectionList_Films_FilmId",
                table: "FilmSelectionList",
                column: "FilmId",
                principalTable: "Films",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_FilmSelectionList_Films_FilmId",
                table: "FilmSelectionList");

            migrationBuilder.DropIndex(
                name: "IX_FilmSelectionList_FilmId",
                table: "FilmSelectionList");

            migrationBuilder.DropColumn(
                name: "FilmId",
                table: "FilmSelectionList");

            migrationBuilder.AddColumn<string>(
                name: "UserId",
                table: "FilmSelectionList",
                type: "nvarchar(450)",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_FilmSelectionList_UserId",
                table: "FilmSelectionList",
                column: "UserId");

            migrationBuilder.AddForeignKey(
                name: "FK_FilmSelectionList_AspNetUsers_UserId",
                table: "FilmSelectionList",
                column: "UserId",
                principalTable: "AspNetUsers",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
