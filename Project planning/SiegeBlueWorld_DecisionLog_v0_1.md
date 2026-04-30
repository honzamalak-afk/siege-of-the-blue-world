**SIEGE OF THE BLUE WORLD**

DECISION LOG — Klíčová rozhodnutí

Verze: 0.1

*Living document — uzavírá se postupně. Každé uzavřené rozhodnutí přesouváme do PRD/Game Bible/Tech Spec.*

---

## ÚVOD

Tento dokument obsahuje **rozhodnutí, která blokují další design**. Jsou seřazeny podle priority — odshora nejdůležitější (odblokuje nejvíc další práce).

**Princip:** Každá otázka má **4 možnosti** + **5. možnost = vlastní odpověď**. Pokud se ti žádná z nabídek nelíbí, doplň vlastní.

**Workflow:**
1. Projít otázku
2. Vybrat odpověď (1-4 nebo vlastní)
3. Zapsat **rozhodnutí + zdůvodnění**
4. Po uzavření přesunout do příslušného dokumentu (PRD/Game Bible/Tech Spec)

---

## STAV ROZHODNUTÍ

| # | Otázka | Status | Cílový dokument |
|---|---|---|---|
| 1 | Velikost a forma týmu | ⚠ Otevřené | PRD sekce 0 |
| 2 | Cílový hardware a engine version | ⚠ Otevřené | PRD sekce 0, 10 |
| 3 | Multiplayer architektura | ⚠ Otevřené | PRD sekce 10 |
| 4 | Damage thresholds (kdy critical injury) | ⚠ Otevřené | PRD 5, Tech Spec 1 |
| 5 | Drone scope V1 (které features) | ⚠ Otevřené | PRD 6.4, Tech Spec 2 |
| 6 | Alignment exclusivity rules | ⚠ Otevřené | PRD 6, Game Bible |
| 7 | Save system mechanika | ⚠ Otevřené | PRD 10, Tech Spec |
| 8 | Difficulty options strategy | ⚠ Otevřené | PRD 11 |
| 9 | Tutorial style | ⚠ Otevřené | PRD 11, Game Bible |
| 10 | V1 deadline a milestone strategie | ⚠ Otevřené | PRD 12, Production Roadmap |

---

# OTÁZKA 1: VELIKOST A FORMA TÝMU

**Proč to blokuje další design:** Bez znalosti týmu nelze určit deadline, scope V1, ani roli AI tooling. Velikost týmu fundamentálně ovlivňuje, **co je realistické vytvořit**.

## Možnosti

### A) Solo dev + AI nástroje
- 1 člověk (ty) + Claude Code, Cursor, AI assets
- **V1 timeline:** 12-18 měsíců
- **Risk:** burnout, omezený scope
- **Výhoda:** plná kontrola, žádná koordinace

### B) Mikro-tým 2-3 lidi
- 1 programátor, 1 designer, 1 artist (může být freelance)
- **V1 timeline:** 9-12 měsíců
- **Risk:** komunikační overhead, méně specializace
- **Výhoda:** sdílení práce, rychlejší než solo

### C) Indie tým 4-6 lidí
- Programmer × 2, Designer, Artist × 2, audio/QA
- **V1 timeline:** 6-9 měsíců
- **Risk:** financování, koordinace
- **Výhoda:** plná specializace, profesionální výsledek

### D) Hybrid: Core tým 2-3 + outsourcing
- 2-3 core lidé (programmer, designer) + outsourcing pro art/audio
- **V1 timeline:** 9-12 měsíců
- **Risk:** kvalita outsourcu, integration
- **Výhoda:** flexibilita, kontrola nákladů

### E) Vlastní odpověď
*Doplň vlastní variantu, pokud žádná nesedí.*

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Jména/role v týmu:


