using System;
using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace WebApplication1.Migrations
{
    /// <inheritdoc />
    public partial class AddMobileOtpColumns : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AddColumn<DateTime>(
                name: "MobileOtpExpiresAt",
                table: "UserCredentials",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<DateTime>(
                name: "MobileOtpGeneratedAt",
                table: "UserCredentials",
                type: "datetime(6)",
                nullable: true);

            migrationBuilder.AddColumn<string>(
                name: "MobileVerificationOtp",
                table: "UserCredentials",
                type: "longtext",
                nullable: true)
                .Annotation("MySql:CharSet", "utf8mb4");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.DropColumn(
                name: "MobileOtpExpiresAt",
                table: "UserCredentials");

            migrationBuilder.DropColumn(
                name: "MobileOtpGeneratedAt",
                table: "UserCredentials");

            migrationBuilder.DropColumn(
                name: "MobileVerificationOtp",
                table: "UserCredentials");
        }
    }
}
