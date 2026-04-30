**PRODUCT REQUIREMENTS DOCUMENT**

Siege of the Blue World

Verze: 0.2 — Vertical Slice (V1)

Engine: Unity | Platforma: PC | Typ: Singleplayer TPS Open-World RPG

---

## CHANGELOG v0.2 (oproti v0.1)

- **Sekce 0:** Aktualizovaný TBD seznam (uzavřené + nové položky)
- **Sekce 2.5 (NOVÁ):** Designová filozofie — Rules-Based Design
- **Sekce 3 (REWRITE):** Tříúrovňová loop struktura (Combat → Encounter → Quest)
- **Sekce 5 (REWRITE):** Combat System s explicitní lethality
- **Sekce 6 (EXPAND):** Drone scout systém (Tier 1 V1, architektura pro Tier 2/3)
- **Sekce 11 (EXPAND):** UI/UX s GPS / Tactical Map systémem
- **Terminologie:** "Drone (zotročený člověk)" → **Puppet** | nový pojem **Puppeteer** (alien operator) | nový pojem **Recon Drone** (hráčův UAV)

---

# 0. STATUS — CO JE ROZHODNUTO A CO NE

Tento přehled slouží jako rychlá orientace pro celý tým. Před zahájením vývoje V1 musí být všechny TBD položky potvrzeny.

| **Oblast** | **Status** | **Poznámka** |
|---|---|---|
| **Název hry** | **✅ Rozhodnuto** | Siege of the Blue World |
| **Engine / Platforma** | **✅ Rozhodnuto** | Unity, PC |
| **Scope V1** | **✅ Rozhodnuto** | Město + les + modul lodi, vertical slice |
| **Scope long-term** | **✅ Rozhodnuto** | 20×20 km open world |
| **Identita hrdiny** | **✅ Rozhodnuto** | Pevná postava, sdílené backstory, vývoj v mnoha směrech |
| **Styl humoru** | **✅ Rozhodnuto** | Cynický + suchý + absurdní + satirický |
| **Počet regionů V1** | **✅ Rozhodnuto** | 1 region (město + les + modul lodi) |
| **Počet questů V1** | **✅ Rozhodnuto** | 3 questy vedoucí ke zničení modulu |
| **Počet nepřátel V1** | **✅ Rozhodnuto** | 4 typy: alien, war dog, Puppet (+ Puppeteer), nepřátelští lidé |
| **Boss fight V1** | **✅ Rozhodnuto** | Ne — silnější jedinci díky technologii, ne boss |
| **Save systém** | **✅ Rozhodnuto** | Free save |
| **Crafting systém** | **✅ Rozhodnuto** | Ano — učení + mimozemské materiály |
| **Inventory limit** | **✅ Rozhodnuto** | Hmotnostní (roste se silou) + prostorový (fixní) |
| **Destrukce prostředí** | **✅ Rozhodnuto** | Ano |
| **Ragdoll systém** | **✅ Rozhodnuto** | Ano — realistická fyzika těl |
| **Designová filozofie** | **✅ Rozhodnuto v0.2** | Rules-Based Design (viz sekce 2.5) |
| **Loop struktura** | **✅ Rozhodnuto v0.2** | Tříúrovňová: Combat → Encounter → Quest |
| **Combat lethality** | **✅ Rozhodnuto v0.2** | 1 rána = kill nebo critical injury |
| **Recon Drone** | **✅ Rozhodnuto v0.2** | Tier 1 funkční ve V1, architektura pro Tier 2/3 |
| **GPS / Tactical Map** | **✅ Rozhodnuto v0.2** | Mini-mapa + heatmap s decay (viz sekce 11.2) |
| **Detail combat mechaniky** | **⚠ TBD** | Damage thresholds, healing timing, alien sense range |
| **Drone scope** | **⚠ TBD** | Které Tier 2/3 features ve V1 vs. V2 |
| **Heatmap decay timing** | **⚠ TBD** | Konkrétní časové prahy (5/10/15/30 min návrh) |
| **Větvení questů (příběh)** | **⚠ TBD** | Týká se příběhu, ne pravidel světa |
| **Team size / deadline** | **⚠ TBD** | Počet lidí a cílový datum V1 |
| **Cílový hardware** | **⚠ TBD** | Min. spec PC |
| **Multiplayer future-proof** | **⚠ TBD** | Architektura připravit / nepřipravit |

---

# 1. PŘEHLED PROJEKTU

Projekt je open-world RPG z pohledu třetí osoby (TPS) zasazený do doby mimozemské invaze na Zemi. Hráč stojí před morálními volbami, které systémově mění svět kolem něj — stav invaze, chování NPC i samotnou identitu postavy.

## 1.1 Klíčové parametry

