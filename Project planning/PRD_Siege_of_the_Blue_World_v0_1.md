**PRODUCT REQUIREMENTS DOCUMENT**

Siege of the Blue World

Verze: 0.1 — Vertical Slice (V1)

Engine: Unity | Platforma: PC | Typ: Singleplayer TPS Open-World RPG

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
| **Počet nepřátel V1** | **✅ Rozhodnuto** | 4 typy: mimozemšťan, war dog, dron, lidé |
| **Boss fight V1** | **✅ Rozhodnuto** | Ne — silnější jedinci díky technologii, ne boss |
| **Save systém** | **✅ Rozhodnuto** | Free save |
| **Crafting systém** | **✅ Rozhodnuto** | Ano — učení + mimozemské materiály |
| **Inventory limit** | **✅ Rozhodnuto** | Hmotnostní (roste se silou) + prostorový (fixní) |
| **Destrukce prostředí** | **✅ Rozhodnuto** | Ano |
| **Ragdoll systém** | **✅ Rozhodnuto** | Ano — realistická fyzika těl |
| **Team size / deadline** | **⚠ TBD** | Počet lidí a cílový datum V1 |
| **Cílový hardware** | **⚠ TBD** | Min. spec PC |
| **Multiplayer future-proof** | **⚠ TBD** | Architektura připravit / nepřipravit |

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

# 2. VIZE A CORE FANTASY

Hráč je obyčejný člověk uprostřed mimozemské invaze. Jeho rozhodnutí nejsou binární — existuje spektrum voleb mezi zachováním lidskosti, přijetím mimozemské moci a čistým egoisem. Svět na tato rozhodnutí reaguje viditelně a systémově.

## 2.1 Klíčové pilíře zážitku

- AGENCY: Každé rozhodnutí má měřitelný dopad na svět
- AMBIVALENCE: Žádná volba není jednoznačně správná
- ATMOSPHERE: Temná, s nádechem cynického humoru
- SYSTÉMOVOST: Svět reaguje automaticky, ne jen skriptovaně

✅ Identita hrdiny: Pevná postava se sdíleným počátečním backstory. Hrdina se vyvíjí v mnoha směrech v závislosti na rozhodnutích hráče — jeho fyzická podoba, schopnosti i vztahy s okolním světem se proměňují.

✅ Styl humoru: Vrstevnatý — cynický, suchý, absurdní i satirický zároveň. Humor vychází ze situací, ne z dialogových vtipů. Příklad: mimozemšťané sledují zotročené lidi hrát fotbal a poražený tým okamžitě fyzicky likvidují. Absurdita spočívá v tom, že mimozemská civilizace zdánlivě přijala lidský zábavní rituál — a zároveň ho zbavila vší sentimentality.

# 3. CORE GAME LOOP

Core loop je základní opakující se cyklus, který hráč prochází. Musí být navržen tak, aby byl ověřitelný v rámci V1 a škálovatelný pro plnou verzi.

## 3.1 Průběh jednoho loopu

- Průzkum světa — hráč se pohybuje po regionu
- Nalezení události / questu — scripted nebo dynamická událost
- Interakce — boj, dialog, hackování nebo rozhodnutí
- Zisk — loot, informace nebo technologie
- Rozhodnutí — lidé vs. mimozemšťané vs. egoismus
- Dopad — změna invasion %, stavu světa a postavy
- Upgrade — skills, gear nebo augmentace

## 3.2 Cílová délka loopu

**⚠ TBD — TO BE CONFIRMED:** *Průměrná délka jednoho loopu zatím není stanovena. Doporučení: cílit na 5–15 minut. Kratší je lépe pro testování — jeden loop by měl být dokončitelný v jednom sezení bez ukládání. Toto číslo je zásadní pro balancing invaze a quest designu.*

**⚠ TBD — TO BE CONFIRMED:** *Jak silně jeden loop ovlivní globální invasion %? Doporučení: definujte minimální a maximální váhu jedné akce. Například: minor quest = ±1–2 %, major quest = ±5–10 %. Bez tohoto není možné balancovat obtížnost.*

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

