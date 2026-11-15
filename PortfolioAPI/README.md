# PortfolioAPI

ASP.NET Core Web API op .NET 10, met een lagenstructuur voor controllers.
Controllers, Entities, DTOs, Data en Services zijn uitgewerkt.
Swagger toont de tien CRUD-endpoints en de registratie-, login- en logoutroutes.

## Structuur

- `Controllers`: ProjectsController en BlogsController verzorgen HTTP en
  ontvangen hun service via constructorinjectie.

- `Entities`: Project met de inhoudsvelden uit script.js; Blog voor blogposts.
- `DTOs`: Create- en Update-DTOs met validatie, en Response-DTOs met Id.
- `Data`: AppDbContext met Projects en Blogs en de databaseconfiguratie.
- `Services`: IProjectService/ProjectService en IBlogService/BlogService.
  Beide bieden asynchrone CRUD. De centrale DTO-mapping zet entiteiten
  om naar DTOs; leesqueries gebruiken AsNoTracking.

De controllers gebruiken de service-interfaces. GetByIdAsync en
UpdateAsync geven null bij een onbekend id; DeleteAsync geeft dan false.
De controller vertaalt die resultaten naar 404. [ApiController] zorgt voor
automatische DTO-validatie en 400-responses. De services valideren invoer ook
bij rechtstreeks gebruik en gooien dan bij ongeldige invoer ValidationException.
Id blijft een door SQL Server gegenereerd getal, niet de tekst-id uit script.js.
Projecten gebruiken title, description, category, technologies, gitHubUrl en datum.
Technologies mag niet null zijn. Een lege lijst, één technologie of meerdere
technologieën zijn toegestaan; de inhoud van de elementen wordt niet gevalideerd.
De lijst wordt via EF Core als JSON in SQL Server opgeslagen.
GitHubUrl en Datum zijn teruggebracht volgens de opdracht en zijn verplicht
bij POST en PUT. Bestaande projecten zonder deze gegevens geven null terug;
vul hun echte repository-URL en projectdatum aan via PUT. Daarom zijn deze
twee databasekolommen nullable. Title, Description en Category zijn de
Engelse namen voor Titel, Beschrijving en Categorie uit de opdracht.
Blogposts blijven titel, inhoud en publicatiedatum gebruiken.
Publicatiedatum is verplicht; gebruik bijvoorbeeld 2026-11-03T00:00:00.

Voor POST /api/projects kun je elk van deze objecten afzonderlijk gebruiken.
Vervang de voorbeeld-URL en voorbeeld-datum door de echte projectgegevens:

```json
{
  "title": "Hotel Simulatie Systeem",
  "description": "Een Java applicatie voor het simuleren van hotel operaties met gasten, liften en verschillende faciliteiten.",
  "category": "java",
  "technologies": ["Java", "Object-georiënteerd programmeren"],
  "gitHubUrl": "https://github.com/USERNAME/HOTEL-REPOSITORY",
  "datum": "2026-11-03T00:00:00"
}
```

```json
{
  "title": "Portfolio Website",
  "description": "Een responsieve portfolio website gebouwd met HTML5 en CSS3.",
  "category": "web",
  "technologies": ["HTML5", "CSS3", "Responsive Design"],
  "gitHubUrl": "https://github.com/USERNAME/PORTFOLIO-REPOSITORY",
  "datum": "2026-11-03T00:00:00"
}
```

PUT gebruikt dezelfde velden. GET en POST geven daarnaast het numerieke id terug.
De website leest nog de vaste JavaScript-array; deze wijziging voegt geen fetch-koppeling toe.

## Endpoints

| Methode | Projecten | Blogposts | Succes |
| --- | --- | --- | --- |
| GET | /api/projects | /api/blogs | 200 met een lijst DTOs |
| GET | /api/projects/{id} | /api/blogs/{id} | 200 met een DTO |
| POST | /api/projects | /api/blogs | 201 met een DTO en Location-header |
| PUT | /api/projects/{id} | /api/blogs/{id} | 200 met de gewijzigde DTO |
| DELETE | /api/projects/{id} | /api/blogs/{id} | 204 zonder inhoud |

GET op id, PUT en DELETE geven 404 als het item niet bestaat.
POST en PUT geven 400 bij ongeldige invoer. De controllers gebruiken geen
DbContext of entiteiten rechtstreeks.

## Authenticatie en autorisatie

Identity bewaart gebruikers en gehashte wachtwoorden via EF Core in dezelfde
SQL Server-database. `AppDbContext` erft van `IdentityDbContext<IdentityUser>`.
De migratie `AddIdentity` maakt de Identity-tabellen; deze wordt net als de
andere migraties bij het starten toegepast. De API maakt de rollen Gast,
Editor en Admin aan als ze nog ontbreken, zonder standaardaccounts of wachtwoorden.

Gebruik Swagger via HTTPS:

1. Voer `POST /auth/register` uit met `email` en `password` in de JSON-body.
   Kies zelf een wachtwoord (minimaal 6 tekens, hoofdletter, kleine letter,
   cijfer en een niet-alfanumeriek teken). Bewaar het niet in de repository.
   Registratie gebruikt `UserManager`, geeft 201 en kent uitsluitend Gast toe.
   Ongeldige invoer of een bestaande gebruikersnaam geeft 400.
