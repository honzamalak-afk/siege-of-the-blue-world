**SIEGE OF THE BLUE WORLD**

QUEST DESIGN DOCUMENT — V1 Questy

Verze: 0.1 (kostra)

*Living document — vyplňuje se postupně, validuje greyboxem.*

---

## ÚVOD K DOKUMENTU

### Co tento dokument je

**Quest Design Document** obsahuje **konkrétní implementační plán** pro questy hry — story beats, NPC dialogy, lokace, mechanická řešení, decision points, outcomes.

### Co tento dokument NENÍ

- ❌ Lore (→ Lore Bible)
- ❌ Mechaniky (→ PRD / Game Bible / Tech Spec)
- ❌ Vizuální design (→ Art Direction)
- ❌ Production timeline (→ Production Roadmap)

### Principy quest designu (z naší diskuse)

#### Princip 1: Cíle dány, cesta NE
Quest říká **CO udělat**, ne **JAK**. Hráč si vybírá přístup podle pravidel světa.

#### Princip 2: Rules-based multiple solutions
Každý quest má **alespoň 3 různé cesty** k cíli, vznikající z hráčových rozhodnutí.

#### Princip 3: Encountery jsou dynamické
Mix combat / non-combat encounterů je **systemic**, ne fixní. Designér dává **building blocks**, ne fixed sequences.

#### Princip 4: Morální váha
Klíčové rozhodnutí v každém questu má **dlouhodobý dopad** — alignment, world state, NPC reactions.

#### Princip 5: Information first
Hráč dostává **dost informací** pro rozhodnutí (NPC, lore, scout). Žádná "gotcha" rozhodnutí.

---

# QUEST TEMPLATE (struktura každého questu)

```
1. METADATA
   - Quest ID
   - Verze
   - Status
   - Estimated playtime
   - Encounter mix (combat/non-combat ratio)

2. STORY OVERVIEW
   - Quest hook (jak začíná)
   - Main premise
   - Stakes (co je v sázce)
   - Story arc (3-5 beat structure)

3. PARTICIPANTS (NPCs)
   - Quest giver
   - Key NPCs (motivace, role)
   - Antagonists
   - Reference Lore Bible

4. LOCATIONS
   - Primary lokace (s mapou)
   - Secondary lokace
   - Environmental storytelling notes

5. INFORMATION FLOW
   - Co hráč ví na začátku
   - Co se musí dozvědět
   - Sources of information (NPC, lore, scout)

6. POSSIBLE PATHS (multiple solutions)
   - Path A (typicky stealth)
   - Path B (typicky combat)
   - Path C (typicky social/manipulation)
   - Path D+ (kreativní emergent)

7. DECISION POINTS
   - Klíčové volby
   - Důsledky každé volby
   - Alignment shifts

8. OUTCOMES
   - Best case
   - Mid cases
   - Worst case
   - Failure states (pokud možné)

9. REWARDS
   - Loot (specifické items)
   - XP / skill progression
   - Alignment shifts
   - Story progression
   - Faction reputation

10. WORLD STATE CHANGES
    - Co se v světě mění po questu
    - Future quest unlocks/locks
    - NPC dialog changes

11. VALIDATION TESTS
    - Otázky pro greybox playtest
    - Co měřit
    - Acceptance criteria

12. DESIGN NOTES
    - Otevřené otázky [TBD]
    - Inspirace
    - Risk mitigations
```

---

# QUEST 1: [PLACEHOLDER NÁZEV]

## 1. METADATA

```
Quest ID: Q1
Verze: 0.1 (kostra)
Status: ⚠ Designing
Estimated playtime: 30-45 min
Encounter mix: ~30% combat, ~70% non-combat
Target greybox: Yes
```

## 2. STORY OVERVIEW

### Quest Hook

**[TBD] Klíčové otázky:**
- Jak hráč zjistí, že má jít do města?
- Je to vnitřní motivace (něco, co chce hrdina) nebo vnější (NPC posílá)?
- Jaký je první moment hry?

**Návrh směru — "Cesta domů":**

