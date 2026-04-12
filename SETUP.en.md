# miniCRM MCP Solution – Setup Guide

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

### Automatic (recommended)

```bat
start-all.bat
```

### Manual (for development)

Run each command in a separate terminal:

```bash
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
