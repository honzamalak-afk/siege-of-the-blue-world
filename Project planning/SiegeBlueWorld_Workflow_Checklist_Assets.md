**SIEGE OF THE BLUE WORLD**

Workflow · Checklist · Asset List

Interní dokument týmu — V1 Vertical Slice

# ČÁST 1 — CHRONOLOGICKÉ FLOW SCHÉMA

Každý box představuje jednu fázi vývoje V1. Šipky ukazují závislosti — nelze začít další fázi, dokud není předchozí uzavřena. Tam kde šipka přichází z více boxů, musí být dokončeny všechny vstupní fáze.

---

## FÁZE 0 — PŘÍPRAVA & ROZHODNUTÍ (před zahájením produkce)

1. Potvrdit všechny TBD položky (team size, deadline, cílový hardware)
2. Rozdělit role v týmu: programmer / designer / artist / QA
3. Nastavit repozitář (Git) a pojmenovat projekt: Siege of the Blue World
4. Vytvořit mood board — vizuální styl + 3 ukázkové humorné situace
5. Připravit Unity projekt: nastavit render pipeline, folder strukturu, Git LFS
6. Vybrat task tracker (GitHub Issues / Notion / Trello) a vytvořit první backlog

▼

## FÁZE 1 — CORE SYSTÉMY — ZÁKLAD (Programmer)

1. Player Controller: pohyb (chůze, sprint, krčení), kamera TPS, základní interakce
2. Základní Combat: střelba (projectile), melee (hitbox + animace), hit detection
3. Zdraví & smrt hráče: HP systém, death state, respawn / game over
4. Základní AI: patrol trasa, detection (sight/sound), attack state, flee state
5. Invasion % tracker: backend datový model (globální + lokální hodnota), žádné UI
6. Save / Load systém: free save — serializace herního stavu do JSON/binary

*výstup: hratelná postava + AI + save*

▼

## FÁZE 2 — WORLD & LEVEL DESIGN (Designer + Artist)

1. Blockout mapy V1: les → město (ulice) → modul lodi — pouze šedé boxy, žádné finální assety
2. Definovat POI (4–6 bodů zájmu) a jejich napojení na quest systém
3. Nastavit additive scenes a streaming zón pro Unity
4. Rozmístit spawn pointy nepřátel dle invasion % logiky
5. Definovat kolizní geometrii a navigační mesh (NavMesh) pro AI
6. Základní osvětlení scény (mood lighting — temné, sci-fi tón)

*výstup: hratelná mapa V1 v blockoutu*

▼

## FÁZE 3 — NEPŘÁTELÉ, QUEST & PROGRESSION (Programmer + Designer)

1. Implementovat 4 typy nepřátel: mimozemšťan, war dog, dron (+ operátor logika), lidé
2. Ragdoll systém: fyzická reakce těl na zásah (směr, výbuch, melee)
3. Destrukce prostředí: označit destruktivní objekty, implementovat destroy trigger
4. Quest systém: tracking, větvení (1 úroveň), dopad na invasion %
5. 3 questy V1: nadesignovat, naskribtovat, implementovat (les → město → modul)
6. Inventory systém: hmotnostní limit + prostorový limit (fixní sloty)
7. Loot systém: drop tabulky dle typu nepřítele a invasion %
8. Crafting systém: knowledge gate + materiálová podmínka → výstup (gear/upgrade)
9. Progression: základní skill tree (lidská větev + mimozemská větev), XP/unlock logika
10. Invasion systém: napojit hráčovy akce na invasion %, pasivní ticker

*výstup: plně hratelné questy + nepřátelé + progression*

▼

## FÁZE 4 — ART PASS & UI (Artist + Programmer)

1. Nahradit blockout assety finálními 3D modely (město, les, modul lodi)
2. Finální modely postav: hrdina, mimozemšťan, war dog, dron, NPC civilisté
3. Animace: pohyb, útok, smrt, idle — pro každý typ postavy
4. VFX: výstřely, exploze, destrukce, alien efekty, invaze vizuály
5. Zvuk: zbraně, kroky, nepřátelé, prostředí, hudba (ambient + combat)
6. UI / HUD: health bar, invasion %, alignment indikátor, inventory screen, quest log
7. UI transformace: vizuální změna HUD dle player alignmentu (human vs. alien-aligned)
8. Finální osvětlení a post-processing (bloom, color grading, fog)