> Hra začíná tím, že hrdina se vrací z **expedice / pracovního výletu** do svého rodného města. Na cestě **slyší rádio** — alien occupation, evacuace nedávno proběhla. Rodina/přátelé jsou někde ve městě. **Hrdina jde tam zjistit**, co se stalo. Cestou potkává **stará vojenská základna** v lese (= Quest 1 hub).

### Main Premise

**[TBD]:**

> Hrdina se musí dostat do města, najít **někoho z rodiny / blízkého** (motivace pro questy 2-3), a po cestě objevit, že **modul lodi v centru** je centrum problému.

### Stakes

**[TBD]:**

> Osobní stakes (najít blízkou osobu) + objevení většího plánu (modul jako klíč).

### Story Arc (5 beats)

**Beat 1: Setup (Forest entry)**
- Hráč začíná s minimální výbavou
- Učí se ovládání skrze pasivní pravidla
- Objevuje vojenskou základnu (drone reward)

**Beat 2: Inciting event (First combat)**
- První alien encounter v lese
- Hráč se učí lethality combat
- Objevuje Puppet body (lore moment)

**Beat 3: Rising action (City entry)**
- Hráč dorazí na okraj města
- Setká se s NPC odboje (prv hub interaction)
- Dostane info o modulu

**Beat 4: Climax (Module recon)**
- Hráč pošle drone k modulu
- Objevuje EMP zónu, Puppeteer patroly
- Klíčové rozhodnutí: jak útočit?

**Beat 5: Resolution (Quest 1 end)**
- Quest 1 končí v hub města
- Setup pro Quest 2
- World state shift (lokální invasion lehce klesne)

[Rozpracovat]

## 3. PARTICIPANTS

### Quest Giver

**[TBD]:**
- Není explicitní quest giver — hra začíná self-driven
- NEBO: NPC v lese (hráč potkává v základně) dá "kontext"

### Key NPCs

**Forest segment:**
- [TBD] NPC v opuštěné základně? Nebo prázdná?

**City entry segment:**
- **NPC odboje (lokální vůdce)** — viz Lore Bible 4.2
- **Civilisté** — 2-3 placeholder, dávají info

**Antagonists:**
- 2-3 patrolující alieni v lese
- 1 alien war dog (volitelný encounter)
- 1 Puppet + Puppeteer skupina (training encounter)
- Aliens ve městě (dle invasion %)

[Vyplnit konkrétními jmény z Lore Bible]

## 4. LOCATIONS

### Primary

**Forest** (start, ~200m corridor):
- Stará lesní cesta
- Opuštěná chata (start point)
- Vojenská základna (drone reward)
- Klíčový landmark: padlý vrak (lore)

**City Edge**:
- Brána do města (alien checkpoint)
- Underground vstup do města (alternativa)

**City Hub**:
- Underground rezistanční base
- Civilní zóna (3-5 NPCs)
- View na modul (objective marker)

### Secondary

- Pre-invasion bunkr (loot opportunity)
- Vojenský konvoj (alternative loot)
- Crashed alien scout pod (lore + alien materials)

### Environmental Storytelling

**[TBD] Klíčové momenty:**
- Krvavá stopa zotročeného člověka v lese (telegraphing rule "krev = stopa")
- Zničený checkpoint (ukazuje failed assault NPC)
- Graffiti odboje (text v lore: "remember [datum]")
- Mrtvý alien voják v ruinách (loot + lore)

[Rozpracovat]

## 5. INFORMATION FLOW

### Co hráč ví na začátku

- Aliení invaze proběhla
- Rodné město je obsazeno
- Hledá konkrétní osobu (rodinu/přítele)

### Co se musí dozvědět

- Jak fungují alien Puppety (z lore deníku)
- Že modul je centrum lokální kontroly (z NPC odboje)
- Jak používat drone (z vojenské základny)
- Pravidla přežití (15 V1 pravidel, postupně)

### Sources of Information

| Info | Source |
|---|---|
| Alien Puppety | Mrtvý Puppet v lese + deník |
| Modul lodi | NPC odboje |
| Drone usage | Vojenská základna manuál |
| Pravidla přežití | Environmental, NPC dialog, gameplay |