- Engine: Unity
- Platforma: PC
- Typ: Singleplayer, Third-Person Shooter + melee
- Tón: Temný, cynický humor, morální ambivalence
- Long-term scope: cca 20×20 km open world
- V1 scope: 1 region, vertical slice (~1×1 km)

**✅ Název hry: Siege of the Blue World**

---

# 2. VIZE A CORE FANTASY

Hráč je obyčejný člověk uprostřed mimozemské invaze. Jeho rozhodnutí nejsou binární — existuje spektrum voleb mezi zachováním lidskosti, přijetím mimozemské moci a čistým egoismem. Svět na tato rozhodnutí reaguje viditelně a systémově.

## 2.1 Klíčové pilíře zážitku

- **AGENCY:** Každé rozhodnutí má měřitelný dopad na svět
- **AMBIVALENCE:** Žádná volba není jednoznačně správná
- **ATMOSPHERE:** Temná, s nádechem cynického humoru
- **SYSTÉMOVOST:** Svět reaguje automaticky, ne jen skriptovaně

✅ **Identita hrdiny:** Pevná postava se sdíleným počátečním backstory. Hrdina se vyvíjí v mnoha směrech v závislosti na rozhodnutích hráče — jeho fyzická podoba, schopnosti i vztahy s okolním světem se proměňují.

✅ **Styl humoru:** Vrstevnatý — cynický, suchý, absurdní i satirický zároveň. Humor vychází ze situací, ne z dialogových vtipů. Příklad: mimozemšťané sledují zotročené lidi hrát fotbal a poražený tým okamžitě fyzicky likvidují. Absurdita spočívá v tom, že mimozemská civilizace zdánlivě přijala lidský zábavní rituál — a zároveň ho zbavila vší sentimentality.

## 2.5 Designová filozofie: Rules-Based Design

Hra **NENÍ** navrhována jako lineární scénář s jediným správným řešením. Hra **JE** navrhována jako sandbox s konzistentními pravidly.

### Princip

Hráč přežije, pokud:
- Pravidla **pochopí**
- Pravidla **dodržuje**
- Pravidla **kreativně využívá**

Hráč je trestán, pokud:
- Pravidla **ignoruje**
- Pravidla **porušuje**
- **Předpokládá výjimky**, které neexistují

### Designerova zodpovědnost

- Pravidla budou **KONZISTENTNÍ** napříč celou hrou
- Pravidla budou **VIDITELNÁ** pro hráče (telegraphing, feedback, environmental storytelling)
- Pravidla budou platit pro AI **stejně jako pro hráče**
- Hra **NEBUDE skriptovat výjimky** pro narativní účely
- **Quest cíle jsou dány, cesta k nim NE** — hráč si vybírá přístup
- Hráčovo rozhodnutí = hráčovy důsledky

### Princip "Common sense survival"

Pravidla přežití nejsou arbitrární game mechaniky. Jsou to **úvahy zdravého rozumu** — totéž, co by udělal myslící civilista nebo voják v reálné invazi:

- Mít nejlepší dostupnou technologii
- Skautovat před akcí
- Útočit z výhody (kryt, vzdálenost, překvapení)
- Při zranění se stáhnout
- Plánovat trasu, ne improvizovat

**Hra hráče NIC nenutí.** Hra dává **informace** a **nástroje**. Hráč se rozhoduje sám. Pokud zvolí špatně, zaplatí — ne proto, že hra ho potrestala, ale proto, že **zdravý rozum to přinesl**.

### Vztah k ostatním sekcím

Tato filozofie ovlivňuje:
- **Sekci 5** (Combat System) — lethality jako výsledek pravidel, ne game mechaniky
- **Sekci 6** (Progression) — drone, gadgety, alien sense jsou nástroje pro pravidla
- **Sekci 8** (Quest System) — multiple solutions přes pravidla
- **Sekci 11** (UI/UX) — informace, ne řešení

> **Detailní seznam pravidel přežití V1** je v samostatném dokumentu **Game Bible**.

---

# 3. CORE GAME LOOP — TŘÍÚROVŇOVÁ STRUKTURA

Hra **NEMÁ** jediný "core loop". Má **vrstvenou strukturu tří loopů**, které do sebe zapadají. Hráč zažívá všechny tři současně, ale na různých časových škálách.

## 3.1 Vrstva 1 — COMBAT LOOP (30 sekund – 3 minuty)

**Nejmenší loop. To, co dělá moment-to-moment hraní zábavné.**

```
ALERT (alien sense / vizuální detekce)
    ↓
SCOUT (vyhodnocení: kolik, kde, jaký typ, kde kryt?)
    ↓
POSITION (přesun do výhodné pozice, kryt, úhel)
    ↓
ENGAGE (jeden výstřel — kill nebo critical injury)
    ↓
RESOLVE (loot, ošetření, přesun)
```

