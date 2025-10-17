using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace eVote360.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class Seeder : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_BallotOptions_ElectionBallots_ElectionBallotId",
                table: "BallotOptions");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionBallots_Elections_ElectionId",
                table: "ElectionBallots");

            migrationBuilder.DropForeignKey(
                name: "FK_ElectionBallots_Positions_PositionId",
                table: "ElectionBallots");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyAssignments_Parties_PartyId",
                table: "PartyAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_VoteItems_ElectionBallots_ElectionBallotId",
                table: "VoteItems");

            migrationBuilder.DropIndex(
                name: "IX_VoteItems_ElectionBallotId",
                table: "VoteItems");

            migrationBuilder.DropIndex(
                name: "IX_VoteItems_VoteId_ElectionBallotId",
                table: "VoteItems");

            migrationBuilder.DropIndex(
                name: "IX_PartyAssignments_UserId",
                table: "PartyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_ElectionBallots_PositionId",
                table: "ElectionBallots");

            migrationBuilder.DropIndex(
                name: "IX_BallotOptions_ElectionBallotId_CandidateId_PartyId_IsNinguno",
                table: "BallotOptions");

            migrationBuilder.DropColumn(
                name: "UserId",
                table: "PartyAssignments");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "ElectionBallots");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "ElectionBallots");

            migrationBuilder.DropColumn(
                name: "CreatedAt",
                table: "BallotOptions");

            migrationBuilder.DropColumn(
                name: "UpdatedAt",
                table: "BallotOptions");

            migrationBuilder.RenameColumn(
                name: "ElectionBallotId",
                table: "BallotOptions",
                newName: "PositionId");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptHash",
                table: "Votes",
                type: "nvarchar(max)",
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(200)",
                oldMaxLength: 200);

            migrationBuilder.AddColumn<bool>(
                name: "Activo",
                table: "PartyAssignments",
                type: "bit",
                nullable: false,
                defaultValue: false);

            migrationBuilder.AddColumn<int>(
                name: "UsuarioId",
                table: "PartyAssignments",
                type: "int",
                nullable: false,
                defaultValue: 0);

            migrationBuilder.AddColumn<Guid>(
                name: "ElectionId",
                table: "BallotOptions",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.CreateTable(
                name: "Candidaturas",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    CandidateId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PositionId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    PartyId = table.Column<Guid>(type: "uniqueidentifier", nullable: false),
                    IsActive = table.Column<bool>(type: "bit", nullable: false),
                    CreatedAt = table.Column<DateTime>(type: "datetime2", nullable: false),
                    UpdatedAt = table.Column<DateTime>(type: "datetime2", nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Candidaturas", x => x.Id);
                });

            migrationBuilder.CreateTable(
                name: "Usuarios",
                columns: table => new
                {
                    Id = table.Column<int>(type: "int", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    Nombre = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Apellido = table.Column<string>(type: "nvarchar(60)", maxLength: 60, nullable: false),
                    Email = table.Column<string>(type: "nvarchar(120)", maxLength: 120, nullable: false),
                    NombreUsuario = table.Column<string>(type: "nvarchar(40)", maxLength: 40, nullable: false),
                    PasswordHash = table.Column<string>(type: "nvarchar(max)", nullable: false),
                    Rol = table.Column<string>(type: "nvarchar(20)", maxLength: 20, nullable: false),
                    Activo = table.Column<bool>(type: "bit", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_Usuarios", x => x.Id);
                });

            migrationBuilder.CreateIndex(
                name: "IX_VoteItems_VoteId_BallotOptionId",
                table: "VoteItems",
                columns: new[] { "VoteId", "BallotOptionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyAssignments_Id",
                table: "PartyAssignments",
                column: "Id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyAssignments_UsuarioId",
                table: "PartyAssignments",
                column: "UsuarioId");

            migrationBuilder.CreateIndex(
                name: "IX_BallotOptions_ElectionId_PositionId_CandidateId",
                table: "BallotOptions",
                columns: new[] { "ElectionId", "PositionId", "CandidateId" },
                unique: true,
                filter: "[CandidateId] IS NOT NULL");

            migrationBuilder.CreateIndex(
                name: "IX_BallotOptions_ElectionId_PositionId_IsNinguno",
                table: "BallotOptions",
                columns: new[] { "ElectionId", "PositionId", "IsNinguno" },
                unique: true,
                filter: "[IsNinguno] = 1");

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_PartyId_CandidateId",
                table: "Candidaturas",
                columns: new[] { "PartyId", "CandidateId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Candidaturas_PartyId_PositionId",
                table: "Candidaturas",
                columns: new[] { "PartyId", "PositionId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_Email",
                table: "Usuarios",
                column: "Email",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_Usuarios_NombreUsuario",
                table: "Usuarios",
                column: "NombreUsuario",
                unique: true);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyAssignments_Parties_PartyId",
                table: "PartyAssignments",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyAssignments_Usuarios_UsuarioId",
                table: "PartyAssignments",
                column: "UsuarioId",
                principalTable: "Usuarios",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropForeignKey(
                name: "FK_PartyAssignments_Parties_PartyId",
                table: "PartyAssignments");

            migrationBuilder.DropForeignKey(
                name: "FK_PartyAssignments_Usuarios_UsuarioId",
                table: "PartyAssignments");

            migrationBuilder.DropTable(
                name: "Candidaturas");

            migrationBuilder.DropTable(
                name: "Usuarios");

            migrationBuilder.DropIndex(
                name: "IX_VoteItems_VoteId_BallotOptionId",
                table: "VoteItems");

            migrationBuilder.DropIndex(
                name: "IX_PartyAssignments_Id",
                table: "PartyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_PartyAssignments_UsuarioId",
                table: "PartyAssignments");

            migrationBuilder.DropIndex(
                name: "IX_BallotOptions_ElectionId_PositionId_CandidateId",
                table: "BallotOptions");

            migrationBuilder.DropIndex(
                name: "IX_BallotOptions_ElectionId_PositionId_IsNinguno",
                table: "BallotOptions");

            migrationBuilder.DropColumn(
                name: "Activo",
                table: "PartyAssignments");

            migrationBuilder.DropColumn(
                name: "UsuarioId",
                table: "PartyAssignments");

            migrationBuilder.DropColumn(
                name: "ElectionId",
                table: "BallotOptions");

            migrationBuilder.RenameColumn(
                name: "PositionId",
                table: "BallotOptions",
                newName: "ElectionBallotId");

            migrationBuilder.AlterColumn<string>(
                name: "ReceiptHash",
                table: "Votes",
                type: "nvarchar(200)",
                maxLength: 200,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(max)");

            migrationBuilder.AddColumn<Guid>(
                name: "UserId",
                table: "PartyAssignments",
                type: "uniqueidentifier",
                nullable: false,
                defaultValue: new Guid("00000000-0000-0000-0000-000000000000"));

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "ElectionBallots",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "ElectionBallots",
                type: "datetime2",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "CreatedAt",
                table: "BallotOptions",
                type: "datetime2",
                nullable: false,
                defaultValue: new DateTime(1, 1, 1, 0, 0, 0, 0, DateTimeKind.Unspecified));

            migrationBuilder.AddColumn<DateTime>(
                name: "UpdatedAt",
                table: "BallotOptions",
                type: "datetime2",
                nullable: true);

            migrationBuilder.CreateIndex(
                name: "IX_VoteItems_ElectionBallotId",
                table: "VoteItems",
                column: "ElectionBallotId");

            migrationBuilder.CreateIndex(
                name: "IX_VoteItems_VoteId_ElectionBallotId",
                table: "VoteItems",
                columns: new[] { "VoteId", "ElectionBallotId" },
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_PartyAssignments_UserId",
                table: "PartyAssignments",
                column: "UserId",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_ElectionBallots_PositionId",
                table: "ElectionBallots",
                column: "PositionId");

            migrationBuilder.CreateIndex(
                name: "IX_BallotOptions_ElectionBallotId_CandidateId_PartyId_IsNinguno",
                table: "BallotOptions",
                columns: new[] { "ElectionBallotId", "CandidateId", "PartyId", "IsNinguno" },
                unique: true,
                filter: "[CandidateId] IS NOT NULL AND [PartyId] IS NOT NULL");

            migrationBuilder.AddForeignKey(
                name: "FK_BallotOptions_ElectionBallots_ElectionBallotId",
                table: "BallotOptions",
                column: "ElectionBallotId",
                principalTable: "ElectionBallots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionBallots_Elections_ElectionId",
                table: "ElectionBallots",
                column: "ElectionId",
                principalTable: "Elections",
                principalColumn: "Id",
                onDelete: ReferentialAction.Cascade);

            migrationBuilder.AddForeignKey(
                name: "FK_ElectionBallots_Positions_PositionId",
                table: "ElectionBallots",
                column: "PositionId",
                principalTable: "Positions",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_PartyAssignments_Parties_PartyId",
                table: "PartyAssignments",
                column: "PartyId",
                principalTable: "Parties",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);

            migrationBuilder.AddForeignKey(
                name: "FK_VoteItems_ElectionBallots_ElectionBallotId",
                table: "VoteItems",
                column: "ElectionBallotId",
                principalTable: "ElectionBallots",
                principalColumn: "Id",
                onDelete: ReferentialAction.Restrict);
        }
    }
}
