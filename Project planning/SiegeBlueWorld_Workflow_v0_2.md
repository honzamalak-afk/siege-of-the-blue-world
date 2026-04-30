**SIEGE OF THE BLUE WORLD**

Workflow · Checklist · Asset List

Verze: 0.2 — V1 Vertical Slice

*Interní dokument týmu — aktualizovat při každé změně scope nebo potvrzeném TBD.*

---

## CHANGELOG v0.2 (oproti v0.1)

- **Fáze 0:** + krok "Vytvořit Game Bible v0.1" + krok "Vytvořit Tech Spec v0.1"
- **Fáze 1.5 (NOVÁ):** Greybox Validation — 6 segmentů testovací rutiny před Fází 2
- **Část 3 (asset list):** Přidány Recon Drone assets, GPS / Tactical Map UI, Puppet/Puppeteer (přejmenování)
- **Terminologie:** Drone (zotročený člověk) → **Puppet** + nový **Puppeteer** (alien operator)

---

# ČÁST 1 — CHRONOLOGICKÉ FLOW SCHÉMA

Každý box představuje jednu fázi vývoje V1. Šipky ukazují závislosti — nelze začít další fázi, dokud není předchozí uzavřena.

---

## FÁZE 0 — PŘÍPRAVA & ROZHODNUTÍ (před zahájením produkce)

1. Potvrdit všechny TBD položky (team size, deadline, cílový hardware)
2. Rozdělit role v týmu: programmer / designer / artist / QA
3. Nastavit repozitář (Git) a pojmenovat projekt: Siege of the Blue World
4. **Vytvořit Game Bible v0.1** (~15 V1 pravidel přežití)
5. **Vytvořit Tech Spec v0.1** (damage tabulky, drone statistiky, mapa decay)
6. Vytvořit mood board — vizuální styl + 3 ukázkové humorné situace
7. Připravit Unity projekt: HDRP pipeline, folder strukturu, Git LFS
8. Vybrat task tracker (GitHub Issues / Notion / Trello) a vytvořit první backlog

▼

## FÁZE 1 — CORE SYSTÉMY — ZÁKLAD (Programmer)

1. Player Controller: pohyb (chůze, sprint, krčení), kamera TPS, základní interakce
2. **Lethality Combat:** body-parts hit detection, damage tabulka z Tech Spec
3. **Critical Injury State:** zpomalení, aim drift, bleeding, časový limit
4. Zdraví & smrt hráče: HP systém, death state, respawn / game over
5. Základní AI: patrol trasa, detection (sight + sound), attack state, flee state, **základní cover behavior**
6. Invasion % tracker: backend datový model (globální + lokální hodnota)
7. **Recon Drone Tier 1 (architektura pro Tier 2/3):** kamera switch, battery, range, detection upload
8. **GPS / Tactical Map:** mini-mapa, heatmap (5 barev), decay timer, GPS šipka
9. Save / Load systém: free save — full state serialization

*výstup: hratelná postava + AI + drone + mapa + save*

▼

## FÁZE 1.5 — GREYBOX VALIDATION (celý tým) ⚠ NOVÁ FÁZE

**Cíl:** Ověřit, že všechny core systémy fungují **dohromady**, dříve než se začne stavět svět.

**Princip:** Stavíme **funkční prototyp** ze šedých kostek a UI textu. Žádné finální assety. Jen mechaniky.

### 1.5.1 Greybox setup (1-2 týdny)

Postavit **400m corridor mapu** se 4 segmenty:

```
ZÁKLADNA (start) → LES (200m) → MĚSTO (100×100m) → MODUL (50×50m)
```

Plus:
- Player capsule s lethality combat
- 4 typy nepřátel (alien, war dog, Puppet, Puppeteer, hostile human) jako různobarevné kapsle
- Drone (cube) s kamerou switch
- Mini-mapa s heatmap (UI text + barevné rectangly)
- 2 zbraně: Pistol (hlasitá) + Crossbow (tichá)
- Save/Load funkční
- 1 quest end-to-end ("Znič modul")

### 1.5.2 Test rutina (6 segmentů)

Každý segment testuje **jiné aspekty designu**:

#### Segment 1 — ZÁKLADNA (Příprava, 3-5 min)
- Encounter typ: Non-combat
- Test: pochopení ovládání, drone basics, save funkce
- Měření: čas v základně, použil drone? quest start?

