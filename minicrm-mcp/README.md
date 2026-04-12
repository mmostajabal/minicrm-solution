# miniCRM MCP Server

Lokális Claude Desktop MCP szerver egyéni miniCRM-felhasználóknak.  
12 CRM-eszköz, stdio transport, magyar nyelvű válaszok.

## Telepítés

```bash
cd minicrm-mcp
npm install
cp .env.example .env
# szerkeszd a .env fájlt a megfelelő értékekkel
```

## Konfiguráció (.env)

| Változó | Leírás | Alapértelmezett |
|---|---|---|
| `GATEWAY_URL` | API Gateway URL | `http://localhost:5080` |
| `GATEWAY_API_KEY` | Gateway hitelesítési kulcs | – |
| `LOG_LEVEL` | Log szint (error/warn/info/debug) | `info` |
| `LOG_FILE` | Log fájl elérési útja | – |

## Indítás

```bash
npm start
```

## Claude Desktop konfiguráció

Másold a `claude_desktop_config.json` tartalmát a Claude Desktop MCP konfigurációs fájljába  
(`%APPDATA%\Claude\claude_desktop_config.json` Windows-on):

```json
{
  "mcpServers": {
    "minicrm": {
      "command": "node",
      "args": ["C:/APP/minicrm-solution/minicrm-mcp/src/index.js"],
      "env": {
        "GATEWAY_URL": "http://localhost:5080",
        "GATEWAY_API_KEY": "az-appsettings-ben-beallitott-kulcs",
        "LOG_LEVEL": "info"
      }
    }
  }
}
```

## 12 MCP Eszköz

| Eszköz | Művelet | miniCRM API végpont |
|---|---|---|
| `kontakt_kereses` | Keresés | `GET /Api/R3/Contact` |
| `kontakt_lekeres` | Olvasás | `GET /Api/R3/Contact/{Id}` |
| `kontakt_letrehozas` | Létrehozás | `PUT /Api/R3/Contact` |
| `kontakt_modositas` | Módosítás | `PUT /Api/R3/Contact/{Id}` |
| `projekt_kereses` | Keresés | `GET /Api/R3/Project` |
| `projekt_lekeres` | Olvasás | `GET /Api/R3/Project/{Id}` |
| `projekt_letrehozas` | Létrehozás | `PUT /Api/R3/Project` |
| `projekt_statusz_valtas` | Státuszváltás | `PUT /Api/R3/Project/{Id}` |
| `teendo_letrehozas` | Létrehozás | `POST /Api/R3/ToDo/` |
| `teendo_lekeres` | Olvasás | `GET /Api/R3/ToDoList/{CardId}` |
| `szamla_lekerdezes` | Olvasás | `GET /Api/Invoice` |
| `schema_lekerdezes` | Olvasás | `GET /Api/R3/Category` |

> **Megjegyzés:** DELETE műveletek szándékosan nincsenek implementálva az adatbiztonság érdekében.
