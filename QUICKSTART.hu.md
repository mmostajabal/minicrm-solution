# Gyors indítás – Futtatás Dockerrel

## Előfeltételek

- [Docker Desktop](https://www.docker.com/products/docker-desktop/) telepítve és elindítva
- [Node.js 18+](https://nodejs.org/en/download) telepítve (az MCP szerverhez)

---

## 1. lépés – MCP szerver függőségeinek telepítése (csak első alkalommal)

```bash
cd minicrm-mcp
npm install
cd ..
```

## 2. lépés – Összes service indítása

```bash
docker compose --profile mock --env-file docker.env up -d --build
```

Ez 6 containert indít el:

| Service | Port |
|---|---|
| Mock API (előre feltöltött tesztadatok) | 5090 |
| ContactService | 5081 |
| ProjectService | 5082 |
| TodoService | 5083 |
| InvoiceService | 5084 |
| API Gateway | 5080 |

Az első indítás néhány percet vesz igénybe — a Docker letölti a .NET alapképeket és felépíti a containereket.

## 3. lépés – Ellenőrzés: fut-e minden container?

```bash
docker compose --env-file docker.env ps
```

Mind a 6 containernek `running` státuszt kell mutatnia.

## 4. lépés – Tesztelés Swaggerben

1. Nyisd meg: http://localhost:5080/swagger
2. Kattints az **Authorize** gombra (jobb felül)
3. Add meg a Gateway API kulcsot: `minicrm-gateway-2026-secure-key`
4. Kattints **Authorize**, majd próbáld ki a `GET /api/contacts` végpontot — 5 teszt névjegyet kell visszakapnod

## 5. lépés – MCP szerver indítása

```bash
cd minicrm-mcp && npm start
```

## 6. lépés – Claude Desktop megnyitása

Nyisd meg a Claude Desktopot és lépj be a miniCRM projektedbe.  
Írj Claude-nak: *„Listázd a kapcsolataimat"* — vissza kell adnia az 5 előre feltöltött teszt-névjegyet.

---

## Leállítás

```bash
docker compose --env-file docker.env down
```

---

## Hasznos parancsok

| Parancs | Leírás |
|---|---|
| `docker compose --env-file docker.env ps` | Futó containerek listázása |
| `docker compose --env-file docker.env logs -f gateway` | Gateway logok követése |
| `docker compose --env-file docker.env down` | Containerek leállítása és törlése |
| `docker compose --env-file docker.env up -d` | Újraindítás (újraépítés nélkül) |

---

## Átváltás valós miniCRM fiókra

Szerkeszd a `docker.env` fájlt:

```env
MINICRM_SYSTEM_ID=A_TE_SYSTEM_ID_D
MINICRM_API_KEY=A_TE_API_KULCSOD
# MINICRM_BASE_URL=http://mock-api:5090   ← kommenteld ki ezt a sort
GATEWAY_API_KEY=minicrm-gateway-2026-secure-key
```

Majd indítsd el mock profil nélkül:

```bash
docker compose --env-file docker.env up -d --build
```