- **Napětí:** "Přežiju tenhle boj?"
- **Odměna:** "Cítím váhu výstřelu, vidím ragdoll, mám loot."
- **Frekvence v hraní:** desítky-stovky × za session

**Toto je nejdůležitější loop ve hře.** Pokud combat není zábavný v 30sekundovém testu, nic dalšího nezachrání hru.

## 3.2 Vrstva 2 — ENCOUNTER LOOP (5–15 minut)

**Středně dlouhý loop. Co hráč udělá v jednom „sezení u POI."**

```
NAJDU POI (budova, vrak, struktura, NPC hub)
    ↓
SKAUTUJU (drone? pozorování? naslouchání?)
    ↓
ROZHODNU PŘÍSTUP (boj / plížení / vyjednávání / útěk / vyhnutí)
    ↓
PROVEDU PLÁN (combat loops uvnitř, nebo non-combat akce)
    ↓
ZISK (loot + informace + možnost morální volby)
    ↓
DOPAD (lokální invasion %, NPC reakce)
```

- **Napětí:** "Co je správný přístup k téhle situaci?"
- **Odměna:** "Loot, info, něco se změnilo ve světě."

### 3.2.1 Encounter typy

Encounter NENÍ jen combat. **Kritická součást gameplay** je mix combat a non-combat encounterů, které vznikají přirozeně z hráčova průzkumu světa.

**Combat encounter typy:**
- Ambush (přepad)
- Stronghold (opevněná pozice)
- Patrol intercept
- Defense (obrana NPC)
- Hit-and-run

**Non-combat encounter typy:**
- Příprava / Hub (základna, crafting, save)
- Stealth průchod
- Vyjednávání / Dialog
- Hackování / Puzzle
- Lore / Discovery (deníky, záznamy)
- Crafting na místě
- Travel / Kontemplace
- Morální volba (bez combat tlaku)

Encountery vznikají **systémově** ze stavu světa (invasion %, alignment, čas, hráčova historie). **Designér nepředepisuje fixní pořadí** — hráč objevuje encountery sám podle své trasy a stylu hraní.

## 3.3 Vrstva 3 — QUEST LOOP (30–90 minut)

**Dlouhý loop. To, co hráč udělá v jedné herní seanci.**

```
QUEST INITIATION (NPC, dynamic event, exploration discovery)
    ↓
CESTA K CÍLI (encounter loops uvnitř — combat i non-combat)
    ↓
KLÍČOVÉ ROZHODNUTÍ (lidé / alien / egoismus)
    ↓
DOPAD (invasion %, alignment, world state changes)
    ↓
UPGRADE (XP, skill unlock, technologie, gear)
```

- **Napětí:** "Jaké rozhodnutí udělám a jak změní svět?"
- **Odměna:** "Vidím konkrétní změnu světa kvůli mému rozhodnutí."

## 3.4 Vizualizace nested loops

```
┌─────────────────────────────────────────────────────┐
│  QUEST LOOP (30–90 min)                             │
│  „Co rozhodnu? Jak změním svět?"                    │
│                                                     │
│   ┌────────────────────────────────────────────┐    │
│   │  ENCOUNTER LOOP (5–15 min)                 │    │
│   │  „Jak se k téhle situaci postavím?"        │    │
│   │                                            │    │
│   │   ┌──────────────────────────────────┐     │    │
│   │   │  COMBAT LOOP (30s–3 min)         │     │    │
│   │   │  „Přežiju tenhle souboj?"        │     │    │
│   │   └──────────────────────────────────┘     │    │
│   │                                            │    │
│   └────────────────────────────────────────────┘    │
│                                                     │
└─────────────────────────────────────────────────────┘
```

## 3.5 Cíle pro V1 testování

- **Priorita 1:** Otestovat Combat Loop v greyboxu — je sám o sobě zábavný?
- **Priorita 2:** Otestovat Quest Loop end-to-end — cítí hráč váhu rozhodnutí?
- **Encounter Loop** vyplyne přirozeně z těchto dvou.

> Greybox testovací rutina je v dokumentu **Workflow Checklist**, Fáze 1.5.

---

# 4. INVASION SYSTEM

Invasion System je klíčová mechanika celé hry. Jde o dynamický stav světa, který reaguje na hráčova rozhodnutí a zpětně ovlivňuje herní podmínky.

## 4.1 Globální invaze (0–100 %)

- Reprezentuje celkový stav invaze na Zemi
- Ovlivňuje spawn nepřátel, dostupnost questů a chování NPC
- Mění se na základě hráčových akcí i pasivně v čase

## 4.2 Lokální invaze (per region)

- Každý region má vlastní hodnotu invaze
- Hráč může aktivně snižovat nebo zvyšovat lokální invazi
- Lokální invaze se promítá do globálního průměru

## 4.3 Player Alignment