[Rozpracovat]

## 6. POSSIBLE PATHS (Multiple Solutions)

### Path A — Stealth-focused

**Strategie:** Vyhnout se většině boje, použít drone scout aktivně.

```
Forest → Stealth around alien patrol → 
Find military base early → Drone equipped →
Recon entire city before entering →
Use civilian intel for safe path →
Quest 1 ends with minimal combat
```

**Reward:** Lehčí Quest 2, stealth skill XP, alignment +5 human (no kills)

### Path B — Combat-focused

**Strategie:** Eliminovat hrozby, vyčistit terén.

```
Forest → Engage alien patrol → 
Take loot from kills → Combat XP →
Approach city loud →
Establish dominance with NPCs (intimidation) →
Quest 1 ends with cleared region
```

**Reward:** Combat XP, more loot, alignment 0 (neutral kills)

### Path C — Social/Manipulation

**Strategie:** Využít NPCs, najít alternative routes přes informace.

```
Forest → Avoid combat, focus on lore →
Find military base, learn drone →
Reach city via underground (NPC tip) →
Build relationships with rezistance →
Quest 1 ends with strong NPC alliances
```

**Reward:** Faction reputation +20 odboj, lehčí social Q2

### Path D — Aggressive (alien-aligned start)

**Strategie:** Zranit Puppeta, vzít implantát.

```
Forest → Kill Puppet, take alien implant →
Alignment shift to alien (early) →
NPC odboje suspicious of player →
Use alien sense, but lose human ally help →
Quest 1 ends with alien-aligned setup
```

**Reward:** Alien sense unlocked, alignment -10 alien, NPC distrust

[Rozpracovat — toto je core multiple solutions]

## 7. DECISION POINTS

### Decision 1: Alien implant z mrtvého Puppeta

**Kdy:** Po prvním combat encounter v lese

**Volby:**
- A) **Zničit implant** (+5 human alignment, zničí možnost alien sense)
- B) **Vzít implant** (-5 alien alignment, unlock alien sense Tier 1)
- C) **Ignorovat** (neutral)

**Důsledky:** Klíčové pro path A/D split.

### Decision 2: Pomoct civilistovi nebo prošvihnout

**Kdy:** Při vstupu do města — civilista je obklopen aliény

**Volby:**
- A) **Zaútočit a zachránit** (+10 odboj rep, riziko)
- B) **Plížit se kolem** (neutral)
- C) **Použít distrakci pro civilního útěk** (+5 odboj rep, low risk)
- D) **Ignorovat** (-5 odboj rep)

### Decision 3: Důvěra k NPC odboje

**Kdy:** První interakce s vůdcem odboje

**Volby:**
- A) **Plně důvěřovat** (rychlý quest progress)
- B) **Skeptický** (více info, pomalejší)
- C) **Ostře odmítnout** (NPC distrust)

[Rozpracovat]

## 8. OUTCOMES

### Best Case (path A nebo C)

- Quest 1 completed
- Strong NPC odboj alliance
- Drone equipped
- Lokální invasion -3%
- Setup pro Quest 2 (mission to Module)

### Mid Cases (path B)

- Quest 1 completed via combat
- Neutral NPC relations
- Combat XP
- Lokální invasion -2% (less than stealth)

### Worst Case (path D nebo failure)

- Quest 1 completed via alien path
- Hostile NPC odboj
- Alien sense unlocked
- Lokální invasion no change
- Setup pro alien-aligned Quest 2

### Failure State

**Lze quest selhat?** [TBD]
- Návrh: Ne — quest pokračuje, ale různými cestami
- Hráč nemůže být **definitivně blocked**

[Rozpracovat]

## 9. REWARDS

| Path | Loot | XP | Alignment | Faction Rep |
|---|---|---|---|---|
| A (Stealth) | Drone, lore | +200 | +5 Human | +10 odboj |
| B (Combat) | Loot from kills, weapons | +250 | 0 | 0 |
| C (Social) | Drone, NPC contacts | +200 | +5 Human | +20 odboj |
| D (Alien) | Drone, alien implant | +200 | -10 Alien | -10 odboj |