Kdy potřeba potvrdit (deadline rozhodnutí):
```

---

# OTÁZKA 2: CÍLOVÝ HARDWARE A ENGINE VERZE

**Proč to blokuje další design:** HDRP je výpočetně náročný. Bez známého minimálního hardware nelze plánovat optimalizace, asset budgety, ani vizuální cíle.

## Možnosti

### A) High-end target (RTX 3060+ / RX 6700+)
- 16 GB RAM, SSD povinné
- Cíl: krásná HDRP grafika, plné effects
- **Trade-off:** menší trh hráčů, ale vizuálně nejlepší
- **Engine:** Unity 6 (latest)

### B) Mid-range target (GTX 1660 / RTX 2060 / RX 5600)
- 16 GB RAM
- Cíl: HDRP s rozumnou kvalitou, optimalizováno
- **Trade-off:** kompromis kvality, ale širší trh
- **Engine:** Unity 6 nebo Unity 2022 LTS

### C) Wide target (GTX 1060 / RX 580)
- 8 GB RAM minimum
- Cíl: V1 funguje na 5-7 let starém HW
- **Trade-off:** nutnost downscale HDRP, možný switch na URP
- **Engine:** Unity 2022 LTS (stabilita)

### D) Modern enthusiast only (RTX 4070+)
- 32 GB RAM, NVMe SSD
- Cíl: AAA quality bez kompromisů
- **Trade-off:** velmi malý trh, ale showcase quality
- **Engine:** Unity 6 + DLSS/FSR

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Engine version: 
Min. spec (CPU/GPU/RAM):
Recommended spec:
Storage požadavek:
```

---

# OTÁZKA 3: MULTIPLAYER ARCHITEKTURA

**Proč to blokuje další design:** Multiplayer rozhodnutí ovlivňuje **celou architekturu kódu** (state management, input handling, save system). Pozdější přidání = drahá refaktorizace.

## Možnosti

### A) Singleplayer only — žádné MP příprava
- Architektura optimalizovaná čistě pro SP
- Maximální využití SP-specific features
- **Trade-off:** přidání MP později = velký refaktor
- **Vhodné pokud:** MP nikdy nebude

### B) Singleplayer + passive MP-ready
- Architektura odděluje state logiku od renderingu
- Žádný actual MP code, ale clean architecture
- **Trade-off:** mírný overhead v SP
- **Vhodné pokud:** MP je možný v budoucnu

### C) SP + Co-op (2-4 hráči)
- Active networking pro co-op questy
- Shared world state
- **Trade-off:** významný development effort
- **Vhodné pokud:** Co-op je core feature V2/V3

### D) SP + Async MP (např. world events sharing)
- SP zážitek, ale světy se ovlivňují (Death Stranding style)
- Lehčí networking, asynchronní
- **Trade-off:** komplexní design rozhodnutí
- **Vhodné pokud:** chceš social bez full MP

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní architektonická rozhodnutí:

```

---

# OTÁZKA 4: DAMAGE THRESHOLDS (KDY NASTÁVÁ CRITICAL INJURY)

**Proč to blokuje další design:** Tato čísla určují **feel celého combat**. Bez nich nelze balancovat zbraně, healing, ani difficulty.

## Možnosti

### A) Strict lethality (Tarkov-style)
- HP < 50% = critical
- Body shot = ~70% damage threshold
- Healing 8s vulnerable
- **Pocit:** Velmi smrtící, opatrné hraní

### B) Moderate lethality (návrh z aktuálního Tech Spec)
- HP < 30% = critical
- Body shot = ~50% damage threshold
- Healing 5s vulnerable
- **Pocit:** Smrtící, ale ne frustrující

### C) Forgiving lethality
- HP < 20% = critical
- Body shot = ~30% damage threshold
- Healing 3s vulnerable
- **Pocit:** Méně smrtící, více action-oriented

### D) Variable per region/difficulty
- Les: forgiving
- Město: moderate
- Modul lodi: strict
- **Pocit:** Progression napětí

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní hodnoty:
- Critical injury threshold: ___% HP
- Body shot threshold: ___ damage
- Healing animation time: ___ seconds
- Vulnerability during heal: yes/no
- Bleeding tick rate: ___ HP/second
- Time to death without heal: ___ seconds
```