#### Segment 2 — LES (Průzkum + první combat, 8-12 min)
- Encounter typ: Mix non-combat scout + 1 combat
- 2 alieni patrolují, 1 lore element (krvavá stopa)
- Test: drone scout funguje? Stealth vs. combat volba? Lethality combat?
- Měření: použil drone (cíl >70%), smrti (cíl 30-50%), strategie (stealth vs. loud)

#### Segment 3 — KRITICKÝ MOMENT (Volba + lore, 5-7 min)
- Encounter typ: Non-combat morální volba
- Mrtvé tělo Puppeta + lore deník + alien implantát
- Test: cítí hráč váhu volby? funguje text-based lore?
- Měření: kolik vzalo implantát (cíl 30-40%), kolik přečetlo deník (cíl >80%)

#### Segment 4 — MĚSTO HUB (Sociální encounter, 10-15 min)
- Encounter typ: Non-combat NPC dialog
- 2 civilisté (info), 1 kolaborant (quest)
- Test: NPC dialog choices, alignment impact
- Měření: využil NPC info (cíl >60%), alignment shift

#### Segment 5 — STEALTH SEKCE (Před modulem, 7-10 min)
- Encounter typ: Stealth + Puppet/Puppeteer mechanika
- 1 alien + 1 Puppet + 1 Puppeteer
- Test: pochopil hráč Puppeteer trick? Stealth funguje?
- Měření: kolik objevilo Puppeteer trick (cíl >50%)

#### Segment 6 — MODUL (Finále + dopad, 5-10 min)
- Encounter typ: Climax + EMP zóna + dopad
- Test: drone selhání v EMP, finále napětí, dopad na invasion %
- Měření: completion rate (cíl 30-50% na první pokus), subjective rating

### 1.5.3 Metriky

**Kvantitativní:**
- Délka questu: 30-45 min
- Počet smrtí: 1-3 per playthrough
- Use rate dronu: >70% encounterů
- Stealth vs. combat ratio: 40-60% stealth
- Heatmap engagement: >80% checked map
- Decision diversity: žádná volba >70%

**Kvalitativní (1-10 stupnice):**
- "Cítil jsi váhu lethality?"
- "Byly volby smysluplné?"
- "Pochopil jsi pravidla bez tutorialu?"
- "Chtěl bys hrát další quest?"
- "Byl drone užitečný?"
- "Cítil jsi 'flow' nebo frustraci?"

**Cíl:** všechny subjective ≥7/10

### 1.5.4 Test protokol

**Iterace 1:** Solo (designer/programmer hraje 3×, zaznamenává)
**Iterace 2:** Solo po opravách (3× re-test)
**Iterace 3:** Externí playtest (3-5 lidí mimo tým, žádný tutorial)
**Iterace 4:** Final tweaks po externím feedbacku

**Total time investment:** 3-6 týdnů (worth it — odhalí fundamental design issues PŘED měsíci práce na vizuálu).

### 1.5.5 Decision gate

Před přechodem do Fáze 2:

✅ Subjective rating >7/10 napříč všemi otázkami
✅ Žádná dominantní strategie (multiple solutions fungují)
✅ Hráč pochopil pravidla bez tutorialu
✅ Lethality cítí "zaslouženě těžké", ne frustrující
✅ Drone systém intuitivní
✅ GPS / Map systém čitelný

**Pokud cokoliv selže → iterujeme design, NE přejdeme na vizuál.**

*výstup: Validovaná core gameplay — připravena pro World Design fázi*

▼

## FÁZE 2 — WORLD & LEVEL DESIGN (Designer + Artist)

1. Blockout mapy V1: les → město (ulice) → modul lodi — pouze šedé boxy
2. Definovat POI (4–6 bodů zájmu) a jejich napojení na quest systém
3. Nastavit additive scenes a streaming zón pro Unity
4. Rozmístit spawn pointy nepřátel dle invasion % logiky
5. Definovat kolizní geometrii a navigační mesh (NavMesh) pro AI
6. Definovat **EMP zóny** kolem alien tech (sféry s trigger collidery)
7. Základní osvětlení scény (mood lighting — temné, sci-fi tón)
8. **Validace pravidel:** procházet pravidla z Game Bible v každém POI

*výstup: hratelná mapa V1 v blockoutu*

▼

## FÁZE 3 — NEPŘÁTELÉ, QUEST & PROGRESSION (Programmer + Designer)