*výstup: vizuálně dokončená V1*

▼

## FÁZE 5 — PLAYTEST & ITERACE (celý tým)

1. Interní playtest #1: ověřit core loop (průzkum → quest → dopad)
2. Interní playtest #2: ověřit combat feel (tempo, ragdoll, destrukce)
3. Interní playtest #3: ověřit invasion mechaniku (hráč cítí dopad svých akcí?)
4. Zapsat playtest log: co funguje, co nefunguje, prioritní fixy
5. Iterační sprint: opravit kritické bugy a balancing problémy
6. Externí playtest (1–3 hráči mimo tým): sběr zpětné vazby
7. Finální iterace na základě externí zpětné vazby

*výstup: otestovaná a iterovaná V1*

▼

## FÁZE 6 — V1 RELEASE — VERTICAL SLICE DOKONČEN

1. Code freeze: žádné nové features, pouze kritické bugy
2. Finální build: optimalizace výkonu (LOD, object pooling, profiling)
3. Sestavit release build pro PC
4. Interní prezentace V1: tým projde celou hrou a zhodnotí
5. Zpětná vazba a rozhodnutí: co jde do V2 scope
6. Archivovat build + dokumentaci (PRD, playtest logy, asset list)

---

# ČÁST 2 — CHECKLIST PRO TÝM

Každý člen týmu může odškrtávat položky v průběhu vývoje. Pořadí odpovídá chronologickému flow z Části 1.

## FÁZE 0 — PŘÍPRAVA

- ☐ Potvrdit TBD: team size, deadline, cílový hardware
- ☐ Rozdělit role (programmer / designer / artist / QA)
- ☐ Nastavit Git repozitář + pojmenovat projekt
- ☐ Vytvořit mood board a 3 ukázkové humorné situace
- ☐ Připravit Unity projekt (render pipeline, folder struktura, Git LFS)
- ☐ Vytvořit task backlog v task trackeru

## FÁZE 1 — CORE SYSTÉMY

- ☐ Player Controller (pohyb, kamera, interakce)
- ☐ Combat: střelba (projectile) + melee (hitbox)
- ☐ HP systém + smrt + respawn/game over
- ☐ AI: patrol, detection, attack, flee
- ☐ Invasion % tracker (backend, bez UI)
- ☐ Save / Load systém (free save)

## FÁZE 2 — WORLD DESIGN

- ☐ Blockout mapy: les → ulice → modul lodi
- ☐ Definovat 4–6 POI a jejich quest napojení
- ☐ Additive scenes + streaming zón
- ☐ Spawn pointy nepřátel dle invasion %
- ☐ NavMesh pro AI
- ☐ Základní osvětlení scény

## FÁZE 3 — NEPŘÁTELÉ, QUESTY, PROGRESSION

- ☐ 4 typy nepřátel (mimozemšťan, war dog, dron + operátor, lidé)
- ☐ Ragdoll systém
- ☐ Destrukce prostředí
- ☐ Quest systém (tracking, větvení, dopad)
- ☐ 3 questy V1 (design + skript + implementace)
- ☐ Inventory: hmotnostní + prostorový limit
- ☐ Loot systém (drop tabulky)
- ☐ Crafting: knowledge gate + materiálová podmínka
- ☐ Progression: skill tree (obě větve)
- ☐ Invasion systém: napojení na akce hráče + pasivní ticker

## FÁZE 4 — ART & UI

- ☐ Finální 3D modely prostředí (město, les, modul lodi)
- ☐ Finální modely postav (hrdina, mimozemšťan, war dog, dron, NPC)
- ☐ Animace pro každý typ postavy
- ☐ VFX (výstřely, exploze, alien efekty)
- ☐ Zvuk (zbraně, prostředí, hudba)
- ☐ HUD: health, invasion %, alignment, inventory, quest log
- ☐ UI transformace dle alignmentu
- ☐ Finální osvětlení + post-processing