[Konkretizovat]

## 10. WORLD STATE CHANGES

### Po Quest 1 completion

- Lokální invasion adjusts (per path)
- NPC odboj base "objevena" pro hráče
- Modul je nyní "known threat" v UI
- Některé nové dialogy odemknuté
- Quest 2 starts available

### Long-term effects

- Pokud zachránil civilistu: civilista se objeví ve městě jako side NPC
- Pokud použil implant: alien sense permanent
- Pokud pomohl odboji: future quest přístup

[Rozpracovat]

## 11. VALIDATION TESTS

### Greybox playtest otázky

1. Pochopil hráč quest goal bez tutorial?
2. Cítil hráč multiple paths jako legitimní?
3. Cítil hráč váhu rozhodnutí (decision points)?
4. Byl combat zaslouženě obtížný?
5. Funguje encounter mix (combat / non-combat)?

### Acceptance criteria

✅ **Pass:** Subjective rating 7+/10 napříč otázkami
✅ **Pass:** Aspoň 50% playtesterů zvolilo různé paths
✅ **Pass:** Žádný path nebyl "dominant strategy"
✅ **Pass:** Hráč pochopil 60%+ pravidel přežití během questu

❌ **Fail (k iteraci):** 
- Někteří hráči stuck (nelze pokračovat)
- Jeden path dominantní (>70% volí)
- Combat frustrující (>3 deaths v jednom encounteru)

## 12. DESIGN NOTES

### Otevřené otázky [TBD]

- Konkrétní jméno hrdiny + jeho hledané osoby?
- Konkrétní jméno vůdce odboje?
- Specifická lokace V1 regionu (geograficky)?
- Pre-invasion historie regionu?

### Inspirace

- **STALKER:** Atmosféra forestu, environmental storytelling
- **The Last of Us:** První hodina hry pacing
- **Half-Life 2:** Slow start s discovery
- **Dishonored:** Multiple solutions design

### Risk Mitigations

- **Risk:** První quest je příliš náročný → frustrace
  - **Mitigation:** Lethality "soft" v Quest 1, escalates v Q2-Q3

- **Risk:** Multiple paths nefungují (jeden je dominant)
  - **Mitigation:** Greybox playtest zachycuje, iteruj balance

- **Risk:** Lore overload v první hodině
  - **Mitigation:** Lore je optional (deníky, environmental), ne required

---

# QUEST 2: [PLACEHOLDER NÁZEV — "Modul Reconnaissance"]

## 1. METADATA

```
Quest ID: Q2
Verze: 0.1 (kostra — preliminary)
Status: ⚠ Designing
Estimated playtime: 30-45 min
Encounter mix: ~40% combat, ~60% non-combat
```

## 2. STORY OVERVIEW

### Quest Hook

**[TBD]:**

> Po Quest 1 hráč ví o modulu. Vůdce odboje (nebo hrdinova hledaná osoba) navrhne **scouting mission**. Cíl: získat **dostatek info pro útok** (Quest 3).

### Main Premise

> Hráč musí **proniknout do alien zóny**, najít **slabinu modulu** (energetické jádro? Puppeteer hierarchy? EMP weakpoint?), a vrátit se s plánem.

### Story Arc

[TBD]

## 3-12. (Stejná struktura jako Quest 1)

[Vyplnit dle template]

---

# QUEST 3: [PLACEHOLDER NÁZEV — "Module Assault"]

## 1. METADATA

```
Quest ID: Q3
Verze: 0.1 (kostra — preliminary)
Status: ⚠ Designing
Estimated playtime: 30-45 min
Encounter mix: ~50% combat, ~50% non-combat (climax)
```

## 2. STORY OVERVIEW

### Quest Hook

**[TBD]:**

> Hráč má plán z Q2. Čas zaútočit. **Final mission V1 vertical slice.**

### Main Premise

> **Multiple approaches** k modulu (z Q2 reconnaissance):
> - Stealth + sabotage jádra
> - Loud assault s útoky odboje
> - Manipulation (alien-aligned hráč)
> - Combo strategie

### Story Arc

[TBD]

## 3-12. (Stejná struktura jako Quest 1)

