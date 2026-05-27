using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using MovieApp.WebApi.Data;

#nullable disable

namespace MovieApp.WebApi.Migrations
{
    [DbContext(typeof(AppDbContext))]
    [Migration("20260526200000_FixAmbassadorProfileIdentity")]
    public partial class FixAmbassadorProfileIdentity : Migration
    {
        /// <inheritdoc />
        protected override void Up(MigrationBuilder migrationBuilder)
        {
            // UserId was incorrectly created as IDENTITY(1,1) -- it is a FK to Users and must accept explicit values.
            migrationBuilder.Sql(@"
                BEGIN TRANSACTION;

                CREATE TABLE [AmbassadorProfiles_New] (
                    [UserId]        INT            NOT NULL,
                    [PermanentCode] NVARCHAR(MAX)  NOT NULL,
                    [RewardBalance] INT            NOT NULL,
                    CONSTRAINT [PK_AmbassadorProfiles_New] PRIMARY KEY CLUSTERED ([UserId] ASC)
                );

                IF EXISTS (SELECT 1 FROM [AmbassadorProfiles])
                BEGIN
                    INSERT INTO [AmbassadorProfiles_New] ([UserId], [PermanentCode], [RewardBalance])
                    SELECT [UserId], [PermanentCode], [RewardBalance] FROM [AmbassadorProfiles];
                END

                DROP TABLE [AmbassadorProfiles];

                EXEC sp_rename N'dbo.AmbassadorProfiles_New', N'AmbassadorProfiles', N'OBJECT';
                EXEC sp_rename N'PK_AmbassadorProfiles_New', N'PK_AmbassadorProfiles', N'OBJECT';

                COMMIT;
            ");
        }

        /// <inheritdoc />
        protected override void Down(MigrationBuilder migrationBuilder)
        {
            migrationBuilder.Sql(@"
                BEGIN TRANSACTION;

                CREATE TABLE [AmbassadorProfiles_New] (
                    [UserId]        INT            NOT NULL IDENTITY(1,1),
                    [PermanentCode] NVARCHAR(MAX)  NOT NULL,
                    [RewardBalance] INT            NOT NULL,
                    CONSTRAINT [PK_AmbassadorProfiles] PRIMARY KEY CLUSTERED ([UserId] ASC)
                );

                DROP TABLE [AmbassadorProfiles];

                EXEC sp_rename N'dbo.AmbassadorProfiles_New', N'AmbassadorProfiles', N'OBJECT';

                COMMIT;
            ");
        }
    }
}
