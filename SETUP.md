# miniCRM MCP Solution – Telepítési Útmutató

## Architektúra áttekintése

```
Claude Desktop
     │ stdio
     ▼
minicrm-mcp/          ← Node.js MCP Server (12 eszköz)
     │ HTTP + X-Gateway-Key
     ▼
minicrm-gateway/      ← C# ASP.NET Core 8 API Gateway  :5080
     │ HTTP
     ├──► MiniCRM.ContactService  :5081  ──► miniCRM REST API (HTTPS)
     ├──► MiniCRM.ProjectService  :5082  ──► miniCRM REST API (HTTPS)
     ├──► MiniCRM.TodoService     :5083  ──► miniCRM REST API (HTTPS)
     └──► MiniCRM.InvoiceService  :5084  ──► miniCRM REST API (HTTPS)

           VAGY (valós fiók nélküli teszteléshez)

     └──► MiniCRM.MockApi         :5090  ← memóriában futó fake miniCRM API
```

## Előfeltételek

- **Node.js** 18+ (https://nodejs.org)
- **.NET 8 SDK** (https://dotnet.microsoft.com/download/dotnet/8.0)
- **Claude Desktop** (claude.ai/download)
- **miniCRM Professional** előfizetés REST API add-on-nal
- miniCRM **SystemId** és **API-kulcs**

---

## 1. lépés: Konfigurációs fájlok beállítása

### 1a. Minden mikroszolgáltatás `appsettings.json` fájlja

A következő 4 fájlban cseréld ki a helyőrzőket a valódi értékekre:

- `minicrm-services/MiniCRM.ContactService/appsettings.json`
- `minicrm-services/MiniCRM.ProjectService/appsettings.json`
- `minicrm-services/MiniCRM.TodoService/appsettings.json`
- `minicrm-services/MiniCRM.InvoiceService/appsettings.json`

```json
"MiniCRM": {
  "BaseUrl":  "https://r3.minicrm.hu",
  "SystemId": "IDE_ÍROD_A_SYSTEM_ID_T",
  "ApiKey":   "IDE_ÍROD_AZ_API_KULCSOT"
}
```

### 1b. Gateway API kulcs beállítása

Szerkeszd a `minicrm-gateway/appsettings.json` fájlt:

```json
"Gateway": {
  "GatewayApiKey": "valassz-egy-eros-veletlenszeru-kulcsot"
}
```

### 1c. MCP szerver `.env` fájl

```bash
cd minicrm-mcp
copy .env.example .env
```

Szerkeszd a `.env` fájlt – a `GATEWAY_API_KEY` legyen **ugyanaz**, mint a Gateway-ben:

```
GATEWAY_URL=http://localhost:5080
GATEWAY_API_KEY=valassz-egy-eros-veletlenszeru-kulcsot
LOG_LEVEL=info
```

---

## 2. lépés: Node.js függőségek telepítése

```bash
cd minicrm-mcp
npm install
```

---

## 3. lépés: .NET csomagok visszaállítása

A megoldás gyökérkönyvtárából:

```bash
dotnet restore minicrm.sln
```

---

## 4. lépés: Szolgáltatások indítása

### Automatikus indítás (ajánlott)

```bat
start-all.bat
```

Ez 7 terminálablakot nyit meg:
- Mock API (5090-es port)
- ContactService (5081-es port)
- ProjectService (5082-es port)
- TodoService (5083-es port)
- InvoiceService (5084-es port)
- API Gateway (5080-as port)

Ezután az MCP szervert kézzel kell elindítani:
```bash
cd minicrm-mcp && npm start
```

### Kézi indítás (fejlesztéshez)

Minden parancsot külön terminálban futtass:

```bash
# Terminal 0 – Mock API (opcionális, valós adatok nélküli teszteléshez)
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

### Ellenőrzés – fut-e minden szolgáltatás?

Nyisd meg ezeket az URL-eket a böngeszőben:

| Szolgáltatás | URL |
|---|---|
| API Gateway Swagger | http://localhost:5080/swagger |
| ContactService | http://localhost:5081/swagger |
| ProjectService | http://localhost:5082/swagger |
| TodoService | http://localhost:5083/swagger |
| InvoiceService | http://localhost:5084/swagger |
| Mock API | http://localhost:5090/Api/R3/Contact |

---

## 5. lépés: Claude Desktop konfiguráció

### Windows

Nyisd meg vagy hozd létre a fájlt:  
`%APPDATA%\Claude\claude_desktop_config.json`

Másold be a következő konfigurációt (a **teljes elérési utat** igazítsd):

```json
{
  "mcpServers": {
    "minicrm": {
      "command": "node",
      "args": ["C:\\APP\\minicrm-solution\\minicrm-mcp\\src\\index.js"],
      "env": {
        "GATEWAY_URL": "http://localhost:5080",
        "GATEWAY_API_KEY": "valassz-egy-eros-veletlenszeru-kulcsot",
        "LOG_LEVEL": "info"
      }
    }
  }
}
```

### 6. lépés: Rendszer-prompt hozzáadása

1. Nyisd meg a Claude Desktop alkalmazást
2. Hozz létre egy új **Projektet** (Claude Projects)
3. Másold be a `minicrm-mcp/system_prompt.md` fájl tartalmát a projekt rendszer-promptjába
4. Indítsd újra a Claude Desktop alkalmazást

---

## 7. lépés: Ellenőrzés

1. **Swagger UI**: Nyisd meg a http://localhost:5080/swagger oldalt
2. **MCP kapcsolat**: A Claude Desktopban keresd a 🔌 ikont – látszódjon a `minicrm` szerver
3. **Teszt parancs**: Írj a Claude-nak: „Milyen projekt-kategóriák érhetők el?"

---

## Az alkalmazás futtatása (gyors útmutató)

### A lehetőség – Valós miniCRM fiókkal

1. Töltsd ki az adatokat a `credentials.bat` fájlban:
   ```bat
   SET MINICRM__SystemId=A_TE_SYSTEM_ID_D
   SET MINICRM__ApiKey=A_TE_API_KULCSOD
   ```
2. Kattints duplán a `start-all.bat` fájlra – várja meg, amíg mind az 5 szolgáltatás elindul
3. Egy terminálban: `cd minicrm-mcp && npm start`
4. Nyisd meg a Claude Desktop alkalmazást → a miniCRM projektedet
5. Írj Claude-nak: *"Listázd a kapcsolataimat"*

### B lehetőség – Mock API-val (valós fiók nélkül)

1. Nyisd meg a `credentials.bat` fájlt és kommenteld ki a sort:
   ```bat
   SET MINICRM__BaseUrl=http://localhost:5090
   ```
2. Kattints duplán a `start-all.bat` fájlra – elindítja a Mock API-t és mind az 5 szolgáltatást
3. Egy terminálban: `cd minicrm-mcp && npm start`
4. Nyisd meg a Claude Desktop alkalmazást → a miniCRM projektedet
5. Írj Claude-nak: *"Listázd a kapcsolataimat"* – az 5 előre feltöltött tesztkontaktot adja vissza

### Ellenőrzés – minden fut?

- Gateway Swagger: http://localhost:5080/swagger
- MCP csatlakozva: keresd a dugó ikont a Claude Desktopban

---

## Mock API (Tesztelés valós miniCRM fiók nélkül)

A Mock API lokálisan szimulálja a miniCRM REST API-t, előre feltöltött tesztadatokkal.
Nem szükséges valós fiók vagy API kulcs.

### Előre betöltött adatok

| Típus | Rekordok |
|---|---|
| Kapcsolatok | 5 (Nagy Gábor, Kovács Eszter, Tóth Péter, ...) |
| Projektek | 4 (Webshop fejlesztés, ERP integráció, ...) |
| Teendők | 3 (Megbeszélés, Telefonálás, Email) |
| Számlák | 2 (INV-2026-001, INV-2026-002) |
| Projekt kategóriák | 3 (Értékesítés, Támogatás, Projekt) |
| Kapcsolat kategóriák | 2 (Ügyfél, Partner) |

### Bekapcsolás

1. Nyisd meg a `credentials.bat` fájlt és kommenteld ki az utolsó sort:
```bat
SET MINICRM__BaseUrl=http://localhost:5090
```

2. Indítsd el a Mock API-t (a `start-all.bat` automatikusan indítja, vagy kézzel):
```bash
cd minicrm-mock/MiniCRM.MockApi && dotnet run
```

3. Indítsd újra a szolgáltatásokat, hogy felvegyék az új `BaseUrl`-t.

4. Teszteld böngészőből: http://localhost:5090/Api/R3/Contact

### Kikapcsolás (visszaváltás valós miniCRM-re)

Kommenteld vissza a sort a `credentials.bat`-ban:
```bat
REM SET MINICRM__BaseUrl=http://localhost:5090
```

---

## Unit tesztek futtatása

A projekthez 28 unit teszt tartozik (xUnit + NSubstitute + FluentAssertions).
A tesztek mockokat használnak – **nem szükséges futtatni a szolgáltatásokat**.

```bash
dotnet test minicrm-tests/MiniCRM.Tests.csproj --logger "console;verbosity=normal"
```

VS Code Test Explorerben való futtatáshoz telepítsd a **.NET 10 SDK**-t (C# Dev Kit bővítmény igényli).

---

## Hibaelhárítás

| Probléma | Megoldás |
|---|---|
| Claude nem látja az MCP szervert | Ellenőrizd a `claude_desktop_config.json` elérési útját, indítsd újra a Claude-ot |
| 401 Unauthorized a Gateway-től | A `GATEWAY_API_KEY` nem egyezik az `appsettings.json`-ban lévővel |
| 401 a miniCRM API-tól | Ellenőrizd a SystemId és ApiKey értékeket az `appsettings.json` fájlokban |
| 429 Rate limit | Várd meg, hogy a percenkénti 60 kérés ablak nullázódjon |
| Mikroszolgáltatás nem indul | Ellenőrizd, hogy a port szabad-e: `netstat -ano \| findstr :5081` |
| Node.js nem található | Telepítsd a Node.js 18+ verzióját és add hozzá a PATH-hoz |

---

## Biztonsági megjegyzések

- Az API kulcsot **soha ne commitold** verziókezelőbe
- A `.env` fájl és az `appsettings.json` fájlok titkos adatokat tartalmaznak
- A DELETE műveletek szándékosan nincsenek implementálva
- Az MCP szerver kizárólag lokálisan fut
