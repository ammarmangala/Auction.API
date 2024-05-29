# Online Veilingplatform Web API

## Inhoudsopgave
1. [Doelstellingen](#doelstellingen)
2. [Context](#context)
3. [Opdracht](#opdracht)
4. [Technische Vereisten](#technische-vereisten)
5. [Web API Functionaliteiten](#web-api-functionaliteiten)
    - [Gebruikers beheren](#gebruikers-beheren)
    - [Items verkopen](#items-verkopen)
    - [Bieden op items](#bieden-op-items)
6. [Nice-to-have Functionaliteiten](#nice-to-have-functionaliteiten)
7. [Richtlijnen](#richtlijnen)
8. [Security](#security)

## Doelstellingen

- De student kan zelfstandig code schrijven om een software-project te realiseren.
- De student kan wensen en feedback van stakeholders vertalen naar acties en deze uitwerken.
- De student denkt kritisch na over het eigen geleverde werk.
- De student denkt na over beveiliging (authenticatie en autorisatie) van een softwaretoepassing en kan deze toepassen in de praktijk.

## Context

Voor deze opdracht werk je een Web API uit voor een online veilingplatform. Via het platform kan een gebruiker een artikel te koop aanbieden en kunnen andere gebruikers een bod uitbrengen op dit artikel. Gebruikers kunnen biedingen op hun items opvolgen en een overzicht raadplegen van de artikelen die zij hebben gekocht.

Er wordt enkel een Web API ontwikkeld, omdat voor het veilingplatform ook een mobiele applicatie dient te worden ontwikkeld, waarvoor de opdracht in een andere cursus zal worden uitgelegd.

## Opdracht

Ontwikkel een Web API voor een online veilingplatform volgens het onderstaande ERD-schema en de beschreven functionaliteiten. Bij de ontwikkeling dien je rekening te houden met de architectuur van je oplossing en je code testbaar te maken volgens de technieken die je hebt geleerd.

## Technische Vereisten

- De opdracht is uitgewerkt in .NET 6.
- Gebruik Entity Framework Core voor persistentie.
- Gebruik MS SQL Server als database.
- Bouw de applicatie uit aan de hand van een gelaagde architectuur.
- Gebruik Dependency Injection waar nodig.
- Gebruik Identity Framework voor authenticatie en autorisatie.
- De Web API is beveiligd met JWT-tokens.

## Web API Functionaliteiten

### Gebruikers beheren

1. **Nieuwe gebruikers registreren**: Registratie met email, wachtwoord en subscription-type (Free, Gold, Platinum). Bewaar het subscription-type als claim.
2. **Inloggen**: Inloggen met gebruikersnaam en wachtwoord. Bij correcte gegevens ontvangt de gebruiker een JWT-token.

### Items verkopen

1. **Item te koop aanbieden**: 
    - Verplichte velden: Naam, Beschrijving, Startprijs, Startdatum + tijdstip.
    - Eindtijdstip:
        - Free: automatisch 3 dagen na starttijdstip.
        - Gold/Platinum: vrij te kiezen, minimaal 12 uur na starttijdstip.
    - Endpoint: `/api/auction/items` (POST, authenticatie vereist).

2. **Verkoop annuleren**: 
    - Annuleren mogelijk zolang de eindtijd niet verstreken is.
    - Endpoint: `/api/auction/items/{itemId}/cancel` (DELETE, authenticatie vereist).

3. **Verkochte items raadplegen**:
    - Voorwaarden: Status is Initial of Paid, eindtijd verstreken, minstens één bod uitgebracht.
    - Endpoint: `/api/auction/sellers/{userId}/items` (GET, authenticatie vereist).

4. **Biedingen opvolgen**:
    - Overzicht en details van biedingen op eigen items.
    - Endpoint: `/api/auction/items/{itemId}/biddings` (GET, authenticatie vereist).

### Bieden op items

1. **Items zoeken**:
    - Zoekparameters: Categorieën, Max. prijs.
    - Resultaten gesorteerd op eindtijd.
    - Endpoint: `/api/auction/items/search` (GET, geen authenticatie vereist).

2. **Bod plaatsen**:
    - Validaties: Minimaal 5% hoger dan huidige hoogste bod, afgerond tot €0,50, bieding niet afgelopen, item niet geannuleerd, geen bod op eigen items.
    - Endpoint: `/api/auction/items/{itemId}/bids` (POST, authenticatie vereist).

3. **Gekochte items raadplegen**:
    - Voorwaarden: Bieding afgelopen, hoogste bod, status niet geannuleerd.
    - Endpoint: `/api/auction/buyers/{userId}/items` (GET, authenticatie vereist).

4. **Betaling simuleren**:
    - Alleen voor eigen gekochte items.
    - Endpoint: `/api/auction/buyers/{userId}/items/{itemId}/payment` (POST, authenticatie vereist).

## Nice-to-have Functionaliteiten

1. **Favorieten beheren**:
    - **Toevoegen aan favorieten**: Item toevoegen aan favorieten.
    - **Verwijderen uit favorieten**: Item verwijderen uit favorieten.
    - **Favorieten raadplegen**: Lijst met favoriete items opvragen.

## Richtlijnen

- Gebruik Exceptions voor foutafhandeling binnen de applicatie (bijv. bod lager dan huidige prijs).

## Security

Beveilig de Web API met JWT-tokens. Zorg voor correcte authenticatie en autorisatie voor de relevante endpoints.

---

Met deze opdracht heb je alle tools en richtlijnen om een robuuste en veilige Web API te ontwikkelen voor een online veilingplatform. Veel succes!