## FÁZE 5 — PLAYTEST

- ☐ Interní playtest #1 (core loop)
- ☐ Interní playtest #2 (combat feel)
- ☐ Interní playtest #3 (invasion mechanika)
- ☐ Playtest log zapsán
- ☐ Iterační sprint — kritické fixy
- ☐ Externí playtest (1–3 hráči)
- ☐ Finální iterace

## FÁZE 6 — V1 RELEASE

- ☐ Code freeze
- ☐ Optimalizace výkonu (LOD, pooling, profiling)
- ☐ Release build pro PC
- ☐ Interní prezentace V1
- ☐ Rozhodnutí o V2 scope
- ☐ Archivace buildu a dokumentace

---

# ČÁST 3 — SEZNAM ASSETS PRO UNITY

Tento seznam pokrývá všechny assety potřebné pro V1 vertical slice. Priority: Kritické (bez toho nefunguje V1), Vysoká (potřeba před playtestem), Střední (pro final pass).

| **Kategorie** | **Asset / položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **3D — Prostředí** | Les: stromy, keře, kameny, tráva, cesta | *Blockout postačí pro F1–F3* | **Kritické** |
| **3D — Prostředí** | Město: budovy (fasády + interiér klíčových), ulice, chodník, auta | *Modulární sada pro opakování* | **Kritické** |
| **3D — Prostředí** | Modul vesmírné lodi: exteriér + vstup | *Unikátní asset, klíčový cíl V1* | **Kritické** |
| **3D — Prostředí** | Destruktivní objekty: zeď, plot, bedna, auto | *Potřeba pro destrukci prostředí* | **Vysoká** |
| **3D — Prostředí** | POI dekorace: stany, barikády, vraky, mimozemský harampádí | *Věrohodnost světa* | **Střední** |
| **3D — Prostředí** | Fotbalový stadion (exteriér + tribuna) | *Klíčový pro humorous event* | **Vysoká** |
| **3D — Postava** | Hrdina: plný 3D model s rigem | *Viditelný v TPS kameře (ruce, tělo)* | **Kritické** |
| **3D — Postava** | Mimozemšťan: plný model s rigem | *1 typ vizuálu, variace sílou/technologií* | **Kritické** |
| **3D — Postava** | Mimozemský War Dog: model s rigem | *Čtyřnohý, agresivní tvar* | **Kritické** |
| **3D — Postava** | Dron (zotročený člověk): model s mimozemskými modifikacemi | *Viditelné implantáty/tech na těle* | **Kritické** |
| **3D — Postava** | NPC civilista: 2–3 varianty modelu | *Placeholder pro F1–F3* | **Vysoká** |
| **3D — Postava** | Operátor dronu: mimozemšťan s ovládacím zařízením | *Vizuálně odlišitelný od standardního* | **Vysoká** |
| **Animace** | Hrdina: idle, chůze, sprint, krčení, střelba, melee, smrt | *Priorita: základní pohybový set* | **Kritické** |
| **Animace** | Mimozemšťan: idle, patrol, útok ranged, útok melee, smrt, flee | *Ragdoll overlay přes death anim.* | **Kritické** |
| **Animace** | War Dog: idle, sprint, útok, smrt | *Čtyřnohý locomotion* | **Kritické** |
| **Animace** | Dron: idle, útok, smrt (instantní při kill operátora) | *Kolaps/shutdown animace* | **Vysoká** |
| **Animace** | NPC: idle, strach, útěk | *Minimální sada* | **Vysoká** |
| **Animace** | Zničení modulu lodi: destrukční sekvence (finální cíl V1) | *Může být částečně VFX* | **Kritické** |
| **VFX** | Výstřel: nábojnice, záblesk, kouř | *Per typ zbraně (lidská vs. alien)* | **Kritické** |
| **VFX** | Zásah: krev/alien fluid, jiskry, střepiny | *Dle materiálu (maso vs. kov vs. alien)* | **Kritické** |
| **VFX** | Exploze: obecná + alien varianta | *Pro granáty a destrukci* | **Vysoká** |
| **VFX** | Destrukce objektu: prach, debris, zlomení | *Napojeno na destroy trigger* | **Vysoká** |
| **VFX** | Invasion vizuál: alien záře, hologramy, energie | *Ambient VFX pro modul a výsadek* | **Střední** |
| **VFX** | Crafting UI efekt: animace výroby/upgradu | *Malý, ale důležitý feedback* | **Střední** |
| **VFX** | Ragdoll aktivace: žádný speciální VFX, jen fyzika | *Řešeno Unity ragdoll systémem* | **Kritické** |
| **Zvuk — SFX** | Zbraně: výstřel, přebíjení, prázdný zásobník (lidská zbraň) | *WAV/OGG, prostorový 3D zvuk* | **Kritické** |
| **Zvuk — SFX** | Zbraně: alien energetická zbraň (odlišná od lidské) | *Sci-fi tón* | **Kritické** |
| **Zvuk — SFX** | Kroky: dle povrchu (tráva, beton, kov, loď) | *Footstep systém* | **Vysoká** |
| **Zvuk — SFX** | Nepřátelé: zvuky útoku, bolesti, detekce, smrt | *Per typ nepřítele* | **Vysoká** |
| **Zvuk — SFX** | Prostředí: vítr v lese, ruch města, hum modulu lodi | *Ambient loop* | **Střední** |
| **Zvuk — SFX** | UI zvuky: klik, quest splněn, inventory, save | *Drobné ale důležité* | **Střední** |
| **Zvuk — Hudba** | Combat hudba: napjatý rytmický loop | *Adaptive audio — spouštěno bojem* | **Vysoká** |
| **Zvuk — Hudba** | Ambient hudba: temný, atmosférický ambient | *Smyčka pro průzkum* | **Střední** |
| **Zvuk — Hudba** | Victory/fail moment: krátký stinger | *Konec V1 sekvence* | **Střední** |
| **UI / UX** | HUD: health bar, invasion %, alignment indikátor | *Dynamicky mění styl dle alignmentu* | **Kritické** |
| **UI / UX** | Inventory screen: mřížka slotů + hmotnostní bar | *Přístupné z pause menu* | **Kritické** |
| **UI / UX** | Quest log: aktivní questy + cíle | *Jednoduchý list, bez mapy v V1* | **Kritické** |
| **UI / UX** | Crafting menu: seznam receptů + podmínky | *Zobrazit co chybí / co je splněno* | **Vysoká** |
| **UI / UX** | Skill tree screen: obě větve (lidská / alien) | *Vizuálně odlišné větve* | **Vysoká** |
| **UI / UX** | Pause menu: resume, save, load, quit | *Standardní, minimalistické* | **Kritické** |
| **UI / UX** | Ikony: zbraně, loot typy, quest markery | *Jednoduchá piktogramová sada* | **Vysoká** |
| **UI / UX** | Font: sci-fi nebo minimalistický bezpatkový | *Konzistentní s tónem hry* | **Střední** |
| **Unity Script / Systém** | Scriptable Objects: nepřátelé, loot, questy, recepty | *Data-driven základ celé hry* | **Kritické** |
| **Unity Script / Systém** | NavMesh Agent konfigurace per typ nepřítele | *Různé rychlosti, poloměry* | **Kritické** |
| **Unity Script / Systém** | Ragdoll prefab setup (rigidbody + collider na každé kosti) | *Per model postavy* | **Kritické** |
| **Unity Script / Systém** | Destruction prefab: verze před a po zničení + debris | *Per destruktivní objekt* | **Vysoká** |
| **Unity Script / Systém** | Adaptive music manager: detekce combat stavu | *Jednoduchý state machine* | **Střední** |
| **Unity Script / Systém** | Invasion event trigger: spawnování událostí dle invasion % | *Např. fotbalový stadion event* | **Vysoká** |

---

Siege of the Blue World — Interní dokument týmu | V1 Vertical Slice

*Aktualizovat při každé změně scope nebo potvrzeném TBD.*
