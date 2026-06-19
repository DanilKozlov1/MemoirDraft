using System.Collections.Generic;
using MemoirDraft.Database.DTO;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace MemoirDraft.Migrations
{
    /// <inheritdoc />
    public partial class AddJsonbConverter : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "TodoItems",
                table: "Notes",
                type: "jsonb",
                nullable: true,
                oldClrType: typeof(List<TodoItem>),
                oldType: "jsonb",
                oldNullable: true,
                oldDefaultValueSql: "'[]'::jsonb");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<List<TodoItem>>(
                name: "TodoItems",
                table: "Notes",
                type: "jsonb",
                nullable: true,
                defaultValueSql: "'[]'::jsonb",
                oldClrType: typeof(string),
                oldType: "jsonb",
                oldNullable: true);
        }
    }
}