- Human (0–33 %): hráč pomáhá lidem, odmítá mimozemskou technologii
- Hybrid (34–66 %): hráč kombinuje obě cesty, svět reaguje ambivalentně
- Alien-aligned (67–100 %): hráč přijal mimozemskou moc, lidé se mu vyhýbají

## 4.4 Dopady invaze na svět

- Spawn nepřátel: typ a hustota nepřátel se mění podle invasion %
- Stav měst: budovy, NPC dialogy a dostupné obchody se mění
- Dostupnost questů: některé questy se odemykají nebo zamykají
- Chování NPC: strach, spolupráce, útok — vše závisí na alignmentu a invazi

**⚠ TBD — TO BE CONFIRMED:** *Počet regionů ve V1. Doporučení: 1 region s plně funkčním invasion systémem je dostačující pro ověření mechaniky. Více regionů zvyšuje scope a riziko nedokončení vertical slice.*

**⚠ TBD — TO BE CONFIRMED:** *Rychlost změny invaze (balancing). Doporučení: navrhněte spreadsheet s hodnotami per akci a otestujte 3 různé rychlosti (pomalá / střední / rychlá) v prvních playtestech.*

**⚠ TBD — TO BE CONFIRMED:** *Zda invaze může růst samovolně bez hráče (idle progression). Doporučení: ANO — přidá časový tlak a urgenci. Implementujte jako passivní ticker (+0.5 % per herní hodinu) s možností pozdějšího rebalancování.*

# 5. COMBAT SYSTEM

Bojový systém je third-person shooter doplněný o melee. Styl je pomalejší a realistický, s důrazem na pocit každého zásahu. Není to arcade shooter — každá situace by měla vyžadovat uvažování.

## 5.1 Základní charakteristiky

- Typ: Third Person Shooter + melee
- Ranged hit detection: projectile-based (fyzická střela)
- Melee hit detection: hitbox + animace
- Pocit boje: pomalý, těžký, každý zásah má váhu

## 5.2 AI — vrstvený přístup

### V1 (Vertical Slice)

- Patrol: nepřítel hlídkuje po předem dané trase
- Attack: detekuje hráče a útočí
- Flee: utíká při nízkém HP nebo přesile

### V2 (post-V1)

- Target selection: nepřátelé si vybírají hráče nebo NPC strategicky
- Environment reaction: využívání krytu, reakce na zvuk a světlo

### V3 (long-term)

- Zotročování civilistů
- Emergentní chování (nepřátelé spolupracují bez skriptu)

## 5.3 Typy nepřátel (V1)

- **Mimozemšťan** — boj na dálku i na blízko. Základní jednotka invaze. Někteří jsou silnější díky napojení na mimozemskou technologii (de facto mini-elite, ne boss).
- **Mimozemský War Dog** — rychlý, agresivní, specializovaný na melee útok. Analogie bojového zvířete.
- **Dron (Zotročený člověk)** — člověk s tělem modifikovaným mimozemskou technologií. Ovládán operátorem-mimozemšťanem na blízkém dosah. Zabití operátora = okamžitá smrt dronu. Klíčová taktická mechanika: hráč musí identifikovat a eliminovat operátora.
- **Lidé (nepřátelští)** — kolaboranti nebo skupiny, které vnímají hráče jako hrozbu. Chování závisí na player alignmentu a invasion %.

✅ Boss fight: NE. Žádný tradiční boss fight v V1. Silnější nepřátelé jsou výsledkem napojení na mimozemskou technologii — jsou to elite jednotky, ne speciální encounter.

# 6. PROGRESSION SYSTEM

Progrese hráče je rozdělena do dvou hlavních cest (lidská vs. mimozemská) a jedné sdílené vrstvy (skill progression). Cesty by měly být vizuálně i funkčně odlišné.

## 6.1 Mimozemská augmentace

- Vyšší stats: síla, rychlost, regenerace
- Nové schopnosti: například telekineze, mimikry, energie
- Negativní důsledek: zhoršují se vztahy s lidskými NPC
- Vizuální dopad: UI a vnímání světa se mění (alien filtr)