1. Implementovat 4 typy nepřátel: alien, war dog, **Puppet (+ Puppeteer mechanika)**, hostile humans
2. **Puppet/Puppeteer link logic:** smrt Puppeteera = instant Puppet collapse
3. Ragdoll systém: fyzická reakce těl na zásah
4. Destrukce prostředí: označit destruktivní objekty, implementovat destroy trigger
5. Quest systém: tracking, multiple solutions (rules-based), dopad na invasion %
6. 3 questy V1: nadesignovat, naskribtovat, implementovat (les → město → modul)
7. Inventory systém: hmotnostní limit + prostorový limit (Tech Spec sekce 8)
8. Loot systém: drop tabulky dle typu nepřítele a invasion %
9. Crafting systém: knowledge gate + materiálová podmínka
10. Progression: základní skill tree (lidská větev + alien větev), XP/unlock logika
11. Invasion systém: napojit hráčovy akce na invasion %, pasivní ticker

*výstup: plně hratelné questy + nepřátelé + progression*

▼

## FÁZE 4 — ART PASS & UI (Artist + Programmer)

1. Nahradit blockout assety finálními 3D modely (město, les, modul lodi)
2. Finální modely postav: hrdina, alien, war dog, **Puppet (s alien implantáty)**, **Puppeteer (s ovládacím zařízením)**, NPC civilisté
3. Animace: pohyb, útok, smrt, idle, **Puppet collapse** — pro každý typ postavy
4. **Recon Drone 3D model + animace** (start, létání, sestřelení)
5. VFX: výstřely, exploze, destrukce, alien efekty, **EMP pulses**, **Puppeteer-Puppet link visualization**
6. Zvuk: zbraně, kroky, nepřátelé, prostředí, hudba (ambient + combat)
7. UI / HUD: health bar, invasion %, alignment indikátor, inventory screen, quest log
8. **GPS / Tactical Map UI finalizace:** mini-mapa, heatmap rendering, full map screen
9. UI transformace: vizuální změna HUD dle player alignmentu (human vs. alien-aligned)
10. Finální osvětlení a post-processing (HDRP Volume profily, color grading, fog)

*výstup: vizuálně dokončená V1*

▼

## FÁZE 5 — PLAYTEST & ITERACE (celý tým)

1. Interní playtest #1: ověřit core loop end-to-end s vizuálem
2. Interní playtest #2: ověřit combat feel (lethality, ragdoll, destrukce)
3. Interní playtest #3: ověřit invasion mechaniku a alignment dopad
4. Zapsat playtest log: co funguje, co nefunguje, prioritní fixy
5. **Validace pravidel:** projít Game Bible — každé pravidlo otestovat v hraní
6. Iterační sprint: opravit kritické bugy a balancing problémy
7. Externí playtest (3-5 hráčů mimo tým): sběr zpětné vazby
8. Finální iterace na základě externí zpětné vazby

*výstup: otestovaná a iterovaná V1*

▼

## FÁZE 6 — V1 RELEASE — VERTICAL SLICE DOKONČEN

1. Code freeze: žádné nové features, pouze kritické bugy
2. Finální build: optimalizace výkonu (LOD, object pooling, profiling)
3. Sestavit release build pro PC
4. Interní prezentace V1: tým projde celou hrou a zhodnotí
5. Zpětná vazba a rozhodnutí: co jde do V2 scope
6. Aktualizovat dokumenty (PRD, Game Bible, Tech Spec) podle V1 zjištění
7. Archivovat build + dokumentaci

---

# ČÁST 2 — CHECKLIST PRO TÝM

Pořadí odpovídá chronologickému flow z Části 1.

## FÁZE 0 — PŘÍPRAVA

- ☐ Potvrdit TBD: team size, deadline, cílový hardware
- ☐ Rozdělit role (programmer / designer / artist / QA)
- ☐ Nastavit Git repozitář + pojmenovat projekt
- ☐ **Vytvořit Game Bible v0.1**
- ☐ **Vytvořit Tech Spec v0.1**
- ☐ Vytvořit mood board a 3 ukázkové humorné situace
- ☐ Připravit Unity projekt (HDRP pipeline, folder struktura, Git LFS)
- ☐ Vytvořit task backlog v task trackeru

## FÁZE 1 — CORE SYSTÉMY

- ☐ Player Controller (pohyb, kamera, interakce)
- ☐ Lethality Combat (body parts, damage tabulka)
- ☐ Critical Injury State (zpomalení, bleeding, timer)
- ☐ HP systém + smrt + respawn/game over
- ☐ AI: patrol, detection, attack, flee, **základní cover**
- ☐ Invasion % tracker (backend, bez UI)
- ☐ **Recon Drone Tier 1** (kamera switch, battery, detection upload)
- ☐ **GPS / Tactical Map** (mini-mapa, heatmap, decay, GPS šipka)
- ☐ Save / Load systém (free save)

