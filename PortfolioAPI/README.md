# PortfolioAPI

ASP.NET Core Web API op .NET 10, met een lagenstructuur voor controllers.
Controllers, Entities, DTOs, Data en Services zijn uitgewerkt.
Swagger toont de tien CRUD-endpoints voor projecten en blogposts.

## Structuur

- `Controllers`: ProjectsController en BlogsController verzorgen HTTP en
  ontvangen hun service via constructorinjectie.

- `Entities`: Project en Blog (de blogpost), met de velden uit de opdracht.
- `DTOs`: Create- en Update-DTOs met validatie, en Response-DTOs met Id.
- `Data`: AppDbContext met Projects en Blogs en de databaseconfiguratie.
- `Services`: IProjectService/ProjectService en IBlogService/BlogService.
  Beide bieden asynchrone CRUD. De centrale ToResponse-mapping zet entiteiten
  om naar DTOs; leesqueries gebruiken AsNoTracking.

De controllers gebruiken de service-interfaces. GetByIdAsync en
UpdateAsync geven null bij een onbekend id; DeleteAsync geeft dan false.
De controller vertaalt die resultaten naar 404. [ApiController] zorgt voor
automatische DTO-validatie en 400-responses. De services valideren invoer ook
bij rechtstreeks gebruik en gooien dan bij ongeldige invoer ValidationException.
Id wordt door SQL Server gegenereerd. Datum en Publicatiedatum zijn verplicht
in de invoer; gebruik een ISO 8601-datum, bijvoorbeeld 2026-11-03T00:00:00.

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
