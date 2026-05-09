using System;
using Microsoft.EntityFrameworkCore.Migrations;
using Npgsql.EntityFrameworkCore.PostgreSQL.Metadata;

#nullable disable

namespace ApiTier7Provision.Infrastructure.Persistence.Migrations
{
    /// <inheritdoc />
    public partial class AddSpotProvisioning : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "model_templates",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    model = table.Column<string>(type: "text", nullable: false),
                    supported_gpus = table.Column<string[]>(type: "text[]", nullable: false),
                    provisioning_template_uuid = table.Column<string>(type: "text", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_model_templates", x => x.id);
                });

            migrationBuilder.CreateTable(
                name: "instances",
                columns: table => new
                {
                    id = table.Column<int>(type: "integer", nullable: false)
                        .Annotation("Npgsql:ValueGenerationStrategy", NpgsqlValueGenerationStrategy.IdentityByDefaultColumn),
                    model_template_id = table.Column<int>(type: "integer", nullable: false),
                    vast_instance_id = table.Column<long>(type: "bigint", nullable: false),
                    gpu = table.Column<string>(type: "text", nullable: false),
                    total_cost_per_hour = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    ingress_cost = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    egress_cost = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    bandwidth_cost = table.Column<decimal>(type: "numeric(18,6)", precision: 18, scale: 6, nullable: false),
                    provisioned_at_utc = table.Column<DateTime>(type: "timestamp with time zone", nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_instances", x => x.id);
                    table.ForeignKey(
                        name: "FK_instances_model_templates_model_template_id",
                        column: x => x.model_template_id,
                        principalTable: "model_templates",
                        principalColumn: "id",
                        onDelete: ReferentialAction.Restrict);
                });

            migrationBuilder.CreateIndex(
                name: "IX_instances_model_template_id",
                table: "instances",
                column: "model_template_id");

            migrationBuilder.CreateIndex(
                name: "IX_instances_vast_instance_id",
                table: "instances",
                column: "vast_instance_id",
                unique: true);

            migrationBuilder.CreateIndex(
                name: "IX_model_templates_model",
                table: "model_templates",
                column: "model",
                unique: true);
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "instances");

            migrationBuilder.DropTable(
                name: "model_templates");
        }
    }
}
