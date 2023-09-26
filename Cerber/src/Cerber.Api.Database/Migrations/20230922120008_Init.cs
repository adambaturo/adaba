using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace Cerber.Api.Database.Migrations
{
    /// <inheritdoc />
    public partial class Init : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "product",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    name = table.Column<string>(type: "nvarchar(50)", maxLength: 50, nullable: false),
                    description = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_product", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "heartbeat",
                columns: table => new
                {
                    id = table.Column<long>(type: "bigint", nullable: false)
                        .Annotation("SqlServer:Identity", "1, 1"),
                    product_id = table.Column<long>(type: "bigint", nullable: false),
                    version = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    instance = table.Column<string>(type: "nvarchar(200)", maxLength: 200, nullable: true),
                    client_timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: true),
                    server_timestamp = table.Column<DateTimeOffset>(type: "datetimeoffset", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("pk_heartbeat", x => x.id);
                    table.ForeignKey(
                        name: "FK_heartbeat_product_product_id",
                        column: x => x.product_id,
                        principalTable: "product",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Cascade);
                });

            migrationBuilder.CreateIndex(
                name: "ix_heartbeat_product_instance_version",
                table: "heartbeat",
                columns: new[] { "product_id", "instance", "version" });

            migrationBuilder.CreateIndex(
                name: "ix_product_name",
                table: "product",
                column: "name",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "heartbeat");

            migrationBuilder.DropTable(
                name: "product");
        }
    }
}