[Vyplnit dle template]

---

# CROSS-QUEST CONNECTIONS

## State Carry Between Quests

| Z Quest 1 | → Quest 2 | → Quest 3 |
|---|---|---|
| Drone equipped | Drone usage essential | Drone optional (EMP zone) |
| Alignment shift | Affects NPC reactions | Affects approach options |
| Faction rep | Affects available info | Affects available alies |
| Survival skills learned | Pravidla 1-8 | Pravidla 9-15 + integration |

## Persistent World State

**Po Quest 1:**
- Forest "cleared" (less aliens)
- City entry zone known
- NPC odboj first contact

**Po Quest 2:**
- Module weakness identified
- Alien response begins (escalation)
- Specific approach unlocked

**Po Quest 3:**
- Module destroyed (VICTORY for V1)
- Liberation of region
- Setup for V2 expansion

---

# ENCOUNTER LIBRARY (V1)

> **Pozn.:** Tyto encountery jsou **building blocks**, které designér může použít napříč questy. NEJSOU fixed sequence.

## Combat Encounter Types (V1)

| Type | Description | Risk Level | Reward |
|---|---|---|---|
| Solo Alien Patrol | 1 alien chodí po cestě | Low | Basic loot |
| Alien Pair | 2 alieni se kryjí | Medium | Mixed loot |
| Puppet Squad | 1 Puppeteer + 2-3 Puppety | High | Alien materials |
| War Dog Ambush | 1-2 War Dogs ze stromu | Medium-High | Combat XP |
| Alien Stronghold | 3-5 alienů u POI | Very High | Best loot |
| Hostile Human Camp | 2-4 kolaboranti | Medium | Lore + loot |

## Non-Combat Encounter Types (V1)

| Type | Description | Outcome |
|---|---|---|
| Lore Discovery | Deník, dokument | Story info |
| Civilian in Need | Civilista žádá pomoc | Faction rep + alignment |
| Hidden Cache | Skrytý loot (puzzle) | Equipment |
| NPC Dialog | Trader, info giver | Quest hooks |
| Environmental Mystery | Co se tady stalo? | Lore + maybe loot |
| Stealth Pass | Slip through alien patrol | XP, no combat |

[Rozpracovat per quest]

---

# CO JEŠTĚ CHYBÍ (priority pro v0.2)

## 🔴 Kritické pro greybox start

- Jméno hrdiny + hledané osoby
- Konkrétní jména klíčových NPC
- Detail Quest 1 paths (mechanically)
- Quest 1 dialog drafts

## 🟡 Důležité po greyboxu

- Quest 2 + Quest 3 detail
- Encounter library expansion
- Side quest framework

## 🟢 Žádoucí pro V1 release

- 5-10 side questů V1
- Easter eggs
- Hidden content
- Replayability hooks

---

# DESIGN NOTES (autoři)

## Při designu questu si klást otázky

✅ Splňuje **rules-based principle** (cíl dán, cesta NE)?
✅ Má **alespoň 3 path options**?
✅ Má **klíčový morální moment**?
✅ Funguje s **encounter mix** (combat + non-combat)?
✅ Dává **dostatek informací** hráči pro rozhodnutí?
✅ Má **dlouhodobý dopad** (world state)?
✅ Lze testovat v **greyboxu**?

## Anti-patterns (vyhnout se)

❌ "Go to X, kill Y, return" (lineární)
❌ "Only one way to win" (rules-based fail)
❌ "Decision without consequences" (designer rouz)
❌ "Tutorial popups everywhere" (anti-rules-based)
❌ "Boss fight required" (PRD říká NO bosses)

## Validation principy

Quest je hotový, pokud:
1. Lze splnit alespoň 3 různými cestami
2. Každá cesta má unikátní reward
3. Decision points cítí váhu
4. World state se viditelně mění
5. Greybox playtest projde acceptance criteria

---

Quest Design Doc v0.1 — Siege of the Blue World

*Aktualizovat při každém designovém milníku.*

*Související dokumenty: Lore Bible (postavy, svět), PRD (mechaniky), Game Bible (pravidla).*
