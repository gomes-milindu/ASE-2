using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddOtpColumnsToCredentials : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "OtpExpiredAt",
                table: "UserCredentials",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "OtpGeneratedAt",
                table: "UserCredentials",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "VerificationOtp",
                table: "UserCredentials",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "OtpExpiredAt",
                table: "UserCredentials");

            migrationBuilder.DropColumn(
                name: "OtpGeneratedAt",
                table: "UserCredentials");

            migrationBuilder.DropColumn(
                name: "VerificationOtp",
                table: "UserCredentials");
        }
    }
}