---

# OTÁZKA 5: DRONE SCOPE V1 (KTERÉ FEATURES)

**Proč to blokuje další design:** Drone systém je core human-path mechanika. Architektura musí být připravena, ale **scope V1 určí, co testujeme v greyboxu**.

## Možnosti

### A) Tier 1 minimum (jen základ)
- Pasivní kamera, 60s baterie, 100m range
- GPS body update
- Bez audio sensoru, bez biometriky
- **Vhodné pokud:** chceš začít rychle, V2 přidá více

### B) Tier 1 + audio sensor (recon-light)
- Tier 1 + slyšíš zvuk
- Bez biometriky
- **Vhodné pokud:** audio je důležité pro stealth feel

### C) Tier 1 + Tier 2 features (full recon)
- Tier 1 + audio + biometrika (detection skrz lehký kryt)
- 120s baterie
- **Vhodné pokud:** drone je central feature pro V1

### D) Tier 1 + designové sloty pro Tier 2/3
- Tier 1 funkční, ale UI a slots pro upgrades hotové
- Hráč vidí, kam se to vyvíjí
- **Vhodné pokud:** chceš ukázat budoucí potenciál

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní specifikace pro V1:
- Battery: ___s
- Range: ___m
- Audio sensor: yes/no
- Biometrika: yes/no
- Speed: ___m/s
- HP (sestřelitelnost): ___
- Inventory cost: ___ slot(s)

Architektura připravena pro:
- Tier 2: yes/no (jaké features)
- Tier 3: yes/no (jaké features)
```

---

# OTÁZKA 6: ALIGNMENT EXCLUSIVITY RULES

**Proč to blokuje další design:** Bez tohoto nelze designovat questy ani progression. Klíčové rozhodnutí o tom, jestli **Hybrid path je viable nebo cul-de-sac**.

## Možnosti

### A) Plně exkluzivní cesty
- Human (0-33%) ↔ Alien (67-100%)
- Hybrid je punishing — nikdo netoleruje
- Hráč musí commitnout
- **Pocit:** Identitní choice je vážná

### B) Hybrid je nejhorší obou světů
- Hybrid má slabší stats obou stran
- Některé questy zamčené pro extremes
- "Nikdo ti nedůvěřuje plně"
- **Pocit:** Penalizace za neutrální

### C) Hybrid je flexible balanced
- Hybrid má střední stats, ale širší přístup
- Některé questy ZAMČENY pro extremes
- "Můžeš s každým"
- **Pocit:** Flexibilita za cenu specializace

### D) Hybrid je nejlepší (best of both)
- Hybrid kombinuje výhody obou
- Hard to achieve (tlak k extremes)
- Reward za balancing
- **Pocit:** Skill expression

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní pravidla:
- Hybrid range: ___% až ___%
- Co Hybrid umí (combat):
- Co Hybrid umí (NPC):
- Co Hybrid NEUMÍ:
- Hybrid quest accessibility: ___ % questů
```

---

# OTÁZKA 7: SAVE SYSTEM MECHANIKA

**Proč to blokuje další design:** PRD říká "free save", ale nedefinuje **implementační detail**. Save mechanika ovlivňuje difficulty, save scumming, story integrity.

## Možnosti

### A) Pure free save (kdekoli, kdykoli, neomezeně)
- Hráč ukládá, kdykoli chce
- Multiple slots
- Quick save klávesa
- **Trade-off:** save scumming v lethality combatu

### B) Free save + autosave checkpoints
- Hráč může uložit + auto checkpoints v klíčových místech
- Pokud zemře, restart z auto nebo manual
- **Trade-off:** balance mezi flexibility a tension