## 6.2 Lidská technologie

- Gadgety: granáty, drony, skeny
- Hackování: přebírání mimozemských systémů
- Sociální výhody: NPC více důvěřují, lepší ceny, bonusové questy

## 6.3 Skill Progression

- Schopnosti se odemykají přes gameplay, ne jen leveling
- Příklad: 10x použití hackování → odemkne pokročilý hack

**⚠ TBD — TO BE CONFIRMED:** *Jak exkluzivní jsou cesty? Doporučení: ne plně exkluzivní v V1. Hráč může kombinovat, ale plný potenciál každé cesty vyžaduje specializaci. Toto snižuje komplexitu V1 a zároveň umožňuje testovat obě cesty.*

**⚠ TBD — TO BE CONFIRMED:** *Max level / cap. Doporučení: pro V1 definujte soft cap (například level 5 v každé větvi), který je dosažitelný v rámci vertical slice, aby bylo možné otestovat celou progression křivku.*

**⚠ TBD — TO BE CONFIRMED:** *Respec (reset progression). Doporučení: NE v V1. Přidejte možnost respecu v pozdější verzi, až bude jasné, jak silně hráči chtějí experimentovat s různými buildy.*

# 7. LOOT SYSTEM

Loot systém zásobuje hráče materiálem pro upgrade a příběhovými informacemi. Jeho komplexita by měla být v V1 minimální — důraz je na funkčnost, ne množství obsahu.

## 7.1 Typy lootu

- Technologie: komponenty pro upgrade gadgetů a augmentací
- Materiály: suroviny (kovový šrot, mimozemský krystal apod.)
- Informace: deníky, záznamy, kódy — narativní loot

## 7.2 Logika dropu

- Loot závisí na typu nepřítele (voják vs. vědecký typ)
- Loot závisí na invasion % (vyšší invaze = více mimozemského lootu)

✅ Inventory limit: Duální systém.

- Hmotnostní limit: roste se zvyšující se silou hlavní postavy (silnější hrdina unese více)
- Prostorový limit: fixní — nezávisí na síle, vždy stejný počet slotů

✅ Crafting systém: Ano — vychází z učení se o mimozemské technologii.

- Hráč nejprve získá znalost (quest, loot, výzkum) o mimozemské technologii
- Poté může podle této znalosti upravit gear nebo vytvořit nový předmět
- K výrobě jsou potřeba mimozemské materiály ze světa
- Systém propojuje progression, loot a narativní vrstvu — vědět jak je podmínkou, mít co je limitací

# 8. QUEST SYSTEM

Quest systém kombinuje scripted příběhové momenty se systémovými questy generovanými stavem světa. V1 ověřuje, zda tato kombinace funguje v praxi.

## 8.1 Typy questů

- Hlavní linka: příběh lidé vs. mimozemšťané
- Region control: questy ovlivňující invasion % v regionu
- Technologie unlock: questy odemykající nové schopnosti
- Osobní příběhy: vedlejší questy NPC s morálními volbami

## 8.2 Struktura

- Hybrid: část questů je plně scripted, část reaguje na systémový stav světa
- Viditelný dopad: každý quest by měl mít měřitelný výsledek (invasion %, alignment)

✅ V1 obsahuje 3 questy, které hráče provází od lesa přes město ke zničení mimozemského modulu lodi. Questy jsou nadesignované jako primární cesta verticálním slicí.

**⚠ TBD — TO BE CONFIRMED:** *Větvení questů v V1. Doporučení: jednoúrovňové větvení (dvě volby, dva výsledky na klíčových momentech). Plné quest trees jsou příliš náročné na obsah pro vertical slice.*

# 9. WORLD DESIGN

Svět je rozdělen do regionů s vlastním stavem invaze. V1 obsahuje jeden region s plnou funkčností — město, základna a několik POI.

## 9.1 Struktura světa

- Grid-based regiony: každý region je samostatná herní zóna
- Každý region má vlastní invasion %, spawn tabulky a dostupné questy
- Additive scenes: Unity streaming pro velké plochy

