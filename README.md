# PlaceBooking

Webová aplikace pro rezervaci pracovních míst.
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
	- Obsahuje: Entity (User, Room, Seat, Booking), Value Objects, Enums, Výjimky.
	- *Závislosti: Žádné.*

2. **PlaceBooking.Application** (Use Cases)
	- Aplikaèní logika a orchestrace.
	- Obsahuje: Services/Commandy pro práci s rezervacemi, Data Transfer Objects, Interface pro infrastrukturu (Repository Pattern).
	- *Závislosti: Domain.*

3. **PlaceBooking.Infrastructure** (External resources)
	- Implementace pøístupu k datùm a externím službám.
	- Obsahuje: EF Core DbContext, Implementace Repositáøù, Migrace.
	- *Závislosti: Domain, Application.*

4. **PlaceBooking.Web** (Presentation)
	- Vstupní bod aplikace.
	- Obsahuje: MVC Controllery, Views, DI Konfiguraci (Program.cs).
	- *Závislosti: Application, Infrastructure (jen pro registraci služeb).*

## Roadmapa

Celkový odhad pracnosti: **cca 10 MD**

1. **Pøíprava & Datová vrstva** (Odhad: 2 MD)
	- Vytvoøení struktury øešení
	- Definice Entit v Domain vrstvì (User, Room, Seat, Booking)
	- Konfigurace EF Core, DbContext a migrace (SQLite)

2. **Logika rezervací (Backend Logic)** (Odhad: 4 MD)
	- Implementace Repository patternu
	- Logika pro vytvoøení rezervace (BookingService)
	- Øešení soubìhu (Concurrency Control)
	- Validace business pravidel (kontrola obsazenosti, oprávnìní, kapacity)

3. **UI - Zobrazení mapy a rezervace** (Odhad: 3 MD)
 	- Implementace pøihlašování (Autentizace uživatele)
	- Pøíprava Layoutu a statických stránek
	- Vizuální reprezentace 7. patra (CSS Grid nebo SVG interaktivní mapa)
	- Interaktivita: kliknutí na místo (JS)
	- Dokonèení procesu rezervace

4. **UI - Historie a Pøehledy** (Odhad: 1 MD)
	- Tabulka historie rezervací
	- Pøehled obsazenosti místnosti pro daný den
	- Pøehled vytíženosti míst

## Jak spustit

*(Bude doplnìno)*
