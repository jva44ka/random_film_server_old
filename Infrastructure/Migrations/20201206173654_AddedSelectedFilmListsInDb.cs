using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Infrastructure.Migrations
{
    public partial class AddedSelectedFilmListsInDb : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "SelectionList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    AlgorithmType = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_SelectionList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_SelectionList_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "FilmSelectionList",
                columns: table => new
                {
                    Id = table.Column<Guid>(nullable: false),
                    UserId = table.Column<string>(nullable: true),
                    SelectionListId = table.Column<Guid>(nullable: false),
                    Order = table.Column<int>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_FilmSelectionList", x => x.Id);
                    table.ForeignKey(
                        name: "FK_FilmSelectionList_SelectionList_SelectionListId",
                        column: x => x.SelectionListId,
                        principalTable: "SelectionList",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Cascade);
                    table.ForeignKey(
                        name: "FK_FilmSelectionList_AspNetUsers_UserId",
                        column: x => x.UserId,
                        principalTable: "AspNetUsers",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_FilmSelectionList_SelectionListId",
                table: "FilmSelectionList",
                column: "SelectionListId");

            migrationBuilder.CreateIndex(
                name: "IX_FilmSelectionList_UserId",
                table: "FilmSelectionList",
                column: "UserId");

            migrationBuilder.CreateIndex(
                name: "IX_SelectionList_UserId",
                table: "SelectionList",
                column: "UserId");
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "FilmSelectionList");

            migrationBuilder.DropTable(
                name: "SelectionList");
        }
    }
}
