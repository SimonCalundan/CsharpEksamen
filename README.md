# Receptserver-system

Eksamensprojekt i C# / .NET. Datamatiker, forår 2026.

Et receptserver-system med Web API, web-grænseflade til apoteker (Blazor Server) og desktop-app til lægehuse (Avalonia). Bruger Entity Framework Core med SQLite til persistence.

## Arkitektur

```
src/
├── Receptserver.Core/         Business layer  : entities, services, DTOs, exceptions, IReceptDbContext
├── Receptserver.Data/         Data access     : DbContext, EF Core migrations, SQLite
├── Receptserver.Api/          Web service API : ASP.NET Core controllers + middleware
├── Receptserver.Apotek.Web/   Presentation 1  : Blazor Server (apotekssystem)
└── Receptserver.Laegehus.App/ Presentation 2  : Avalonia desktop (lægehus-system)
```

Klassediagram: [`docs/klassediagram.pdf`](docs/klassediagram.pdf) (kilde: `docs/klassediagram.mmd`).

## Krav

- .NET 10 SDK (`dotnet --version` skal vise 10.0.x)

## Build

Fra rodmappen:

```bash
dotnet build
```

Alle 5 projekter skal bygge med 0 warnings og 0 errors. SQLite-databasen genereres automatisk første gang API'et startes (`db.Database.Migrate()` ved opstart).

## Sådan køres systemet

Følgende skal startes i denne rækkefølge (de to klienter kalder API'et over HTTP).

> **macOS-note:** Hvis du har downloadet projektet som ZIP via browser, så ryd Gatekeeper-quarantine inden første build (ellers fejler Avalonia-appen med "beskadiget"):
> ```bash
> xattr -cr <udpakket-projektmappe>
> ```

### 1. Web API (kerne — skal køre først)

```bash
cd src/Receptserver.Api
dotnet run --launch-profile http
```

- Lytter på `http://localhost:5078`
- Swagger UI: `http://localhost:5078/swagger`
- Database-fil oprettes automatisk: `src/Receptserver.Api/receptserver.db`
- Opretter 3 lægehuse + 3 apoteker ved første migration

### 2. Apotek Web (Blazor Server)

I et nyt terminal-vindue:

```bash
cd src/Receptserver.Apotek.Web
dotnet run --launch-profile http
```

- Åbn `http://localhost:5143` i browser
- Indtast et CPR-nummer (fx `0101801234` hvis du har oprettet recept via lægehus-app)
- Tryk Søg → klik Udlever per ordination

### 3. Lægehus App (Avalonia desktop)

I et nyt terminal-vindue:

```bash
cd src/Receptserver.Laegehus.App
dotnet run
```

Et desktop-vindue åbnes. Vælg lægehus, indtast CPR, evt. tilknyt apotek, tilføj ordinationer, klik "Opret recept".

## Seed-data (hardkodet i database via EF Core HasData)

### Lægehuse

| Ydernummer | Navn | Adresse |
|---|---|---|
| 012345 | Lægerne i Aarhus C | Banegårdsgade 1, 8000 Aarhus |
| 054321 | Risskov Lægehus | Skovvejen 12, 8240 Risskov |
| 067890 | Viby Lægehus | Skanderborgvej 200, 8260 Viby |

### Apoteker

| Navn | Adresse |
|---|---|
| Aarhus Apotek | Store Torv 3, 8000 Aarhus |
| Risskov Apotek | Stadionalle 1, 8240 Risskov |
| Viby Apotek | Skanderborgvej 220, 8260 Viby |

## API-endpoints

### Lægehus-API (`/api/laegehus/`)

| Metode | Sti | Formål                        |
|---|---|-------------------------------|
| GET | `/laegehuse` | Liste med alle lægehuse       |
| GET | `/apoteker` | Liste med alle apoteker       |
| POST | `/recepter` | Opret recept med ordinationer |

### Apotek-API (`/api/apotek/`)

| Metode | Sti | Formål |
|---|---|---|
| GET | `/recepter?cpr=...` | Find aktive (ikke-lukkede) recepter for et CPR |
| POST | `/ordinationer/{id}/udlever` | Foretag udlevering på en ordination |

## Genskab database fra scratch

```bash
rm src/Receptserver.Api/receptserver.db
cd src/Receptserver.Api && dotnet run --launch-profile http
```

API'et kører migrationen ved opstart og genskaber DB med seed-data.
