using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace LinnworksTest.Migrations
{
    public partial class ChangeTokenValueType : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<Guid>(
                name: "Value",
                table: "Tokens",
                nullable: false,
                oldClrType: typeof(string),
                oldNullable: true);
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Value",
                table: "Tokens",
                nullable: true,
                oldClrType: typeof(Guid));
        }
    }
}