## 9.2 Obsah V1 regionu

- Les: startovní oblast hráče — bezpečnější, méně nepřátel, orientační bod
- Město: centrum questů, NPC interakcí a dynamických událostí — ulice, budovy, civilisté
- Modul vesmírné lodi: na okraji města — základna mimozemšťanů, cíl V1 (zničení)
- Propojení: cesta z lesa přes město k modulu tvoří přirozenou progression trasu

## 9.3 Dynamické události

- Časově omezené: zmizí bez hráčovy intervence
- Dynamické: reagují na aktuální invasion %
- Příklady: mimozemský útok na civilisty, lidský odpor, crash lodi

**⚠ TBD — TO BE CONFIRMED:** *Počet POI ve V1. Doporučení: 4–6 POI podél trasy les → město → modul. Každé by mělo mít alespoň 1 napojení na quest nebo invasion event. Méně POI = rychlejší produkce a jasnější focus pro vertical slice.*

**⚠ TBD — TO BE CONFIRMED:** *Hustota eventů. Doporučení: 2–4 paralelní události v jednu chvíli na region. Více eventů najednou zvyšuje chaos (žádoucí při vysoké invazi), méně dává prostor pro klidnější průzkum.*

# 10. TECHNICKÁ ARCHITEKTURA

Technická architektura musí být navržena tak, aby V1 bylo možné iterovat rychle a zároveň aby neblokovala budoucí scaling na plnou mapu.

## 10.1 Core systémy (V1 scope)

- Player Controller: pohyb, střelba, melee, interakce
- AI System: patrol / attack / flee (V1 vrstva)
- Combat System: hit detection, damage, feedback
- Inventory System: loot sbírání, zobrazení, limitace
- Quest System: tracking, větvení, dopady
- Save / Load System: persistence herního stavu

## 10.2 World Architecture

- Additive Scenes: streaming regionů za běhu
- Scriptable Objects: data-driven přístup pro nepřátele, loot, questy
- LOD (Level of Detail): optimalizace výkonu na dálku
- Object Pooling: efektivní spawn/despawn nepřátel a projektilů

## 10.3 Fyzika a destrukce

✅ Ragdoll systém: Ano — fyzika těl reaguje realisticky na zásah (směr, síla, typ zbraně). Tělo se chová jinak při výstřelu, výbuchu nebo melee úderu.

✅ Destrukce prostředí: Ano — prostředí je částečně destruktivní tam, kde má dopad na gameplay (kryt se dá zničit, výbuchy mění taktické možnosti).

✅ Save systém: Free save — hráč může uložit kdykoli.

**⚠ TBD — TO BE CONFIRMED:** *Multiplayer future-proofing. Doporučení: NE aktivně, ale ANO pasivně. Neimplementujte multiplayer, ale navrhněte architekturu player controlleru a game state tak, aby nebyla fundamentálně neslučitelná s networked multiplayer (oddělená state logika od renderingu).*

**⚠ TBD — TO BE CONFIRMED:** *Cílový hardware. Doporučení: definujte minimální spec (CPU, GPU, RAM) před zahájením optimalizace. Navrhujeme: GTX 1060 / RX 580, 8 GB RAM, i5-8400 jako minimální cíl. Toto ovlivní všechna performance rozhodnutí.*

# 11. UI / UX

UI je ovlivněno stavem hráčovy transformace — čím více je hráč alien-aligned, tím více se UI vizuálně mění. Toto je silný narativní prvek, který by měl být navržen od začátku.

## 11.1 Principy

- UI reaguje na player alignment (Human vs. Alien-aligned)
- HUD zobrazuje invasion %, alignment a základní stats
- Minimalistický přístup: zobrazovat pouze to, co hráč potřebuje

**⚠ TBD — TO BE CONFIRMED:** *Realistické vs. stylizované UI. Doporučení: stylizované UI s sci-fi estetikout — snadněji čitelné, méně náročné na asset produkci, lépe reflektuje tón hry.*

