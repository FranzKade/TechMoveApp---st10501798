using System;
using Microsoft.EntityFrameworkCore.Migrations;

namespace TechMoveApp.Migrations
{
    /// <inheritdoc />
    public partial class BaselineSchema : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // LEAVE EMPTY: Tables already exist
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            // LEAVE EMPTY: Prevents accidental dropping of live tables during a rollback
        }
    }
}