### C) Limited save zones (Dark Souls style)
- Save jen v určitých zónách (campfire / safe house)
- Auto checkpoint v krit. místech
- **Trade-off:** vyšší tension, ale frustrující

### D) Free save s consequences
- Save kdekoli, ale enemy reset nebo time pass
- Aliené posílí, pokud save scummuješ
- **Trade-off:** komplexní, ale sophisticated

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní specifikace:
- Save anywhere: yes/no
- Save slots: ___
- Quick save: yes/no (klávesa: ___)
- Auto save: yes/no (kdy: ___)
- Cloud save: yes/no
- Save during combat: yes/no
```

---

# OTÁZKA 8: DIFFICULTY OPTIONS STRATEGY

**Proč to blokuje další design:** Pokud bude multi-difficulty, **každý systém musí mít difficulty modifiers**. To je významný design overhead.

## Možnosti

### A) Single difficulty (žádný výběr)
- Jen jedna obtížnost — designerova vize
- Lethality je default
- **Trade-off:** menší trh, ale konzistentní zážitek
- **Reference:** Souls games

### B) 3 difficulties (Easy/Normal/Hard)
- Easy: méně lethality, více HP, vyšší healing
- Normal: designerova vize
- Hard: Tarkov mode
- **Trade-off:** trojnásobný balancing effort

### C) 2 difficulties (Standard/Hardcore)
- Standard: PRD vize
- Hardcore: permadeath, no save, harder
- **Trade-off:** double balancing, ale clearer

### D) Custom difficulty sliders
- Hráč si nastaví parametry
- Lethality slider, HP slider, AI accuracy
- **Trade-off:** balancing nightmare, ale flexible
- **Reference:** Wasteland 3, Pillars of Eternity

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní specifikace:
- Počet difficulties: ___
- Jména: 
- Lze měnit during game: yes/no
- Achievements based on difficulty: yes/no
```

---

# OTÁZKA 9: TUTORIAL STYLE

**Proč to blokuje další design:** Tutorial style ovlivňuje **každý quest design**, **environmental storytelling**, a **NPC dialogy**.

## Možnosti

### A) No tutorial (pure discovery)
- Hráč objevuje sám
- Lore deníky a NPC dialogy
- Žádné popups
- **Trade-off:** elegantní, ale frustrující pro některé hráče
- **Reference:** Dark Souls, Hollow Knight

### B) Soft tutorial (contextual hints)
- Popup tipy v relevantní situaci
- "Stiskni F pro ovládání drona" první encounter
- Disable po naučení
- **Trade-off:** balance learning + freedom

### C) Tutorial mission (první 30 minut)
- Strukturovaná tutorial mise
- Postupné odemykání mechanik
- Skippable pro veterány
- **Trade-off:** klasické, ale méně immersive

### D) Lore-integrated tutorial (Bioshock style)
- Tutorial je součástí příběhu
- Hráč se učí mechaniky **jako postava**
- Žádné meta-popups
- **Trade-off:** elegantní, ale narrative-locked

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní pravidla:
- Style: 
- Skippable: yes/no
- Trvání tutorial sekce: ___
- Fallback pro veterány: 
```

---

# OTÁZKA 10: V1 DEADLINE A MILESTONE STRATEGIE

**Proč to blokuje další design:** Bez deadline nelze plánovat scope, sprint plán, ani kdy začít vizuální fázi.

## Možnosti

### A) Aggressive — V1 za 6 měsíců
- Minimální scope
- 1 region, 3 questy, basic features
- **Trade-off:** crunch risk, ale rychlý feedback
- **Vhodné pokud:** tým 4+ lidí

### B) Realistic — V1 za 9-12 měsíců
- Standard indie tempo
- Plný V1 scope per PRD
- 2-week sprints
- **Trade-off:** rozumný balance

### C) Conservative — V1 za 12-18 měsíců
- Solo nebo malý tým
- Plný scope + buffer pro iterace
- Žádný crunch
- **Trade-off:** delší time-to-feedback

### D) No deadline — feature complete
- Releaes when ready
- **Trade-off:** scope creep risk, ale plná kvalita
- **Reference:** Star Citizen (negativně), Dwarf Fortress (pozitivně)

### E) Vlastní odpověď

## ROZHODNUTÍ

```
[ZAPIŠ ROZHODNUTÍ ZDE]

