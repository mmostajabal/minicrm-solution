# Quick Start – Run with Docker

## Prerequisites

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) installed and running
- [Node.js 18+](https://nodejs.org/en/download) installed (for the MCP server)

---

## Step 1 – Install MCP server dependencies (first time only)

```bash
cd minicrm-mcp
npm install
cd ..
```

## Step 2 – Build and start all services

```bash
docker compose --profile mock --env-file docker.env up -d --build
```

This starts 6 containers:

| Service | Port |
|---|---|
| Mock API (fake miniCRM data) | 5090 |
| ContactService | 5081 |
| ProjectService | 5082 |
| TodoService | 5083 |
| InvoiceService | 5084 |
| API Gateway | 5080 |

First run takes a few minutes — Docker downloads the .NET base images and builds the containers.

## Step 3 – Verify all containers are running

```bash
docker compose --env-file docker.env ps
```

All 6 containers should show status `running`.

## Step 4 – Test in Swagger

1. Open http://localhost:5080/swagger
2. Click the **Authorize** button (top right)
3. Enter the Gateway API key: `minicrm-gateway-2026-secure-key`
4. Click **Authorize**, then try `GET /api/contacts` — you should see 5 test contacts

## Step 5 – Start the MCP server

```bash
cd minicrm-mcp && npm start
```

## Step 6 – Open Claude Desktop

Open Claude Desktop and go to your miniCRM project.  
Ask Claude: *"List my contacts"* — it should return the 5 seeded test contacts.

---

## Stop everything

```bash
docker compose --env-file docker.env down
```

---

## Useful commands

| Command | Description |
|---|---|
| `docker compose --env-file docker.env ps` | Show running containers |
| `docker compose --env-file docker.env logs -f gateway` | Follow gateway logs |
| `docker compose --env-file docker.env down` | Stop and remove containers |
| `docker compose --env-file docker.env up -d` | Start again (no rebuild) |

---

## Switch to real miniCRM account

Edit `docker.env`:

```env
MINICRM_SYSTEM_ID=YOUR_SYSTEM_ID
MINICRM_API_KEY=YOUR_API_KEY
# MINICRM_BASE_URL=http://mock-api:5090   ← comment out this line
GATEWAY_API_KEY=minicrm-gateway-2026-secure-key
```

Then start without the mock profile:

```bash
docker compose --env-file docker.env up -d --build
```
