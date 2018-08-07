using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using System;
using System.Collections.Generic;

namespace EveMoonminingTool.Migrations
{
    public partial class Initial : Migration
    {
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.CreateTable(
                name: "MiningJob",
                columns: table => new
                {
                    ID = table.Column<int>(nullable: false)
                        .Annotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn),
                    Amount = table.Column<int>(nullable: false),
                    Corp = table.Column<string>(nullable: true),
                    Day = table.Column<DateTime>(nullable: false),
                    EstimatedValue = table.Column<int>(nullable: false),
                    OreID = table.Column<int>(nullable: false),
                    OreType = table.Column<string>(nullable: true),
                    Pilot = table.Column<string>(nullable: true),
                    SystemID = table.Column<int>(nullable: false),
                    Volumen = table.Column<float>(nullable: false)
                },
                constraints: table =>
                {
                    table.PrimaryKey("PK_MiningJob", x => x.ID);
                });
        }

        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropTable(
                name: "MiningJob");
        }
    }
}
