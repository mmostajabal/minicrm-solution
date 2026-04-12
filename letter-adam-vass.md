# Tárgy: miniCRM MCP Solution – Az első verzió elkészült

**Dátum:** 2026. április 12.  
**Feladó:** Mohammad Mostajabal  
**Címzett:** Vass Ádám

---

Kedves Ádám,

örömmel értesítelek, hogy a **miniCRM MCP Solution** első verziója elkészült és kipróbálható.

Az alkalmazás lehetővé teszi, hogy a **Claude AI asszisztenssel** természetes nyelven kezeld a miniCRM rendszeredet — névjegyek, projektek, teendők és számlák lekérdezése és létrehozása egyszerű szöveges parancsokkal, valódi kód írása nélkül.

---

## Mit tud az alkalmazás?

- Névjegyek, projektek, teendők és számlák **lekérdezése és létrehozása** Claude-on keresztül
- **Mock API** — valós miniCRM fiók nélkül is azonnal kipróbálható, előre feltöltött tesztadatokkal
- **Docker támogatás** — egyetlen paranccsal elindítható, .NET telepítése nélkül
- **API Gateway** — biztonságos, kulcsvédett belépési pont
- **Kétnyelvű** — magyar és angol nyelv támogatása
- **28 unit teszt** — az üzleti logika teljes lefedettséggel tesztelve

---

## Kipróbálás

A mellékelt **QUICKSTART.hu.pdf** tartalmazza a teljes lépésről lépésre útmutatót.

Röviden, Docker segítségével:

```bash
docker compose --profile mock --env-file docker.env up -d --build
```

Majd nyisd meg: **http://localhost:5080/swagger**  
Kattints az **Authorize** gombra és add meg: `minicrm-gateway-2026-secure-key`

---

## Forráskód

**https://github.com/mmostajabal/minicrm-solution**

---

Kérlek, jelezz vissza, ha kérdésed van vagy bármit szeretnél módosítani.

Üdvözlettel,  
**Mohammad Mostajabal**