- **Human (0–33 %):** hráč pomáhá lidem, odmítá mimozemskou technologii
- **Hybrid (34–66 %):** hráč kombinuje obě cesty, svět reaguje ambivalentně
- **Alien-aligned (67–100 %):** hráč přijal mimozemskou moc, lidé se mu vyhýbají

## 4.4 Dopady invaze na svět

- **Spawn nepřátel:** typ a hustota nepřátel se mění podle invasion %
- **Stav měst:** budovy, NPC dialogy a dostupné obchody se mění
- **Dostupnost questů:** některé questy se odemykají nebo zamykají
- **Chování NPC:** strach, spolupráce, útok — vše závisí na alignmentu a invazi

**⚠ TBD:** Rychlost změny invaze (balancing). Doporučení: spreadsheet s hodnotami per akci, otestovat 3 různé rychlosti v prvních playtestech.

**⚠ TBD:** Idle progression. Doporučení: ANO — pasivní ticker (~+0.5 %/h) přidá časový tlak.

---

# 5. COMBAT SYSTEM

Bojový systém je third-person shooter doplněný o melee. **Filozofie:** lethality-first — combat je realistický, smrtící a vyžaduje uvažování. Není to arcade shooter.

## 5.1 Lethality principy

Combat je navržen kolem principu **„jedna rána = vážný následek"**. Hráč i nepřítel jsou křehcí.

| Zásah | Výsledek (hráč i nepřítel) |
|---|---|
| **Headshot** | Instant kill |
| **Body shot (torso)** | Critical injury (znemožňuje pokračování v boji) |
| **Limb shot** | Movement penalty + bleeding |
| **Brnění (alien)** | Pokrývá konkrétní části, vyžaduje výběr cíle |
| **Melee zezadu** | Instant kill (hráč i nepřítel) |
| **Melee frontálně** | Critical injury, riskantní |
| **Exploze** | Mass damage + ragdoll fyzika |

**Critical injury** není „menší HP". Je to **stav**, který vynucuje **stažení**:
- Pohyb se sníží o ~50%
- Aim drift (zbraň se chvěje)
- Bleeding tick (postupně umíráš bez ošetření)
- ~60s do smrti, pokud se neošetříš

> **Detailní damage tabulka (zbraň × část těla × výsledek)** je v dokumentu **Tech Spec**.

## 5.2 Pocit boje