## FÁZE 1.5 — GREYBOX VALIDATION ⚠ NOVÁ

- ☐ Postavit 400m corridor scénu (4 segmenty)
- ☐ Greybox setup: capsule hráč, kapsle nepřátelé, cube drone
- ☐ Mini-mapa s heatmap (UI text + rectangly)
- ☐ 2 zbraně (Pistol + Crossbow), Save/Load
- ☐ Implementovat 6 testovacích segmentů
- ☐ Solo playtest 3× + zaznamenat
- ☐ Iterace na základě solo feedbacku
- ☐ Externí playtest 3-5 lidí
- ☐ Final tweaks
- ☐ **Decision gate:** všechny subjective ratings ≥7/10
- ☐ Dokumentace: aktualizace Game Bible + Tech Spec

## FÁZE 2 — WORLD DESIGN

- ☐ Blockout mapy: les → ulice → modul lodi
- ☐ Definovat 4–6 POI a jejich quest napojení
- ☐ Additive scenes + streaming zón
- ☐ Spawn pointy nepřátel dle invasion %
- ☐ NavMesh pro AI
- ☐ **EMP zóny** kolem alien tech
- ☐ Základní osvětlení scény
- ☐ Validace pravidel z Game Bible v každém POI

## FÁZE 3 — NEPŘÁTELÉ, QUESTY, PROGRESSION

- ☐ 4 typy nepřátel (alien, war dog, **Puppet + Puppeteer**, hostile humans)
- ☐ **Puppet/Puppeteer link logic** (instant collapse)
- ☐ Ragdoll systém
- ☐ Destrukce prostředí
- ☐ Quest systém (tracking, multiple solutions, dopad)
- ☐ 3 questy V1 (design + skript + implementace)
- ☐ Inventory: hmotnostní + prostorový limit
- ☐ Loot systém (drop tabulky)
- ☐ Crafting: knowledge gate + materiálová podmínka
- ☐ Progression: skill tree (obě větve)
- ☐ Invasion systém: napojení na akce hráče + pasivní ticker

## FÁZE 4 — ART & UI

- ☐ Finální 3D modely prostředí (město, les, modul lodi)
- ☐ Finální modely postav (hrdina, alien, war dog, **Puppet, Puppeteer**, NPC)
- ☐ **Recon Drone 3D model**
- ☐ Animace pro každý typ postavy
- ☐ **Puppet collapse animace**
- ☐ VFX (výstřely, exploze, alien efekty, **EMP pulses**, **link visualization**)
- ☐ Zvuk (zbraně, prostředí, hudba)
- ☐ HUD: health, invasion %, alignment, inventory, quest log
- ☐ **GPS / Tactical Map UI finalizace** (mini + full map)
- ☐ UI transformace dle alignmentu
- ☐ HDRP osvětlení + post-processing (Volume profily)

## FÁZE 5 — PLAYTEST

- ☐ Interní playtest #1 (core loop)
- ☐ Interní playtest #2 (combat feel, lethality)
- ☐ Interní playtest #3 (invasion mechanika)
- ☐ Validace pravidel z Game Bible v hraní
- ☐ Playtest log zapsán
- ☐ Iterační sprint — kritické fixy
- ☐ Externí playtest (3-5 hráčů)
- ☐ Finální iterace

## FÁZE 6 — V1 RELEASE

- ☐ Code freeze
- ☐ Optimalizace výkonu (LOD, pooling, profiling)
- ☐ Release build pro PC
- ☐ Interní prezentace V1
- ☐ Rozhodnutí o V2 scope
- ☐ Update dokumentů (PRD, Game Bible, Tech Spec)
- ☐ Archivace buildu a dokumentace

---

# ČÁST 3 — SEZNAM ASSETS PRO UNITY

Tento seznam pokrývá všechny assety potřebné pro V1 vertical slice. **🆕 = nově přidáno v rozšíření v0.2.**

## LEGENDA PRIORIT

| **Kritické** | Bez toho nefunguje V1 |
|---|---|
| **Vysoká** | Potřeba před playtestem |
| **Střední** | Pro final pass |

---