Vybráno: [A/B/C/D/E]

Zdůvodnění:


Konkrétní timeline:
- V1 vertical slice deadline: 
- Greybox milestone: 
- Alpha milestone: 
- Beta milestone: 
- Sprint length: ___ weeks
- Major milestones každé: ___ weeks
```

---

# DALŠÍ OTÁZKY (Backlog — pro další iterace)

Tyto otázky nejsou v top 10, ale budou potřeba dál:

## 🟡 Důležité (decision log v0.2)
- Větvení questů V1 (kolik narativních cest)
- EMP zone radius (konzistentní per modul?)
- Heatmap decay timing (přesné hodnoty)
- NPC reputation system formula
- Loot rarity tiers (kolik a jaké)
- Crafting failure rates
- Day/Night V1 (statický confirm)
- Weather V1 (žádné confirm)

## 🟢 Žádoucí (decision log v0.3+)
- Localization plán
- Console support roadmap
- Mod support strategy
- Achievement design philosophy
- Marketing strategy
- Beta plan
- Post-launch content roadmap

---

# WORKFLOW PRO TENTO DOKUMENT

## Jak rozhodnutí uzavřít

1. **Diskutuj** otázku s týmem (nebo sám se sebou)
2. **Vyber** A/B/C/D/E + zdůvodnění
3. **Zapiš** do sekce ROZHODNUTÍ pod otázkou
4. **Změň status** v tabulce na "✅ Uzavřeno"
5. **Přesuň** rozhodnutí do příslušného dokumentu (PRD/Game Bible/Tech Spec)
6. **Odkaz** v této sekci na dokument, kde je to teď

## Příklad uzavřeného rozhodnutí

```
ROZHODNUTÍ (uzavřeno 2025-XX-XX):

Vybráno: B (Mikro-tým 2-3 lidi)

Zdůvodnění: Solo je riziko burnoutu, indie tým je finančně 
nedostupný. Mikro-tým 3 lidí dává dostatečnou specializaci 
bez velkého overhead.

Jména/role:
- Jan: Lead designer + Unity programmer
- [TBD]: Artist (3D + 2D, freelance)
- [TBD]: Audio (možná outsource)

Status: ✅ UZAVŘENO
Přesunuto do: PRD sekce 0 (Status), Workflow Fáze 0
```

---

# SHRNUTÍ — co se odblokuje, když uzavřeš tyto otázky

| Po uzavření... | Můžeš začít... |
|---|---|
| **Otázka 1 (tým)** | Plánovat sprint plán, role assignments |
| **Otázka 2 (HW/engine)** | Optimalizační rozhodnutí, asset budgety |
| **Otázka 3 (multiplayer)** | Architektonický design, save system |
| **Otázka 4 (damage)** | Detailní balancing tabulky, AI difficulty |
| **Otázka 5 (drone)** | Greybox testování, art assets |
| **Otázka 6 (alignment)** | Quest design, NPC dialog system |
| **Otázka 7 (save)** | Implementace save systému |
| **Otázka 8 (difficulty)** | Difficulty modifiers v Tech Spec |
| **Otázka 9 (tutorial)** | Quest 1 design, environmental storytelling |
| **Otázka 10 (deadline)** | Production roadmap, sprint planning |

**Po uzavření všech 10:** Můžeš s jistotou pokračovat na **detailní quest design** a **art direction**.

---

Decision Log v0.1 — Siege of the Blue World

*Aktualizovat při každém uzavřeném rozhodnutí.*

*Související dokumenty: PRD, Game Bible, Tech Spec, Production Roadmap.*
