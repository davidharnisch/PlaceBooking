# PlaceBooking

Webová aplikace pro rezervaci pracovních míst (Desk Booking System).
Projekt slouží jako ukázka .NET backend vývoje.

## Technologie

- **Platforma:** .NET 10
- **Architektura:** Clean Architecture (Onion Architecture)
- **Web:** ASP.NET Core MVC (s Bootstrap)
- **Databáze:** SQLite (pro vývoj), pøístup pøes Entity Framework Core (ORM)
- **Verzování:** Git

## Architektura projektu

Øešení je rozdìleno do 4 projektù dle zodpovìdností:

1. **PlaceBooking.Domain** (Core)
   - Nezávislé jádro systému.
   - Obsahuje: Entity (Room, Seat, Booking), Value Objects, Enums, Výjimky.
   - *Závislosti: Žádné.*

2. **PlaceBooking.Application** (Use Cases)
   - Aplikaèní logika a orchestrace.
   - Obsahuje: Services/Commandy pro práci s rezervacemi, DTOs, Interface pro infrastrukturu (Repository Pattern).
   - *Závislosti: Domain.*

3. **PlaceBooking.Infrastructure** (External resources)
   - Implementace pøístupu k datùm a externím službám.
   - Obsahuje: EF Core DbContext, Implementace Repositáøù, Migrace.
   - *Závislosti: Domain, Application.*

4. **PlaceBooking.Web** (Presentation)
   - Vstupní bod aplikace.
   - Obsahuje: MVC Controllery, Views, DI Konfiguraci (Program.cs).
   - *Závislosti: Application, Infrastructure (jen pro registraci služeb).*

## Roadmapa & Plán (MVP)

Celkový odhad pracnosti: **cca 10-12 MD**

1. **Pøíprava & Datová vrstva** (Odhad: 2 MD)
   - Vytvoøení struktury øešení
   - Definice Entit v Domain vrstvì (Room, Seat, Booking)
   - Nastavení EF Core, DbContext a migrace (SQLite)

2. **Jádro rezervací (Backend Logic)** (Odhad: 3 MD)
   - Implementace Repository patternu
   - Logika pro vytvoøení rezervace (BookingService)
   - Validace pravidel (kontrola obsazenosti, kapacity)

3. **UI - Zobrazení mapy a rezervace** (Odhad: 3 MD)
   - Pøíprava Layoutu a statických stránek
   - Vizuální reprezentace 7. patra (CSS/HTML grid nebo canvas)
   - Interaktivita: kliknutí na místo -> zobrazení detailu (AJAX/JS)
   - Dokonèení procesu rezervace (Backend controller)

4. **Historie a Pøehledy** (Odhad: 2 MD)
   - Tabulka historie mých rezervací
   - Pøehled obsazenosti (kalendáø/filtr)

## Jak spustit lokálnì

*(Bude doplnìno po implementaci DB a Web projektu)*