## UNITY PACKAGE MANAGER

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Unity Package | HDRP (High Definition RP) | Nainstalovat jako první — základ celého projektu | **Kritické** |
| Unity Package | Input System (com.unity.inputsystem) | Vyžaduje pro greybox simulaci vstupu | **Kritické** |
| Unity Package | Cinemachine | TPS kamera, follow, aim assist | **Kritické** |
| Unity Package | Visual Effect Graph (VFX Graph) | HDRP particles | **Kritické** |
| Unity Package | TextMeshPro | Veškerý text ve hře | **Kritické** |
| Unity Package | ProBuilder | 3D blockout přímo v Unity | **Vysoká** |
| Unity Package | NavMesh Components (AI Navigation) | NavMesh pro 4 typy nepřátel | **Kritické** |
| Unity Package | Unity UI (uGUI) | HUD, inventory, quest log, **mini-mapa** | **Kritické** |
| Unity Package | Addressables | Streaming assetů — additive scenes | **Vysoká** |
| Unity Package | VFX Graph Samples | 30+ hotových HDRP efektů | **Vysoká** |
| **🆕 Unity Package** | **Universal Render Pipeline Decals** | EMP zóny, krev, alien stopy | **🆕 Vysoká** |

---

## 3D — PROSTŘEDÍ

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| 3D — Prostředí | Les: stromy, keře, kameny, tráva, cesta | Blockout postačí pro F1–F3 | **Kritické** |
| 3D — Prostředí | Město: budovy (fasády + interiér klíčových), ulice, chodník, auta | Modulární sada pro opakování | **Kritické** |
| 3D — Prostředí | Modul vesmírné lodi: exteriér + vstup | Unikátní asset, klíčový cíl V1 | **Kritické** |
| 3D — Prostředí | Destruktivní objekty: zeď, plot, bedna, auto | Potřeba pro destrukci prostředí | **Vysoká** |
| 3D — Prostředí | POI dekorace: stany, barikády, vraky, alien harampádí | Věrohodnost světa | **Střední** |
| 3D — Prostředí | HDRP materiály: tráva, asfalt, beton, kov, půda | Základní PBR materiály | **Kritické** |
| 3D — Prostředí | Decals: stopy, trhliny, krev, alien fluid | HDRP Decal Projector | **Vysoká** |
| 3D — Prostředí | Interiér modulu lodi: chodby, panely, terminály | Pro quest uvnitř modulu | **Vysoká** |
| 3D — Prostředí | Zbraně: pistole, puška, **kuše/luk**, alien energetická zbraň | Viditelné v ruce hráče v TPS pohledu | **Kritické** |
| 3D — Prostředí | Loot předměty: krystaly, součástky, lékárničky | Viditelné ve světě před sebráním | **Střední** |
| **🆕 3D — Prostředí** | **Recon Drone 3D model** (Tier 1) | Hráčův UAV — kvadrokoptéra | **🆕 Kritické** |
| **🆕 3D — Prostředí** | **Alien tech struktury** (věže, antény, EMP emittery) | Permanentní červené body na mapě | **🆕 Vysoká** |
| **🆕 3D — Prostředí** | **Vojenská základna** (lore — místo nálezu drona) | Storytelling element | **🆕 Střední** |

---

## 3D — POSTAVY

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| 3D — Postava | Hrdina: plný 3D model s rigem | Viditelný v TPS kameře | **Kritické** |
| 3D — Postava | Alien: plný model s rigem | 1 typ vizuálu, variace technologií | **Kritické** |
| 3D — Postava | Mimozemský War Dog: model s rigem | Čtyřnohý, agresivní tvar | **Kritické** |
| **🔄 3D — Postava** | **Puppet** (zotročený člověk) | ~~Drone~~ — **přejmenováno na Puppet**. Lidské tělo s alien implantáty (viditelné fluorescentní stopy) | **Kritické** |
| **🆕 3D — Postava** | **Puppeteer** (alien operator) | Vizuálně odlišitelný — ovládací zařízení/headpiece, antény, neagresivní postura | **🆕 Kritické** |
| 3D — Postava | NPC civilista: 2–3 varianty modelu | Placeholder pro F1–F3 | **Vysoká** |
| **🆕 3D — Postava** | **Hostile human (kolaborant)** | Civilní oblečení s improvizovanou výzbrojí | **🆕 Vysoká** |
| **🆕 3D — Postava** | **NPC odboj** (lidský vůdce, obchodník) | Pro questy, social hub | **🆕 Vysoká** |

---

