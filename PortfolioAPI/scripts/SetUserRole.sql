-- Voer lokaal uit in PortfolioDb. Registreer de gebruiker eerst via /auth/register.
-- Pas alleen deze twee waarden aan. Geen wachtwoord nodig.
DECLARE @Email nvarchar(256) = N'editor@example.test';
DECLARE @Role nvarchar(256) = N'Editor'; -- Gast, Editor of Admin

SET XACT_ABORT ON;
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
IF @Role NOT IN (N'Gast', N'Editor', N'Admin')
    THROW 50000, 'Kies Gast, Editor of Admin.', 1;

DECLARE @UserId nvarchar(450) = (SELECT Id FROM AspNetUsers WHERE NormalizedEmail = UPPER(@Email));
DECLARE @RoleId nvarchar(450) = (SELECT Id FROM AspNetRoles WHERE NormalizedName = UPPER(@Role));
IF @UserId IS NULL OR @RoleId IS NULL
    THROW 50001, 'Gebruiker of rol ontbreekt. Start de API en registreer de gebruiker eerst.', 1;

BEGIN TRANSACTION;
DELETE FROM AspNetUserRoles WHERE UserId = @UserId;
INSERT INTO AspNetUserRoles (UserId, RoleId) VALUES (@UserId, @RoleId);
-- Maakt bestaande cookies ongeldig, ook bij het intrekken van schrijfrechten.
UPDATE AspNetUsers SET SecurityStamp = CONVERT(nvarchar(36), NEWID()),
    ConcurrencyStamp = CONVERT(nvarchar(36), NEWID()) WHERE Id = @UserId;
COMMIT;
