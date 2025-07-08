using Microsoft.EntityFrameworkCore.Migrations;

#nullable disable

namespace ASP.NET_Razor_Final.Migrations
{
    /// <inheritdoc />
    public partial class SeedUser : Migration
    {
        /// <inheritdoc />
       protected override void Up(MigrationBuilder migrationBuilder)
        {
            for (int i = 0; i < 150; i++)
            {
                migrationBuilder.InsertData(
                    "Users",
                    columns: new[]
                    {
                        "Id",
                        "UserName",
                        "Email",
                        "SecurityStamp",
                        "EmailConfirmed",
                        "PhoneNumberConfirmed",
                        "TwoFactorEnabled",
                        "LockoutEnd",
                        "AccessFailedCount",
                        "HomeAdress",
                        "LockoutEnabled" // 👈 nên thêm luôn cột này nếu nó không cho null
                    },
                    values: new object[]
                    {
                        Guid.NewGuid().ToString(),
                        "User-" + i.ToString("D3"),
                        $"email{i:D3}@example.com",
                        Guid.NewGuid().ToString(),
                        true,
                        false,
                        false,
                        null, // ✅ đúng kiểu với LockoutEnd
                        0,
                        "...@#...",
                        false // 👈 LockoutEnabled phải có nếu NOT NULL
                    }
                );
            }
        }
    //     [UserName]
    //   ,[NormalizedUserName]
    //   ,[Email]
    //   ,[NormalizedEmail]
    //   ,[EmailConfirmed]
    //   ,[PasswordHash]
    //   ,[SecurityStamp]
    //   ,[ConcurrencyStamp]
    //   ,[PhoneNumber]
    //   ,[PhoneNumberConfirmed]
    //   ,[TwoFactorEnabled]
    //   ,[LockoutEnd]
    //   ,[LockoutEnabled]
    //   ,[AccessFailedCount]
    //   ,[HomeAdress]
    //   ,[BirthDate]

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.AlterColumn<string>(
                name: "Name",
                table: "UserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "UserTokens",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "ProviderKey",
                table: "UserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");

            migrationBuilder.AlterColumn<string>(
                name: "LoginProvider",
                table: "UserLogins",
                type: "nvarchar(128)",
                maxLength: 128,
                nullable: false,
                oldClrType: typeof(string),
                oldType: "nvarchar(450)");
        }
    }
}