## ANIMACE

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Animace | Hrdina: idle, chůze, sprint, krčení, střelba, melee, smrt | Mixamo nebo custom | **Kritické** |
| Animace | Hrdina: reload, aim, cover, **drone control pose** | Doplněk základního setu | **Vysoká** |
| Animace | **Hrdina: critical injury** (zpomalený, krčící se, kulhavý) | Vizuální feedback lethality | **🆕 Kritické** |
| Animace | **Hrdina: medkit application** (5s, vulnerability) | Healing animace | **🆕 Vysoká** |
| Animace | **Hrdina: alien augmentace vizuál** | Glow ruce, změna postury | **Střední** |
| Animace | Alien: idle, patrol, útok ranged, útok melee, smrt, flee | Ragdoll overlay přes death | **Kritické** |
| Animace | **Alien: cover behavior** (peeking, dodging) | V1 cover AI | **🆕 Vysoká** |
| Animace | War Dog: idle, sprint, útok, smrt | Čtyřnohý locomotion | **Kritické** |
| **🔄 Animace** | **Puppet:** idle, útok, **collapse animace (instant na smrt Puppeteera)** | Shutdown animace, ragdoll | **Kritické** |
| **🆕 Animace** | **Puppeteer: flee behavior** (utíká, schovává se) | Reaktivní AI animace | **🆕 Vysoká** |
| Animace | NPC: idle, strach, útěk | Minimální sada | **Vysoká** |
| Animace | Animator Controller setup per postava | Blend tree pro locomotion | **Kritické** |
| Animace | Zničení modulu lodi: destrukční sekvence | Finální cíl V1 | **Kritické** |
| **🆕 Animace** | **Recon Drone:** start, létání, otáčení, sestřelení (pád) | Pro hráčův UAV | **🆕 Kritické** |

---

## VFX — VIZUÁLNÍ EFEKTY

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| VFX | Výstřel: nábojnice, záblesk, kouř | Per typ zbraně | **Kritické** |
| VFX | Zásah: krev/alien fluid, jiskry, střepiny | Dle materiálu | **Kritické** |
| VFX | Exploze: obecná + alien varianta | Pro granáty a destrukci | **Vysoká** |
| VFX | Destrukce objektu: prach, debris, zlomení | Napojeno na destroy trigger | **Vysoká** |
| VFX | Invasion vizuál: alien záře, hologramy, energie | Ambient VFX pro modul a výsadek | **Střední** |
| VFX | Crafting UI efekt: animace výroby/upgradu | Drobný feedback | **Střední** |
| VFX | Ragdoll aktivace | Řešeno Unity ragdoll | **Kritické** |
| VFX | HDRP Volume profily: den/noc, invasion mood, interiér lodi | Post-processing per zóna | **Kritické** |
| VFX | Volumetrická mlha (Fog Volume) | HDRP nativní | **Vysoká** |
| VFX | Alien augmentace efekt: pulsující implantáty | Vizuální feedback alien path | **Střední** |
| VFX | Spawn efekt nepřátel: teleport/materialization | Invasion event vizuál | **Střední** |
| **🆕 VFX** | **EMP pulse efekt** (kolem alien tech) | Vizuálně viditelná zóna — fialovo-modrá pulsace | **🆕 Vysoká** |
| **🆕 VFX** | **Puppet implant glow** (na těle Puppeta) | Fluorescentní stopy podél implantátů | **🆕 Vysoká** |
| **🆕 VFX** | **Puppet collapse efekt** (smrt Puppeteera) | Implant deactivation, fade-out | **🆕 Kritické** |
| **🆕 VFX** | **Puppeteer-Puppet link** (energetické pulzy) | Lehce viditelné vlákno mezi nimi | **🆕 Střední** |
| **🆕 VFX** | **Recon Drone propeller motion** | Particle blur efekt | **🆕 Střední** |
| **🆕 VFX** | **Drone shot down efekt** | Jiskry, kouř, pád | **🆕 Vysoká** |
| **🆕 VFX** | **Critical injury vision** (red border pulsace, blur) | Hráčův HUD při nízkém HP | **🆕 Kritické** |
| **🆕 VFX** | **Bleeding stopa na zemi** | Krev za hráčem při critical | **🆕 Vysoká** |

---

