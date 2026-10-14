using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ServiceDesk.Tickets.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddTicketsAndHistory : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "tickets",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    Number = table.Column<string>(type: "text", nullable: false),
                    Title = table.Column<string>(type: "text", nullable: false),
                    Description = table.Column<string>(type: "text", nullable: false),
                    RequesterId = table.Column<string>(type: "text", nullable: false),
                    State = table.Column<string>(type: "text", nullable: false),
                    IsSensitive = table.Column<bool>(type: "boolean", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_tickets", x => x.Id);
                    table.ForeignKey(
                        name: "FK_tickets_user_references_RequesterId",
                        column: x => x.RequesterId,
                        principalTable: "user_references",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateTable(
                name: "ticket_history",
                columns: table => new
                {
                    Id = table.Column<Guid>(type: "uuid", nullable: false),
                    TicketId = table.Column<Guid>(type: "uuid", nullable: false),
                    EventType = table.Column<string>(type: "text", nullable: false),
                    FromValue = table.Column<string>(type: "text", nullable: true),
                    ToValue = table.Column<string>(type: "text", nullable: true),
                    ActorId = table.Column<string>(type: "text", nullable: false),
                    OccurredAt = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_ticket_history", x => x.Id);
                    table.ForeignKey(
                        name: "FK_ticket_history_tickets_TicketId",
                        column: x => x.TicketId,
                        principalTable: "tickets",
                        principalColumn: "Id",
                        onDelete: ReferentialAction.Restrict);
                    table.ForeignKey(
                        name: "FK_ticket_history_user_references_ActorId",
                        column: x => x.ActorId,
                        principalTable: "user_references",
                        principalColumn: "UserId",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_ticket_history_ActorId",
                table: "ticket_history",
                column: "ActorId");

            migrationBuilder.CreateIndex(
                name: "IX_ticket_history_TicketId_OccurredAt",
                table: "ticket_history",
                columns: new[] { "TicketId", "OccurredAt" });

            migrationBuilder.CreateIndex(
                name: "IX_tickets_Number",
                table: "tickets",
                column: "Number",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_tickets_RequesterId",
                table: "tickets",
                column: "RequesterId");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "ticket_history");

            migrationBuilder.DropTable(
                name: "tickets");
        }
    }
}