**⚠ TBD — TO BE CONFIRMED:** *HUD minimalismus vs. informace. Doporučení: dynamický HUD — v klidu minimální (jen compass a health), v boji plný (ammo, minimap, invasion %). Toto zvyšuje imerzi a je technicky implementovatelné v V1.*

# 12. MVP — VERTICAL SLICE (V1)

Vertical slice je cílem první fáze vývoje. Jeho účelem NENÍ vydat hotový produkt, ale ověřit, že core mechaniky fungují tak, jak jsou navrženy.

## 12.1 Obsah V1

- 1 region: les (start) → město → modul vesmírné lodi (cíl)
- Město: několik ulic, dynamičtí NPC, budovy
- 4 typy nepřátel: mimozemšťan, war dog, dron (zotročený člověk), nepřátelští lidé
- 3 questy vedoucí ke zničení mimozemského modulu
- Základní invasion systém (globální + lokální)
- Free save
- Základní crafting (učení + materiály)
- Duální inventory limit (hmotnostní + prostorový)
- Ragdoll fyzika + destrukce prostředí
- Základní combat (ranged + melee)
- Základní progression (lidská i mimozemská cesta)

## 12.2 Cíle V1

- Ověřit core loop (průzkum → quest → rozhodnutí → dopad)
- Ověřit invasion mechaniku (zda hráč cítí vliv svých rozhodnutí)
- Ověřit combat feel (odpovídá pomalý realistický styl vizi?)
- Ověřit progression (jsou obě cesty funkčně odlišné?)

**⚠ TBD — TO BE CONFIRMED:** *Deadline V1. Doporučení: stanovte datum nejdříve s ohledem na team size. Bez znalosti počtu vývojářů nelze realisticky odhadnout. Jakmile je tým definován, navrhujeme sprint-based plánování s milestones každé 2–4 týdny.*

**⚠ TBD — TO BE CONFIRMED:** *Team size. Doporučení: pro V1 je optimální tým 2–4 lidí s jasně rozdělenými rolemi: 1 programmmer (Unity), 1 level/world designer, 1 game/quest designer, 1 artist (2D/3D). Pokud je tým menší, zredukujte scope V1 (méně POI, méně questů).*

# 13. OUT OF SCOPE (V1)

Následující prvky jsou explicitně mimo rozsah V1 a nesmí blokovat jeho dokončení:

- Plná mapa 20×20 km
- Pokročilá AI (V2 a V3 vrstvy)
- Kompletní příběh a narativní linka
- Velké množství herních assetů
- Multiplayer
- Plný crafting systém
- Boss fight (rozhodnuto: není součástí V1)
- Character creation / custom hrdina (rozhodnuto: pevná postava)
- Lokalizace (překlady)

# 14. WORKFLOW DOPORUČENÍ PRO TÝM

Tato sekce slouží jako praktický průvodce pro tým. Navrhuje pořadí kroků před zahájením vývoje.

## 14.1 Okamžité priority (před zahájením produkce)

- Potvrdit a zapsat všechny TBD položky ze sekce 0
- Definovat team size a role
- Stanovit deadline V1
- Vytvořit mood board pro vizuální styl (na základě potvrzeného vrstevnatého humoru)
- Napsat 3 ukázkové dialogy / situace pro ověření tónu humoru

## 14.2 První sprint (doporučené pořadí)

- Player controller (pohyb, základní střelba)
- Základní AI (patrol + attack)
- Invasion % tracker (backend bez UI)
- 1 quest end-to-end (od zadání po dopad na invazi)
- Playtest: ověření core loop

## 14.3 Nástroje pro koordinaci

- PRD (tento dokument): živý dokument, aktualizovat při každém TBD rozhodnutí
- Task tracker: GitHub Issues, Notion nebo Trello — tým si vybere
- Verze: sémantické verzování buildů (V1.0.1, V1.0.2 atd.)
- Playtest log: zapisovat každý playtest se zjištěními a rozhodnutími

---

PRD v0.1 — Siege of the Blue World

*Dokument je živý — aktualizujte ho při každém TBD rozhodnutí.*