2. Voer `POST /auth/login` uit met dezelfde velden. Succes geeft 200 en een
   HttpOnly-authenticatiecookie; verkeerde inloggegevens geven 401.
   Deze eigen loginroute gebruikt altijd cookies, dus `useCookies` is niet nodig.
   Swagger stuurt de cookie automatisch mee bij volgende verzoeken.
3. `POST /auth/logout` geeft 204 en verwijdert de aanmeldcookie.

Alle GET-routes voor projecten en blogposts zijn publiek. POST, PUT en DELETE
gebruiken de policy `CanEdit`, die expliciet Editor **of** Admin vereist.
Zonder geldige cookie volgt 401; een aangemelde Gast krijgt 403.
De controle gebeurt in de API voordat de controlleractie wordt uitgevoerd.

### Lokaal Editor en Admin toekennen

Registreer eerst afzonderlijke gebruikers voor Gast, Editor en Admin. Open
`scripts/SetUserRole.sql` in SQL Server Management Studio, kies de lokale
`PortfolioDb`, vul het e-mailadres en de gewenste rol in en voer het script uit.
Herhaal dit voor de andere gebruiker met rol Admin. Een gewone gebruiker laat
je Gast. Het script vervangt de huidige rol; Admin heeft dus zelfstandig toegang.
Je kunt hetzelfde script met rol Gast gebruiken om schrijfrechten in te trekken.

Het script wijzigt ook de security stamp. De API controleert deze bij ieder
verzoek, waardoor een bestaande cookie na de wijziging 401 oplevert. Log opnieuw
in via `/auth/login` om de nieuwe rol te gebruiken. Gebruik deze procedure ook
bij een latere wijziging van gebruikersclaims/policies in de database.

Controleer handmatig beide soorten inhoud: publieke GET (200), schrijven zonder
cookie (401), schrijven als Gast (403), en POST/PUT/DELETE als Editor en als Admin
(201/200/204 bij geldige gegevens en bestaande id's). Log tussen gebruikers uit.
Een rolwijziging moet eerst 401 geven met de oude cookie en pas na login de nieuwe
rechten. Een beheerroute en een afzonderlijk xUnit-project zijn nog niet toegevoegd;
het testgedeelte van opdracht 4 wordt apart uitgewerkt.

De cookie is Secure en SameSite=Strict: gebruik HTTPS. De latere SPA moet de
cookie meesturen; bij een andere origin zijn expliciete CORS- en cookiekeuzes nodig.
Geheimen horen in lokale configuratie of omgevingsvariabelen, nooit in Git.

Bron: [Microsoft Learn over Identity](https://learn.microsoft.com/en-us/aspnet/core/security/authentication/identity?view=aspnetcore-10.0).

## Database en starten

Installeer de .NET 10 SDK en start SQL Server Express. De standaardverbinding
in PortfolioAPI/appsettings.json gebruikt Windows-authenticatie op
`.\SQLEXPRESS` met database `PortfolioDb`. Pas de servernaam aan wanneer je
een andere SQL Server-instance gebruikt. De Windows-gebruiker moet de database
kunnen aanmaken en lezen/schrijven. TrustServerCertificate is bedoeld voor
lokale ontwikkeling.

Vanuit deze map:

```powershell
dotnet restore PortfolioAPI/PortfolioAPI.csproj
dotnet run --project PortfolioAPI/PortfolioAPI.csproj --launch-profile https
```

Open `/swagger` op de HTTPS-URL uit de console. De bestaande launch-profielen
gebruiken de Development-omgeving, waarin Swagger beschikbaar is.

De applicatie gebruikt EF Core Migrations. Bij het starten past MigrateAsync
alle nog niet uitgevoerde migraties toe. De migratiebestanden en de modelsnapshot
staan in PortfolioAPI/Migrations. De tabel __EFMigrationsHistory houdt bij
welke migraties zijn toegepast. Bestaande gegevens blijven bij een normale
herstart bewaard.

Voor de EF-commandoregeltool installeer je eenmalig de bijpassende versie:

```powershell
dotnet tool install --global dotnet-ef --version 10.0.14
```

Een database bijwerken kan ook expliciet, vanuit deze map:

```powershell
dotnet ef database update --project PortfolioAPI/PortfolioAPI.csproj
```

Na een modelwijziging maak en controleer je een nieuwe migration:

```powershell
dotnet ef migrations add NaamVanWijziging --project PortfolioAPI/PortfolioAPI.csproj
dotnet ef database update --project PortfolioAPI/PortfolioAPI.csproj
```

Controleer gegenereerde migrations op onbedoeld gegevensverlies voordat je ze
toepast. De lokale PortfolioDb bestond al voor de overstap; het bestaande
schema is vergeleken met een nieuw via migrations opgebouwd schema voordat
de overeenkomstige migraties in de geschiedenis zijn geregistreerd. Voer
InitialCreate niet rechtstreeks uit op een bestaande database zonder
migratiegeschiedenis. Gebruik daarvoor eerst een gecontroleerde baseline.

Je kunt de verbinding overschrijven zonder een wachtwoord in Git op te slaan:

```powershell
$env:ConnectionStrings__DefaultConnection = 'Server=YOUR_SERVER;Database=PortfolioDb;Trusted_Connection=True;TrustServerCertificate=True'
```

Referentie: [Microsoft Learn over DbContext en dependency injection](https://learn.microsoft.com/en-us/ef/core/dbcontext-configuration/).