## ZVUK — SFX

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Zvuk — SFX | Zbraně: výstřel, přebíjení, prázdný zásobník (lidská zbraň) | WAV/OGG, 3D zvuk | **Kritické** |
| Zvuk — SFX | Zbraně: alien energetická zbraň | Sci-fi tón | **Kritické** |
| **🆕 Zvuk — SFX** | **Crossbow / Luk:** výstřel, natažení, dopad šípu | Tichý — klíčový pro stealth | **🆕 Kritické** |
| Zvuk — SFX | Kroky: dle povrchu (tráva, beton, kov, voda, listí) | Footstep systém s surface detection | **Vysoká** |
| Zvuk — SFX | Nepřátelé: zvuky útoku, bolesti, detekce, smrt | Per typ nepřítele | **Vysoká** |
| Zvuk — SFX | Prostředí: vítr v lese, ruch města, hum modulu lodi | Ambient loop | **Střední** |
| Zvuk — SFX | UI zvuky: klik, quest splněn, inventory, save | Drobné ale důležité | **Střední** |
| Zvuk — SFX | Destrukce prostředí: lámání, prach, řinčení kovů | Per typ materiálu | **Vysoká** |
| Zvuk — SFX | Ragdoll dopad: tělo padá na různé povrchy | Fyzický feedback | **Vysoká** |
| Zvuk — SFX | Dialog: hlas hrdiny + NPC | Placeholder lepší než ticho | **Střední** |
| **🆕 Zvuk — SFX** | **Recon Drone:** start, hum při létání, sestřelení** | UAV ambient sounds | **🆕 Kritické** |
| **🆕 Zvuk — SFX** | **EMP pulse:** elektromagnetický bzučák | Audio cue pro EMP zónu | **🆕 Vysoká** |
| **🆕 Zvuk — SFX** | **Puppet:** zotročené dýchání, mechanické pohyby | Atmosférický horror element | **🆕 Vysoká** |
| **🆕 Zvuk — SFX** | **Puppet collapse:** elektrický shutdown, pád | Smrt Puppeta | **🆕 Vysoká** |
| **🆕 Zvuk — SFX** | **Heartbeat při critical injury** | Vnitřní HUD audio | **🆕 Kritické** |
| **🆕 Zvuk — SFX** | **Alien sense bzučení** (alien-aligned hráč) | Vnitřní audio cue | **🆕 Střední** |
| **🆕 Zvuk — SFX** | **Map ping** (heatmap update) | UI feedback při scoutingu | **🆕 Střední** |

---

## ZVUK — HUDBA

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Zvuk — Hudba | Combat hudba: napjatý rytmický loop | Adaptive audio | **Vysoká** |
| Zvuk — Hudba | Ambient hudba: temný, atmosférický | Smyčka pro průzkum | **Střední** |
| Zvuk — Hudba | Victory/fail moment: krátký stinger | Konec V1 sekvence | **Střední** |
| **🆕 Zvuk — Hudba** | **Stealth tension hudba** (low pulse) | Při scoutingu, plížení | **🆕 Vysoká** |
| **🆕 Zvuk — Hudba** | **Critical injury hudba** (high tension) | Při nízkém HP, časový tlak | **🆕 Vysoká** |

---

## UI / UX

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| UI / UX | HUD: health bar, invasion %, alignment indikátor | Dynamicky mění styl dle alignmentu | **Kritické** |
| UI / UX | Inventory screen: mřížka slotů + hmotnostní bar | Přístupné z pause menu | **Kritické** |
| UI / UX | Quest log: aktivní questy + cíle | Jednoduchý list | **Kritické** |
| UI / UX | Crafting menu: seznam receptů + podmínky | Knowledge gate UI | **Vysoká** |
| UI / UX | Skill tree screen: obě větve (lidská / alien) | Vizuálně odlišné větve | **Vysoká** |
| UI / UX | Pause menu: resume, save, load, quit | Standardní | **Kritické** |
| UI / UX | Ikony: zbraně, loot typy, quest markery | Jednoduchá piktogramová sada | **Vysoká** |
| UI / UX | Font: sci-fi nebo minimalistický bezpatkový | Konzistentní s tónem hry | **Střední** |
| UI / UX | Dialog UI: text box, jméno mluvčího, portrait | NPC interakce | **Kritické** |
| **🔄 UI / UX** | **Mini-mapa s heatmap** (top-right corner) | **Dynamic GPS map s barevným kódováním** | **Kritické** |
| **🔄 UI / UX** | **Full Tactical Map** (Tab/M) | **5-vrstvý systém: Hrozby, Tech, Heatmap, Visibility, Terén** | **Kritické** |
| **🆕 UI / UX** | **GPS šipka k cíli** (modrá, jako auto GPS) | Diegetický UX | **🆕 Kritické** |
| **🆕 UI / UX** | **Heatmap legend** (vysvětlení 5 barev) | Tutorial element | **🆕 Vysoká** |
| **🆕 UI / UX** | **Drone control UI** (battery bar, range indicator, return button) | Při ovládání dronu | **🆕 Kritické** |
| **🆕 UI / UX** | **Drone camera view** (full-screen overlay) | Při ovládání dronu | **🆕 Kritické** |
| UI / UX | Damage number popup (floating text) | Feedback zásahu | **Střední** |
| UI / UX | Notifikace / toast system | Quest update, loot pickup | **Střední** |
| UI / UX | Crosshair system: různý dle zbraně | TPS shooter nutnost | **Vysoká** |
| UI / UX | Loading screen / scene transition | Additive scene streaming | **Vysoká** |
| **🆕 UI / UX** | **Critical injury HUD** (red border, heartbeat, timer) | Lethality feedback | **🆕 Kritické** |
| **🆕 UI / UX** | **Bleeding indicator** (HP bar pulsuje) | Visual urgency | **🆕 Vysoká** |
| **🆕 UI / UX** | **Alignment shift indicator** (po volbě, animace) | Feedback při morálních volbách | **🆕 Vysoká** |