- **Pomalý** — žádné arcade tempo
- **Těžký** — každý zásah má váhu (zvuk, ragdoll, kamera shake)
- **Strategický** — hráč přemýšlí, ne reaguje reflexivně
- **Zaslouženě obtížný** — smrt je vždy vysvětlitelná („to byla moje chyba")

## 5.3 Hit detection

- **Ranged:** projectile-based (fyzická střela, ne hitscan)
- **Melee:** hitbox + animace
- **Body parts:** každá postava má detailní hitbox systém (head, torso, limbs)
- **Brnění:** layered (alien postavy mají brnění na konkrétních místech)

## 5.4 AI — vrstvený přístup

### V1 (Vertical Slice)

- **Patrol:** předem dané trasy
- **Detection:** sight (50m line-of-sight) + sound (20m radius)
- **Attack:** přesné, lethal
- **Flee:** při nízkém HP nebo přesile

### V2 (post-V1)

- Cover usage (využívání krytu)
- Sound investigation (vyšetřování podezřelých zvuků)
- Coordinated attacks (skupinové taktiky)

### V3 (long-term)

- Zotročování civilistů na Puppety
- Emergent behavior (nepřátelé spolupracují bez skriptu)

> **Důležité pro lethality:** I V1 AI musí mít alespoň základní cover behavior, jinak combat nebude napjatý.

## 5.5 Typy nepřátel (V1)

| Nepřítel | Charakteristika | Klíčová mechanika |
|---|---|---|
| **Alien** | Boj na dálku i blízko, základní jednotka invaze | Někteří silnější díky technologii (mini-elite, ne boss) |
| **War Dog** | Rychlý, melee, zvířecí AI | Slabší zrak, lepší sluch |
| **Puppet** | Zotročený člověk s alien implantáty | Ovládán Puppeteerem na ~30m. Smrt Puppeteera = okamžitá smrt Puppeta |
| **Puppeteer** | Alien operator stojící za scénou | Vizuálně odlišný (ovládací zařízení, antény). Klíčový taktický cíl |
| **Hostile humans** | Kolaboranti nebo skupiny vnímající hráče jako hrozbu | Reaktivní na alignment a invasion % |

### 5.5.1 Puppet / Puppeteer mechanika

**Klíčová taktická pravidla:**
- Puppet je **mrtvý okamžitě**, když Puppeteer zemře (telepatická vazba)
- Puppeteer stojí **~30m od Puppeta**, často v krytu
- Puppet sám se chová **slabě** (zpomalená reakce, naprogramovaný pohyb)
- Hráč musí Puppeteera **identifikovat a eliminovat** pro efektivní vyřešení encounter
- **Headshot zezadu** Puppeteera = stealth eliminace celé skupiny

**Lore:**
- Puppet je člověk, jehož mysl je ovládána alien technologií
- Implantáty jsou viditelné na těle (fluorescentní stopy)
- Po smrti Puppeteera Puppet kolabuje — animace shutdown

## 5.6 Boss fight: NE

Žádný tradiční boss fight v V1. Silnější nepřátelé jsou výsledkem napojení na mimozemskou technologii — jsou to **elite jednotky**, ne speciální encounter. Lethality systém **nemá smysl s HP-houba bosse**.

---

# 6. PROGRESSION SYSTEM

Progrese hráče je rozdělena do dvou hlavních cest (lidská vs. mimozemská) a jedné sdílené vrstvy (skill progression). Cesty jsou funkčně i vizuálně odlišné.

## 6.1 Mimozemská augmentace (Alien path)

- **Vyšší stats:** síla, rychlost, regenerace
- **Nové schopnosti:** telekineze, mimikry, energie, alien sense
- **Negativní důsledek:** zhoršují se vztahy s lidskými NPC
- **Vizuální dopad:** UI a vnímání světa se mění (alien filtr)

**Filozofie:** *"Brutální síla, fyzická převaha, ale ztráta lidskosti."*

## 6.2 Lidská technologie (Human path)

- **Gadgety:** granáty, **Recon Drone**, scanery, hackovací zařízení, EMP
- **Hackování:** přebírání mimozemských systémů
- **Sociální výhody:** NPC více důvěřují, lepší ceny, bonusové questy
- **Informační převaha:** drone scout, tactical map, intel

**Filozofie:** *"Mozek místo svalů. Informace místo síly. Plán místo improvizace."*

> Klíčový design princip: **Human path NESMÍ být mechanicky slabší** v lethality combatu. Vyrovnává to **informační převahou** (drone, mapa, hackování) a **lepším přístupem k zdrojům** (NPC vztahy).

## 6.3 Skill Progression

- Schopnosti se odemykají **přes gameplay**, ne jen leveling
- Příklad: 10× použití hackování → odemkne pokročilý hack
- Sdílená pro obě cesty (skills jako střelba, fyzická kondice, atd.)

## 6.4 Recon Drone (Klíčová Human technologie)

**Recon Drone** je hráčův RC dron pro pasivní průzkum prostředí. Není to bojový dron — je to **informační nástroj**. Implementuje pravidlo přežití *"skautuj před akcí"*.

> ⚠ **Terminologická poznámka:** "Drone" v game world označoval dříve zotročeného člověka (nyní **Puppet**). Hráčův dron je **Recon Drone** (UAV).

### 6.4.1 Tier 1 — Scout Drone (V1 funkční)

| Atribut | Hodnota |
|---|---|
| **Kamera** | Pasivní (vidíš co dron vidí) |
| **Range** | ~100 m od hráče |
| **Battery** | ~60 sekund letu |
| **Rychlost** | Pomalá (nutí hráče plánovat) |
| **Hluk** | Tichý (nepřitahuje aliény, pokud není moc blízko) |
| **Inventory cost** | 1 slot |
| **Detekce alienů** | Vizuální spatření na otevřeném prostranství |
| **GPS integration** | ✅ Přidává body na mapu (heatmap update) |

**Limitace (klíčové pro balanc):**
- ❌ Nemůže letět skrz zdi/budovy
- ❌ Aliené ho mohou sestřelit (ztráta resource)
- ❌ Battery limit (časový tlak)
- ❌ EMP zóny ho deaktivují
- ❌ Hráč je nehybný/zranitelný během ovládání

### 6.4.2 Tier 2 — Recon Drone (architektura připravena, V1/V2 TBD)

Vylepšení:
- Audio sensor
- Biometrika (alien detection skrz lehký kryt)
- 120s battery, 200m range
- Označování cílů na mapě
- Craftovaný z alien materiálů (knowledge gate)

### 6.4.3 Tier 3 — Tactical Drone (V2/V3, architektura připravena)

Vylepšení:
- Carry payload (granát, EMP, distrakce)
- 240s battery, 500m range
- Photo log, předpovědi patrol vzorců

### 6.4.4 Design role dronu

- **Posiluje human path** (informace = výhoda)
- **Implementuje pravidlo "skautuj před akcí"**
- **Vytváří pre-combat encounter** (5-10 min planning gameplay)
- **Umožňuje emergent strategie** (drone & distract, drone lure, drone bait)
- **Eskalace v čase** (V2/V3): aliené adaptují, jamming pole

> **Detailní statistiky a balancing** jsou v dokumentu **Tech Spec**.

## 6.5 Další human gadgety (V1+)

| Gadget | Účel | V1? |
|---|---|---|
| **Granáty** | Mass damage, distrakce | ✅ V1 |
| **EMP granát** | Krátkodobá deaktivace alien tech | V2 |
| **Hackovací zařízení** | Otevírání alien zámků, deaktivace dronů | V1 (omezené) |
| **Motion sensor** | Statický detector pohybu | V2 |
| **Pasti** | Mine, šňůry | V2/V3 |
| **Smoke / Decoy** | Distrakční zařízení | V2 |
| **Optical scope** | Lepší dohled bez drona | V1 |
| **Komunikátor** | Slyšet alien rádio přenosy | V2/V3 |

**⚠ TBD:** Která Tier 2 features dronu jsou ve V1 vs. V2 (architektura připravena pro všechny).

**⚠ TBD:** Max level / cap. Doporučení: pro V1 soft cap level 5 v každé větvi.

**⚠ TBD:** Respec. Doporučení: NE v V1.

---

# 7. LOOT SYSTEM

Loot systém zásobuje hráče materiálem pro upgrade a příběhovými informacemi. Komplexita V1 minimální.

## 7.1 Typy lootu

- **Technologie:** komponenty pro upgrade gadgetů a augmentací
- **Materiály:** suroviny (kovový šrot, mimozemský krystal)
- **Informace:** deníky, záznamy, kódy — narativní loot

## 7.2 Logika dropu

- Loot závisí na **typu nepřítele** (voják vs. vědecký typ)
- Loot závisí na **invasion %** (vyšší invaze = více alien lootu)

✅ **Inventory limit:** Duální systém.
- **Hmotnostní limit:** roste se silou hrdiny
- **Prostorový limit:** fixní počet slotů

✅ **Crafting systém:** Knowledge gate + materiálová podmínka.
- Hráč nejprve získá **znalost** (quest, loot, výzkum)
- Poté může podle znalosti **vyrobit nebo upravit** předmět
- Vyžaduje **alien materiály** ze světa
- Propojuje progression, loot a narativní vrstvu

---

# 8. QUEST SYSTEM

Quest systém kombinuje scripted příběhové momenty se systémovými questy generovanými stavem světa.

## 8.1 Typy questů

- **Hlavní linka:** příběh lidé vs. mimozemšťané
- **Region control:** questy ovlivňující invasion % v regionu
- **Technologie unlock:** questy odemykající nové schopnosti
- **Osobní příběhy:** vedlejší questy NPC s morálními volbami

## 8.2 Struktura

- **Hybrid:** část questů scripted, část systémová
- **Viditelný dopad:** každý quest má měřitelný výsledek
- **Multiple solutions:** quest cíle jsou dány, **cesta k nim NE** (rules-based)

✅ **V1 obsahuje 3 questy**, které hráče provází od lesa přes město ke zničení mimozemského modulu lodi.

## 8.3 Vztah k Rules-Based Designu

Questy **neříkají** hráči, jak je splnit. Říkají **co je cíl**. Hráč:
- Zjišťuje **pravidla světa** (sekce 2.5)
- Sbírá **informace** (NPC, lore, scout)
- Rozhoduje **přístup** (combat / stealth / vyjednávání / sabotage)
- Provede **plán** podle vlastního stylu

**Příklad:** Quest "Znič modul" lze splnit:
- Stealth + sabotáž jádra zevnitř
- Hlasitý útok s granáty
- Manipulace alienů, aby se zničili sami
- Vylákání alien posádky a útok zvenčí
- EMP útok (alien-aligned hráč)
- Vyjednávání (high-alignment alien path)

**⚠ TBD — TÝKÁ SE PŘÍBĚHU:** Větvení questů (počet hlavních story branches v V1). Toto je narativní rozhodnutí, ne mechanické — pravidla světa už multiple solutions umožňují.

---

# 9. WORLD DESIGN

Svět je rozdělen do regionů s vlastním stavem invaze. V1 obsahuje jeden region s plnou funkčností.

## 9.1 Struktura světa

- **Grid-based regiony:** každý region je samostatná herní zóna
- Každý region má vlastní invasion %, spawn tabulky a dostupné questy
- **Additive scenes:** Unity streaming pro velké plochy

## 9.2 Obsah V1 regionu

- **Les:** startovní oblast — bezpečnější, méně nepřátel, orientační bod
- **Město:** centrum questů, NPC interakcí a dynamických událostí
- **Modul vesmírné lodi:** na okraji města — základna alienů, cíl V1 (zničení)
- **Propojení:** cesta z lesa přes město k modulu = přirozená progression trasa

## 9.3 Dynamické události

- **Časově omezené:** zmizí bez hráčovy intervence
- **Dynamické:** reagují na aktuální invasion %
- **Příklady:** alien útok na civilisty, lidský odpor, crash lodi

**⚠ TBD:** Počet POI ve V1. Doporučení: 4–6 POI podél trasy.

**⚠ TBD:** Hustota eventů. Doporučení: 2–4 paralelní v jednu chvíli per region.

---

# 10. TECHNICKÁ ARCHITEKTURA

Technická architektura musí umožnit rychlou iteraci V1 a zároveň neblokovat scaling na plnou mapu.

## 10.1 Core systémy (V1 scope)

- **Player Controller:** pohyb, střelba, melee, interakce, drone control
- **AI System:** patrol / detection / attack / flee + cover (V1 vrstva)
- **Combat System:** hit detection, body parts damage, critical injury state
- **Inventory System:** loot, dual limit (weight + slots)
- **Quest System:** tracking, multiple solutions, dopady
- **Save / Load System:** free save, full state serialization
- **GPS / Tactical Map System:** mini-mapa, heatmap, decay
- **Recon Drone System:** kamera switch, battery, detection logging

## 10.2 World Architecture

- **Additive Scenes:** streaming regionů
- **Scriptable Objects:** data-driven (nepřátelé, loot, questy, recepty)
- **LOD:** optimalizace
- **Object Pooling:** spawn/despawn

## 10.3 Fyzika a destrukce

✅ **Ragdoll systém:** Realistická fyzika těl při zásahu (směr, síla, typ zbraně).

✅ **Destrukce prostředí:** Částečně destruktivní tam, kde má dopad na gameplay.

✅ **Save systém:** Free save.

**⚠ TBD:** Multiplayer future-proofing. Doporučení: NE aktivně, ANO pasivně (oddělená state logika).

**⚠ TBD:** Cílový hardware. Doporučení: GTX 1660 / RTX 2060, 16 GB RAM (HDRP náročnost).

---

# 11. UI / UX

UI je ovlivněno stavem hráčovy transformace — alignment mění vzhled HUD. **Filozofie:** diegetický UX (informace, ne řešení).

## 11.1 Principy

- UI reaguje na **player alignment** (Human / Hybrid / Alien-aligned)
- **Dynamický HUD:** v klidu minimální, v boji plný
- **Stylizované sci-fi UI** s diegetickým ráměním (smartphone/wrist device pro Human, alien filtr pro Alien-aligned)
- Minimalistický přístup: zobrazovat jen to, co hráč potřebuje

## 11.2 GPS / Tactical Map System

**Filozofie:** *"Hra dává INFORMACE, ne ŘEŠENÍ. Hráč si vybírá CESTU sám."*

V roce 2050 je GPS s navigací standardní technologie. Hráč začíná hru s běžným zařízením, které je vždy dostupné.

### 11.2.1 Mini-mapa (vždy viditelná)

- Roh obrazovky (~200x200 px)
- Hráčova pozice ve středu
- GPS šipka k aktivnímu cíli (modrá, jako auto GPS)
- Heatmap overlay (pokud existují drone data)
- Možnost vypnout v settings (pro hardcore mód)

### 11.2.2 Plná mapa (Tab/M)

- Otevře full-screen mapu
- Vrstvy (zapínat/vypínat):
  - Hrozby (alien pozice s decay)
  - Tech (statické struktury)
  - Heatmap rizika
  - Visibility (line-of-sight z alien pozic)
  - Terén
- Personal markers (hráč si přidává vlastní piny — V2)

### 11.2.3 Heatmap colors (5 úrovní)

| Barva | Stav | Význam |
|---|---|---|
| 🟢 **Sytá zelená** | Scout < 5 min, čisto | "Bezpečno, jdi" |
| 🟡 **Žluto-zelená** | Scout 5-10 min, čisto | "Pravděpodobně bezpečno, ověř" |
| ⚪ **Šedá** | Bez data nebo > 10 min | "Neznám tuto oblast" |
| 🟠 **Oranžová** | Alien spatřen 5-10 min | "Byl tu nepřítel, možná tam ještě je" |
| 🔴 **Červená** | Alien spatřen < 5 min | "Aktivní hrozba, vyhni se" |
| ⚫ **Tmavě červená** | Permanentní alien tech | "Statická hrozba (modul, věž, EMP)" |

### 11.2.4 Information decay

- Data automaticky stárne v čase
- Statické struktury (modul, věže) **nestárnou**
- Dynamické info (aliené, patroly) **decay** podle časovače
- Hráč musí scoutovat znovu pro fresh data
- Decay je **automatický** (timer-based)

**⚠ TBD:** Konkrétní decay timing. Doporučení V1: 5/10/15/30 min thresholds.

### 11.2.5 Update logic

- Mapa updatuje **pouze za drone scoutingu** (ne real-time)
- Mezi scouty: stará data, decay
- Nový scout = refresh barev v dosahu
- Chybí scout → šedá zóna (no data)

### 11.2.6 Co mapa NEDĚLÁ (rules-based zachování)

- ❌ Nedoporučuje strategii
- ❌ Nemarkuje "best approach"
- ❌ Neoznačuje "stealth route"
- ❌ Nekreslí trasu kromě GPS šipky k cíli
- ✅ Pouze ukazuje DATA, hráč rozhoduje

> GPS šipka = **fyzický směr** k cíli (jako auto GPS).
> Heatmap = **informace** o riziku.
> **Strategie = volba hráče.**

> **Detailní specifikace** (decay timery, update logic, edge cases) je v dokumentu **Tech Spec**.

---

# 12. MVP — VERTICAL SLICE (V1)

Vertical slice ověřuje, že core mechaniky fungují tak, jak jsou navrženy.

## 12.1 Obsah V1

- 1 region: les → město → modul vesmírné lodi
- Město: ulice, dynamičtí NPC, budovy
- 4 typy nepřátel: alien, war dog, Puppet (+ Puppeteer), nepřátelští lidé
- 3 questy → cíl: zničení modulu
- Základní invasion systém (globální + lokální)
- Free save
- Základní crafting (knowledge gate + materiály)
- Duální inventory limit
- Ragdoll fyzika + destrukce prostředí
- Lethality combat (ranged + melee, body parts damage)
- Tier 1 Recon Drone funkční (architektura pro Tier 2/3)
- GPS / Tactical Map s heatmapou a decay
- Základní progression (lidská i alien cesta)
- Rules-based design ~15 V1 pravidel (viz Game Bible)

## 12.2 Cíle V1

- Ověřit **Combat Loop** v greyboxu (priorita 1)
- Ověřit **Quest Loop** end-to-end (priorita 2)
- Ověřit **invasion mechaniku** (cítí hráč dopad?)
- Ověřit **lethality combat** (zaslouženě obtížný, ne frustrující?)
- Ověřit **rules-based design** (pochopí hráč pravidla bez tutorialu?)
- Ověřit **progression** (jsou cesty smysluplně odlišné?)

## 12.3 Validace přes greybox

**Před art passem (Fáze 4)** proběhne **greybox validace** (Fáze 1.5 ve Workflow). Detailní rutina v dokumentu **Workflow Checklist**.

**⚠ TBD:** Deadline V1.

**⚠ TBD:** Team size. Doporučení: 2–4 lidé s jasnými rolemi.

---

# 13. OUT OF SCOPE (V1)

- Plná mapa 20×20 km
- Pokročilá AI (V2 a V3 vrstvy nad rámec V1 cover)
- Kompletní příběh
- Velké množství herních assetů
- Multiplayer
- Plný crafting systém
- Boss fight (rozhodnuto: NE)
- Character creation (rozhodnuto: pevná postava)
- Lokalizace
- Tier 3 Tactical Drone (architektura ano, implementace V2/V3)
- Personal markers na mapě (V2)
- Day/Night cycle (V2)
- Weather system (V2/V3)

---

# 14. WORKFLOW DOPORUČENÍ PRO TÝM

## 14.1 Okamžité priority (před zahájením produkce)

- Potvrdit a zapsat všechny TBD ze sekce 0
- Definovat team size a role
- Stanovit deadline V1
- **Vytvořit Game Bible v0.1** (~15 V1 pravidel přežití)
- **Vytvořit Tech Spec v0.1** (damage tabulky, drone statistiky, mapa decay)
- Vytvořit mood board pro vizuální styl
- Napsat 3 ukázkové humorné situace

## 14.2 První sprint (doporučené pořadí)

- Player controller (pohyb, kamera, lethality combat)
- Základní AI (patrol + attack + cover)
- Invasion % tracker (backend bez UI)
- Recon Drone Tier 1 (kamera switch, battery)
- GPS / Tactical Map mini-version (heatmap stub)
- 1 quest end-to-end
- **Greybox playtest** (Fáze 1.5 z Workflow)

## 14.3 Nástroje pro koordinaci

- **PRD** (tento dokument): designová specifikace, stabilní
- **Game Bible:** pravidla přežití, živý dokument
- **Tech Spec:** mechaniky a balancing tabulky
- **Workflow Checklist:** operativní postup, fáze, assety
- Task tracker: GitHub Issues / Notion / Trello
- Verze: sémantické verzování (V1.0.1, V1.0.2)
- Playtest log: zápis každého playtestu

---

PRD v0.2 — Siege of the Blue World

*Dokument je živý — aktualizujte ho při každém TBD rozhodnutí.*

*Související dokumenty: Game Bible (pravidla), Tech Spec (mechaniky), Workflow Checklist (postup).*
