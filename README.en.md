# miniCRM MCP Solution – Setup Guide

> Hungarian version: [README.md](README.md)

## Architecture overview

```
Claude Desktop
     │ stdio
     ▼
minicrm-mcp/          ← Node.js MCP Server (12 tools)
     │ HTTP + X-Gateway-Key
     ▼
minicrm-gateway/      ← C# ASP.NET Core 8 API Gateway  :5080
     │ HTTP
     ├──► MiniCRM.ContactService  :5081  ──► miniCRM REST API (HTTPS)
     ├──► MiniCRM.ProjectService  :5082  ──► miniCRM REST API (HTTPS)
     ├──► MiniCRM.TodoService     :5083  ──► miniCRM REST API (HTTPS)
     └──► MiniCRM.InvoiceService  :5084  ──► miniCRM REST API (HTTPS)

           OR (for testing without a real account)

     └──► MiniCRM.MockApi         :5090  ← in-memory fake miniCRM API
```

## Prerequisites

- **Node.js** 18+ (https://nodejs.org)
- **.NET 8 SDK** (https://dotnet.microsoft.com/download/dotnet/8.0)
- **Claude Desktop** (claude.ai/download)
- **miniCRM Professional** subscription with REST API add-on
- miniCRM **SystemId** and **API key**

---

## Step 1: Set credentials

### 1a. Each microservice `appsettings.json`

Replace the placeholder values in all four files:

- `minicrm-services/MiniCRM.ContactService/appsettings.json`
- `minicrm-services/MiniCRM.ProjectService/appsettings.json`
- `minicrm-services/MiniCRM.TodoService/appsettings.json`
- `minicrm-services/MiniCRM.InvoiceService/appsettings.json`

```json
"MiniCRM": {
  "BaseUrl":  "https://r3.minicrm.hu",
  "SystemId": "YOUR_SYSTEM_ID",
  "ApiKey":   "YOUR_API_KEY"
}
```

### 1b. Gateway API key

Edit `minicrm-gateway/appsettings.json`:

```json
"Gateway": {
  "GatewayApiKey": "choose-a-strong-random-key"
}
```

### 1c. MCP server `.env` file

```bash
cd minicrm-mcp
copy .env.example .env
```

Edit `.env` – `GATEWAY_API_KEY` must match the Gateway key exactly:

```
GATEWAY_URL=http://localhost:5080
GATEWAY_API_KEY=choose-a-strong-random-key
LANGUAGE_ID=en
LOG_LEVEL=info
```

---

## Step 2: Install Node.js dependencies

```bash
cd minicrm-mcp
npm install
```

---

## Step 3: Restore .NET packages

From the solution root:

```bash
dotnet restore minicrm.sln
```

---

## Step 4: Start all services

### Automatic (recommended) — start-all.bat

Double-click `start-all.bat` from the solution root folder.

What it does:
1. Loads credentials from `credentials.bat`
2. Opens a terminal window for each service in this order:
   - **Mock API** — port 5090 (fake miniCRM, for testing without real account)
   - **ContactService** — port 5081
   - **ProjectService** — port 5082
   - **TodoService** — port 5083
   - **InvoiceService** — port 5084
   - **API Gateway** — port 5080

Wait until all windows show `Now listening on: http://0.0.0.0:XXXX` before continuing.

Then start the MCP server in a separate terminal:
```bash
cd minicrm-mcp && npm start
```

> **Note:** The Mock API window starts automatically but only serves data
> if `MINICRM__BaseUrl=http://localhost:5090` is set in `credentials.bat`.
> By default the services call the real miniCRM API.

### Manual (for development)

Run each command in a separate terminal:

```bash
# Terminal 0 – Mock API (optional, for testing without real credentials)
cd minicrm-mock/MiniCRM.MockApi && dotnet run

# Terminal 1 – ContactService
cd minicrm-services/MiniCRM.ContactService && dotnet run

# Terminal 2 – ProjectService
cd minicrm-services/MiniCRM.ProjectService && dotnet run

# Terminal 3 – TodoService
cd minicrm-services/MiniCRM.TodoService && dotnet run

# Terminal 4 – InvoiceService
cd minicrm-services/MiniCRM.InvoiceService && dotnet run

# Terminal 5 – API Gateway
cd minicrm-gateway && dotnet run

# Terminal 6 – MCP Server
cd minicrm-mcp && npm start
```

### Verify all services are running

Open these URLs in your browser:

| Service | URL |
|---|---|
| API Gateway Swagger | http://localhost:5080/swagger |
| ContactService | http://localhost:5081/swagger |
| ProjectService | http://localhost:5082/swagger |
| TodoService | http://localhost:5083/swagger |
| InvoiceService | http://localhost:5084/swagger |
| Mock API | http://localhost:5090/Api/R3/Contact |

---

## Step 5: Claude Desktop configuration

### Windows

Open or create:  
`%APPDATA%\Claude\claude_desktop_config.json`

Paste the following (adjust the **full path** to match your installation):

```json
{
  "mcpServers": {
    "minicrm": {
      "command": "node",
      "args": ["C:\\APP\\minicrm-solution\\minicrm-mcp\\src\\index.js"],
      "env": {
        "GATEWAY_URL":    "http://localhost:5080",
        "GATEWAY_API_KEY": "choose-a-strong-random-key",
        "LANGUAGE_ID":    "en",
        "LOG_LEVEL":      "info"
      }
    }
  }
}
```

---

## Step 6: Add the system prompt

1. Open Claude Desktop
2. Create a new **Project** (Claude Projects)
3. Paste the contents of `minicrm-mcp/system_prompt.en.md` as the project system prompt
4. Restart Claude Desktop

---

## Step 7: Verify

1. **Swagger UI**: open http://localhost:5080/swagger
2. **MCP connection**: look for the 🔌 icon in Claude Desktop — the `minicrm` server should appear
3. **Test command**: type to Claude: *"What project categories are available?"*

---

## How to run the application (quick start)

### Option A – With real miniCRM account

1. Fill in your credentials in `credentials.bat`:
   ```bat
   SET MINICRM__SystemId=YOUR_SYSTEM_ID
   SET MINICRM__ApiKey=YOUR_API_KEY
   ```
2. Double-click `start-all.bat` — waits for all 5 services to start
3. In a terminal: `cd minicrm-mcp && npm start`
4. Open Claude Desktop → your miniCRM project
5. Ask Claude: *"List my contacts"*

### Option B – With Mock API (no real account needed)

1. Open `credentials.bat` and uncomment:
   ```bat
   SET MINICRM__BaseUrl=http://localhost:5090
   ```
2. Double-click `start-all.bat` — starts Mock API + all 5 services
3. In a terminal: `cd minicrm-mcp && npm start`
4. Open Claude Desktop → your miniCRM project
5. Ask Claude: *"List my contacts"* — returns the 5 seeded test contacts

### Verify everything is working

- Gateway Swagger: http://localhost:5080/swagger
- MCP connected: look for the plug icon in Claude Desktop

---

## Mock API (Testing without a real miniCRM account)

The Mock API simulates the miniCRM REST API locally with pre-loaded test data.
No real account or API key is needed.

### Pre-loaded seed data

| Type | Records |
|---|---|
| Contacts | 5 (Nagy Gábor, Kovács Eszter, Tóth Péter, ...) |
| Projects | 4 (Webshop fejlesztés, ERP integráció, ...) |
| Todos | 3 (Meeting, Call, Email) |
| Invoices | 2 (INV-2026-001, INV-2026-002) |
| Project categories | 3 (Értékesítés, Támogatás, Projekt) |
| Contact categories | 2 (Ügyfél, Partner) |

### How to enable

1. Open `credentials.bat` and uncomment the last line:
```bat
SET MINICRM__BaseUrl=http://localhost:5090
```

2. Start the Mock API (included in `start-all.bat`, or manually):
```bash
cd minicrm-mock/MiniCRM.MockApi && dotnet run
```

3. Restart the services so they pick up the new `BaseUrl`.

4. Test directly in browser: http://localhost:5090/Api/R3/Contact

### How to disable (switch back to real miniCRM)

Comment the line out again in `credentials.bat`:
```bat
REM SET MINICRM__BaseUrl=http://localhost:5090
```

---

## Running unit tests

The project includes 28 unit tests using xUnit + NSubstitute + FluentAssertions.
Tests use mocks — **no services need to be running**.

```bash
dotnet test minicrm-tests/MiniCRM.Tests.csproj --logger "console;verbosity=normal"
```

To run from VS Code Test Explorer, install **.NET 10 SDK** (required by C# Dev Kit extension).

---

## Changing the language (Hungarian / English)

The MCP server supports two languages: **Hungarian** (`hu`) and **English** (`en`).

### In the `.env` file

Edit `minicrm-mcp/.env`:

```
LANGUAGE_ID=hu   # Hungarian
LANGUAGE_ID=en   # English (default for this guide)
```

### In Claude Desktop configuration

In `%APPDATA%\Claude\claude_desktop_config.json`:

```json
"env": {
  "GATEWAY_URL":    "http://localhost:5080",
  "GATEWAY_API_KEY": "your-gateway-key",
  "LANGUAGE_ID":    "en",
  "LOG_LEVEL":      "info"
}
```

Change `"en"` to `"hu"` for Hungarian responses.

> **Note:** After changing the language, restart the MCP server and Claude Desktop.

---

## Troubleshooting

| Problem | Solution |
|---|---|
| Claude does not see the MCP server | Check the path in `claude_desktop_config.json`, restart Claude |
| 401 Unauthorized from Gateway | `GATEWAY_API_KEY` in `.env` does not match `appsettings.json` |
| 401 from miniCRM API | Check SystemId and ApiKey in each service's `appsettings.json` |
| 429 Rate limit | Wait for the 60-request/minute window to reset |
| Microservice won't start | Check the port is free: `netstat -ano \| findstr :5081` |
| Node.js not found | Install Node.js 18+ and add it to your PATH |

---

## Security notes

- **Never commit** the API key or `.env` to version control
- `.env` and `appsettings.json` files contain secrets
- DELETE operations are intentionally not implemented
- The MCP server runs exclusively on localhost