---

## UNITY SCRIPT / SYSTÉM

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Unity Script | Scriptable Objects: nepřátelé, loot, questy, recepty | Data-driven základ | **Kritické** |
| Unity Script | NavMesh Agent konfigurace per typ nepřítele | Různé rychlosti, poloměry | **Kritické** |
| Unity Script | Ragdoll prefab setup | Per model postavy | **Kritické** |
| Unity Script | Destruction prefab: před/po zničení + debris | Per destruktivní objekt | **Vysoká** |
| Unity Script | Adaptive music manager: detekce combat stavu | Jednoduchý state machine | **Střední** |
| Unity Script | Invasion event trigger: spawnování dle invasion % | Např. fotbalový stadion event | **Vysoká** |
| Unity Script | Object Pool Manager | Spawn/despawn bez GC spike | **Kritické** |
| Unity Script | Footstep Manager: surface detection → audio | Raycast pod hráče | **Vysoká** |
| Unity Script | Dialog systém: trigger → NPC linka → response volby | I jednoduchý systém pro V1 | **Kritické** |
| Unity Script | Audio Mixer setup: Master / SFX / Music / Voice | Settings menu | **Vysoká** |
| Unity Script | LOD Group konfigurace per asset | Výkon | **Kritické** |
| Unity Script | Scene streaming controller (Additive Scenes) | Load/unload regionů | **Vysoká** |
| Unity Script | Camera shake systém | Exploze, zásah | **Střední** |
| Unity Script | Interakční systém (E klávesa): loot, NPC, terminál | Proximity trigger + UI | **Kritické** |
| Unity Script | Alignment systém (Human/Hybrid/Alien tracker) | Backend pro invasion + UI | **Kritické** |
| Unity Script | Game Event Bus (ScriptableObject events) | Decoupled komunikace | **Vysoká** |
| **🆕 Unity Script** | **Body Parts Damage System** | Multi-hitbox damage podle Tech Spec | **🆕 Kritické** |
| **🆕 Unity Script** | **Critical Injury State Manager** | Bleeding, slow movement, aim drift | **🆕 Kritické** |
| **🆕 Unity Script** | **Recon Drone Controller** | Camera switch, battery, range, EMP detection | **🆕 Kritické** |
| **🆕 Unity Script** | **Map Data System** (heatmap data points + decay) | Persistent intelligence | **🆕 Kritické** |
| **🆕 Unity Script** | **GPS / Tactical Map UI Controller** | Mini + full map rendering | **🆕 Kritické** |
| **🆕 Unity Script** | **EMP Zone Trigger** (sférický collider) | Drone failure logic | **🆕 Kritické** |
| **🆕 Unity Script** | **Sound Emitter System** | Per-povrch zvuk + alien response | **🆕 Vysoká** |
| **🆕 Unity Script** | **Puppet/Puppeteer Link Manager** | Instant collapse logic | **🆕 Kritické** |
| **🆕 Unity Script** | **Cover Detection AI** (V1 vrstva) | Aliení používají kryt | **🆕 Vysoká** |
| **🆕 Unity Script** | **Heatmap Decay Timer** | Auto-fade barev podle Tech Spec | **🆕 Vysoká** |

---

Siege of the Blue World — Workflow v0.2 — V1 Vertical Slice

*Aktualizovat při každé změně scope nebo potvrzeném TBD.*

*Související dokumenty: PRD (specifikace), Game Bible (pravidla), Tech Spec (mechaniky).*
