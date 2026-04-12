# miniCRM MCP Server

Local Claude Desktop MCP server for individual miniCRM users.  
12 CRM tools, stdio transport, bilingual support (Hungarian / English).

## Installation

```bash
cd minicrm-mcp
npm install
cp .env.example .env
# edit .env with your values
```

## Configuration (.env)

| Variable | Description | Default |
|---|---|---|
| `GATEWAY_URL` | API Gateway URL | `http://localhost:5080` |
| `GATEWAY_API_KEY` | Gateway authentication key | – |
| `LANGUAGE_ID` | Tool language: `hu` (Hungarian) or `en` (English) | `hu` |
| `LOG_LEVEL` | Log level: error / warn / info / debug | `info` |
| `LOG_FILE` | Log file path (leave empty to disable) | – |

## Start

```bash
npm start
```

## Claude Desktop Configuration

Copy the content of `claude_desktop_config.json` into the Claude Desktop MCP config file  
(`%APPDATA%\Claude\claude_desktop_config.json` on Windows):

```json
{
  "mcpServers": {
    "minicrm": {
      "command": "node",
      "args": ["C:/APP/minicrm-solution/minicrm-mcp/src/index.js"],
      "env": {
        "GATEWAY_URL":    "http://localhost:5080",
        "GATEWAY_API_KEY": "the-key-set-in-appsettings",
        "LANGUAGE_ID":    "en",
        "LOG_LEVEL":      "info"
      }
    }
  }
}
```

## 12 MCP Tools

| Tool | Operation | miniCRM API endpoint |
|---|---|---|
| `kontakt_kereses` | Search | `GET /Api/R3/Contact` |
| `kontakt_lekeres` | Read | `GET /Api/R3/Contact/{Id}` |
| `kontakt_letrehozas` | Create | `PUT /Api/R3/Contact` |
| `kontakt_modositas` | Update | `PUT /Api/R3/Contact/{Id}` |
| `projekt_kereses` | Search | `GET /Api/R3/Project` |
| `projekt_lekeres` | Read | `GET /Api/R3/Project/{Id}` |
| `projekt_letrehozas` | Create | `PUT /Api/R3/Project` |
| `projekt_statusz_valtas` | Status change | `PUT /Api/R3/Project/{Id}` |
| `teendo_letrehozas` | Create | `POST /Api/R3/ToDo/` |
| `teendo_lekeres` | Read | `GET /Api/R3/ToDoList/{CardId}` |
| `szamla_lekerdezes` | Read | `GET /Api/Invoice` |
| `schema_lekerdezes` | Read | `GET /Api/R3/Category` |

> **Note:** DELETE operations are intentionally not implemented for data safety.
