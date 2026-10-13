using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceDesk.Tickets.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class InitialUserReferences : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "user_references",
                columns: table => new
                {
                    UserId = table.Column<string>(type: "text", nullable: false),
                    DisplayName = table.Column<string>(type: "text", nullable: false),
                    IsActive = table.Column<bool>(type: "boolean", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_user_references", x => x.UserId);
                });
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "user_references");
        }
    }
}
