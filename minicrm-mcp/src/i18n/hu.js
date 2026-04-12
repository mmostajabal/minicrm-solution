/**
 * Hungarian (Magyar) strings – LANGUAGE_ID=hu
 */
export default {
  // ── Contacts ──────────────────────────────────────────────────────────────
  kontakt_kereses: {
    description:
      'Kontaktok keresése névvel, e-mail-lel vagy telefonszámmal a miniCRM-ben. ' +
      'Visszaadja az egyező kontaktok listáját. Ha több találat van, mindet visszaadja, ' +
      'hogy a felhasználó kiválaszthassa a megfelelőt.',
    Name:        'Keresés névrészlet alapján (pl. "Kovács")',
    Email:       'Keresés e-mail-cím alapján',
    Phone:       'Keresés telefonszám alapján',
    StatusId:    'Szűrés státusz-azonosítóra',
    UserId:      'Szűrés felelős felhasználóra',
    Page:        'Lapszám (alapértelmezett: 1)',
    MaxElements: 'Maximum találatok száma (alapértelmezett: 100)',
  },
  kontakt_lekeres: {
    description:
      'Egy adott kontakt teljes adatlapjának lekérése azonosítója alapján. ' +
      'Visszaadja a nevet, e-mailt, telefonszámot, céget és az összes egyedi mezőt.',
    Id: 'A kontakt egyedi azonosítója',
  },
  kontakt_letrehozas: {
    description:
      'Új kontakt létrehozása a miniCRM-ben. ' +
      'FONTOS: végrehajtás előtt mindig mutasd meg a felhasználónak az adatokat és kérj jóváhagyást!',
    FirstName:   'Keresztnév',
    LastName:    'Vezetéknév',
    Email:       'E-mail-cím',
    Phone:       'Telefonszám',
    Type:        '"Person" (magánszemély) vagy "Company" (cég)',
    Description: 'Megjegyzés / leírás',
    Position:    'Pozíció / beosztás',
  },
  kontakt_modositas: {
    description:
      'Meglévő kontakt adatainak módosítása azonosítója alapján. ' +
      'Csak a megadott mezőket frissíti. ' +
      'FONTOS: végrehajtás előtt mindig mutasd meg a változtatásokat és kérj jóváhagyást!',
    Id:          'A módosítandó kontakt azonosítója',
    FirstName:   'Új keresztnév',
    LastName:    'Új vezetéknév',
    Email:       'Új e-mail-cím',
    Phone:       'Új telefonszám',
    Description: 'Új megjegyzés',
    Position:    'Új beosztás',
  },

  // ── Projects ──────────────────────────────────────────────────────────────
  projekt_kereses: {
    description:
      'Projektek / ügyletek keresése és szűrése a miniCRM-ben. ' +
      'Szűrhető kategóriára, státuszra, kontaktra, felelős felhasználóra és névre.',
    CategoryId:  'Projekt-kategória azonosítója',
    StatusId:    'Státusz azonosítója',
    ContactId:   'Kapcsolódó kontakt azonosítója',
    UserId:      'Felelős felhasználó azonosítója',
    Name:        'Névrészlet szerinti keresés',
    Page:        'Lapszám',
    MaxElements: 'Maximum találatok száma',
  },
  projekt_lekeres: {
    description:
      'Egy adott projekt teljes adatlapjának lekérése: státusz, felelős, ' +
      'kapcsolódó kontakt, egyedi mezők és előzmények.',
    Id: 'A projekt egyedi azonosítója',
  },
  projekt_letrehozas: {
    description:
      'Új projekt / ügylet létrehozása a miniCRM-ben. ' +
      'FONTOS: végrehajtás előtt mindig mutasd meg az adatokat és kérj jóváhagyást!',
    CategoryId:  'Projekt-kategória azonosítója (kötelező)',
    ContactId:   'Kapcsolódó kontakt azonosítója',
    Name:        'A projekt neve',
    UserId:      'Felelős felhasználó azonosítója',
    Description: 'Leírás / megjegyzés',
  },
  projekt_statusz_valtas: {
    description:
      'EGY projekt státuszának megváltoztatása. ' +
      'KIZÁRÓLAG a StatusId mezőt módosítja – más adatot nem érint. ' +
      'FONTOS: végrehajtás előtt mutasd meg az új státuszt és kérj jóváhagyást!',
    Id:       'A projekt azonosítója',
    StatusId: 'Az új státusz azonosítója',
  },

  // ── Todos ─────────────────────────────────────────────────────────────────
  teendo_letrehozas: {
    description:
      'Új teendő / feladat létrehozása egy projekthez. ' +
      'FONTOS: végrehajtás előtt mindig mutasd meg az adatokat és kérj jóváhagyást!',
    ProjectId: 'A projekt azonosítója, amelyhez a teendő tartozik',
    UserId:    'A felelős felhasználó azonosítója',
    Type:      'Teendő típusa',
    Deadline:  'Határidő ISO 8601 formátumban (pl. "2026-04-20T10:00:00")',
    Comment:   'A teendő leírása / megjegyzése',
  },
  teendo_lekeres: {
    description:
      'Egy projekt teendőlistájának lekérése. ' +
      'Visszaadja a típust, határidőt, státuszt és felelőst minden teendőhöz.',
    CardId: 'A projekt azonosítója (CardId = ProjectId az R3 API-ban)',
  },

  // ── Invoices + Schema ─────────────────────────────────────────────────────
  szamla_lekerdezes: {
    description:
      'Számlák lekérdezése projekthez vagy kontakthoz. ' +
      'Visszaadja az összegeket, státuszokat és esedékességi dátumokat. ' +
      'Csak olvasási művelet – számlák nem módosíthatók az MCP-n keresztül.',
    ProjectId: 'Projekt azonosítója',
    ContactId: 'Kontakt azonosítója',
    Status:    'Szűrés számlastátuszra',
    Page:        'Lapszám',
    MaxElements: 'Maximum találatok száma',
  },
  schema_lekerdezes: {
    description:
      'A miniCRM-fiók elérhető moduljainak, projekt-kategóriáinak, státuszainak ' +
      'és egyedi mezőinek lekérdezése. Hasznos a helyes azonosítók és ' +
      'státusznevek meghatározásához.',
    Type:
      'Lekérdezendő típus: "categories" = összes kategória és státusz, ' +
      'vagy egy konkrét modul neve (pl. "Project", "Contact")',
  },

  // ── Response messages ─────────────────────────────────────────────────────
  messages: {
    noContact:         'Nem található kontakt a megadott feltételekkel.',
    contactCreated:    '✅ Kontakt sikeresen létrehozva!',
    contactUpdated:    '✅ Kontakt sikeresen módosítva!',
    contactsFound:     (n) => `**${n} kontakt találat:**`,
    noProject:         'Nem található projekt a megadott feltételekkel.',
    projectCreated:    '✅ Projekt sikeresen létrehozva!',
    statusChanged:     '✅ Projekt státusza sikeresen megváltoztatva!',
    projectsFound:     (n) => `**${n} projekt találat:**`,
    noTodo:            'Nincs teendő ehhez a projekthez.',
    todoCreated:       '✅ Teendő sikeresen létrehozva!',
    todosFound:        (n) => `**${n} teendő:**`,
    noInvoice:         'Nem található számla a megadott feltételekkel.',
    invoicesFound:     (n) => `**${n} számla:**`,
    schemaHeader:      (type) => `**miniCRM séma – ${type}:**`,
    noData:            'Nincs adat.',
    unknownTool:       (name) => `Ismeretlen eszköz: ${name}`,
    toolError:         (name) => `❌ Hiba a(z) "${name}" eszköz futtatásakor:`,
    rateLimitError:    'Az API sebesség-korlátja elérve (60 kérés/perc). Kérem várjon egy kicsit.',
    authError:         'Hitelesítési hiba – ellenőrizze a GATEWAY_API_KEY beállítást.',
    notFoundError:     (msg) => `A keresett erőforrás nem található: ${msg}`,
    apiError:          (status, msg) => `API hiba (${status}): ${msg}`,
    statusDone:        '✅ Kész',
    statusOpen:        '🔄 Nyitott',
  },
};
