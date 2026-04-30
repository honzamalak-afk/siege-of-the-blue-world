**SIEGE OF THE BLUE WORLD**

PRODUCTION ROADMAP — Plán vývoje

Verze: 0.1 (kostra)

*Living document — aktualizuje se per sprint a po milnících.*

---

## ÚVOD K DOKUMENTU

### Co tento dokument je

**Production Roadmap** je **operativní plán vývoje hry**. Obsahuje časové milníky, sprint plány, odhady, rizika, a release strategii.

### Co tento dokument NENÍ

- ❌ Designové specifikace (→ PRD, Game Bible, Tech Spec)
- ❌ Detailní technické úkoly (→ task tracker — GitHub Issues / Notion)
- ❌ Marketing plán (→ samostatný Marketing Doc)

### Cílová čtenářská skupina

- **Tým** — ví, co je kdy očekáváno
- **Producer / lead** — řízení projektu
- **Stakeholders / investors** (pokud bude) — milníky, deliverables
- **Hráči (high-level public roadmap)** — komunikace o developmentu

### Závislost na rozhodnutích

⚠ **Tento dokument je závislý na uzavření Decision Logu.**

Klíčová rozhodnutí, která ovlivňují roadmap:
- Velikost týmu (Decision Log Q1)
- Cílový hardware (Decision Log Q2)
- V1 deadline (Decision Log Q10)

**Bez těchto rozhodnutí je roadmap pouze odhad.**

---

# ČÁST 1 — VYSOKÁ ÚROVEŇ

## 1.1 Project Overview

| Atribut | Hodnota |
|---|---|
| **Project name** | Siege of the Blue World |
| **Genre** | Singleplayer TPS Open-World RPG |
| **Engine** | Unity (HDRP) |
| **Platform** | PC (Windows) |
| **Scope (long-term)** | 20×20 km open world |
| **Scope (V1)** | Vertical slice — 1 region, 3 questy |
| **Project start** | [TBD — datum začátku] |
| **V1 target release** | [TBD — Decision Log Q10] |
| **Full game target** | [TBD — typicky 2-3 roky po V1] |

## 1.2 Project Phases

```
PHASE 0: Pre-production           [██████░░░░░░░░░░] V minulosti / now
PHASE 1: Vertical Slice (V1)      [░░░░░░░░░░░░░░░░] Next 6-12 měsíců
PHASE 2: Alpha Build               [░░░░░░░░░░░░░░░░] Po V1
PHASE 3: Beta + Polish             [░░░░░░░░░░░░░░░░] Po Alphě
PHASE 4: Release + Post-launch    [░░░░░░░░░░░░░░░░] Future
```

### Phase 0 — Pre-production (CURRENT)

**Cíl:** Mít dost dokumentace pro start tvorby.

**Deliverables:**
- ✅ PRD v0.2
- ✅ Game Bible v0.1
- ✅ Tech Spec v0.1
- ✅ Workflow Checklist v0.2
- ✅ Lore Bible v0.1 (kostra)
- ✅ Quest Design v0.1 (kostra)
- ✅ Production Roadmap v0.1 (kostra)
- ⚠ Decision Log uzavřený (top 10 otázek)

**Milestone:** "Documentation Ready for Greybox" — všechny v0.x dokumenty hotové, Decision Log top 10 uzavřený.

### Phase 1 — Vertical Slice (V1)

**Cíl:** Funkční prototyp s 1 regionem, 3 questy, všemi core mechanikami.

**Deliverables:**
- Greybox (Fáze 1.5 z Workflow)
- World blockout
- 3 questy implementované
- 4 typy nepřátel
- Recon Drone Tier 1
- GPS / Tactical Map
- Lethality combat
- Save/Load
- HDRP visual pass
- Internal + external playtest

**Milestone:** "V1 Vertical Slice Complete" — hra je hratelná end-to-end.

### Phase 2 — Alpha Build

**Cíl:** Rozšířit V1 do více regionů, dokončit core systémy.

**Deliverables:**
- 3-5 regionů
- 10-15 questů
- V2 features (Tier 2 drone, day/night, advanced AI)
- Plný progression systém
- All 15 V1 pravidel + V2 rozšíření
- Internal alpha test

### Phase 3 — Beta + Polish

**Cíl:** Plný open world, all features, bug-free.

**Deliverables:**
- 16-25 regionů (nebo zmenšení do realistic scope)
- 30-50+ questů
- Plný příběh
- Polish: graphics, audio, UI
- Performance optimization
- Closed beta → Open beta

### Phase 4 — Release + Post-launch

**Cíl:** Vydat hru, post-launch support.

**Deliverables:**
- Steam release
- Day-1 patch
- Post-launch content roadmap
- Mod support (pokud rozhodnuto)

---

# ČÁST 2 — TIMELINE (Realistic Estimate)

⚠ **Závislé na Decision Log Q1 (tým) a Q10 (deadline).**

## 2.1 Optimistic Timeline (Tým 4-6 lidí)

```
[Now]                                    [V1]              [Alpha]            [Release]
  │                                        │                  │                    │
  │ ────── 6-9 měsíců ──────────────────── │ ── 9-12 m ────── │ ── 9-12 m ──────── │
  │                                        │                  │                    │
Pre-prod                              V1 done            Alpha done          Beta + Release

Total: 24-36 měsíců (2-3 roky)
```

## 2.2 Realistic Timeline (Tým 2-3 lidi)

```
[Now]                                            [V1]                  [Alpha]                [Release]
  │                                                │                      │                       │
  │ ────── 9-12 měsíců ─────────────────────────── │ ── 12-18 m ────────  │ ── 12-18 m ────────── │
  │                                                │                      │                       │
Pre-prod                                      V1 done                Alpha done             Beta + Release

Total: 36-54 měsíců (3-4.5 roku)
```

## 2.3 Solo Dev Timeline

```
[Now]                                                    [V1]                          [Alpha]
  │                                                        │                              │
  │ ────── 12-18 měsíců ─────────────────────────────────  │ ── 18-24 m ──────────────── │ 
  │                                                        │                              │
Pre-prod                                              V1 done                      Alpha done

Total V1 + Alpha: 30-42 měsíců
Full game: 5+ let (může se hodit downscale ambition)
```

## 2.4 Doporučený přístup

> **Recommendation:** Solo dev na full open world je **velmi ambiciózní**. Doporučuji:
> 
> - **Krátkodobě (Phase 0-1):** Solo + AI tools je OK pro greybox a V1
> - **Střednědobě (Phase 2+):** Hire alespoň 1 dalšího člověka (artist nebo programmer)
> - **Dlouhodobě (Phase 3-4):** Full team nebo redukovat scope na "small open world" (5×5 km místo 20×20 km)

[Aktualizovat po Decision Log Q1]

---

# ČÁST 3 — SPRINT PLANNING (PHASE 1 / V1)

## 3.1 Sprint Strategy

### Sprint length

**Návrh:** 2-week sprints (industry standard).

**Důvody:**
- Krátké sprinty = rychlejší feedback
- 2 týdny = realistické pro indie tempo
- Konzistentní cadence pro retrospektivy

### Sprint structure

```
Týden 1: Implementation
   Den 1: Sprint planning (1-2h)
   Den 2-5: Development
   Den 6-7: Continued dev

Týden 2: Implementation + review
   Den 8-12: Continued dev + testing
   Den 13: Sprint demo (1-2h)
   Den 14: Retrospective + next sprint plan (1-2h)
```

## 3.2 V1 Sprint Plan (Detail)

### Sprint 1-2: Foundation (Setup + Player + AI basics)

**Goals:**
- Unity HDRP project setup
- Folder structure, Git, naming conventions
- Player Controller (movement, camera)
- Basic combat (raycast pistole)
- Basic AI (patrol + attack)
- HP system

**Deliverable:** Player se hýbe, střílí, AI ho útočí. Capsule scene.

### Sprint 3-4: Combat depth (Lethality + AI cover)

**Goals:**
- Body parts hitbox
- Critical injury state
- Healing animations
- AI cover behavior
- Sound detection system

**Deliverable:** Lethality combat funguje v test scéně.

### Sprint 5-6: Drone + Map

**Goals:**
- Recon Drone Tier 1
- Camera switch mechanika
- Battery system
- GPS / Tactical Map basics
- Heatmap rendering
- Decay timer

**Deliverable:** Hráč může vypustit drona, vidí body na mapě.

### Sprint 7-8: Save/Load + Inventory

**Goals:**
- Free save system
- Inventory dual limit
- Item categories
- Loot pickup
- Quest tracker UI

**Deliverable:** Hráč může uložit, načíst, sbírat loot.

### Sprint 9: Greybox Validation Setup

**Goals:**
- 400m corridor scéna
- 4 segmenty (les → město → modul)
- Spawn points pro 4 nepřátele
- 1 quest end-to-end (debug)

**Deliverable:** Greybox playable, ready for testing.

### Sprint 10: Greybox Testing & Iteration

**Goals:**
- Solo playtest 3×
- Iterace
- External playtest 3-5 lidí
- Final tweaks

**Deliverable:** Greybox validated, decision gate passed.

### Sprint 11-14: World Design (Blockout)

**Goals:**
- Full V1 region blockout
- 4-6 POI s konkrétními lokacemi
- NavMesh setup
- EMP zóny
- Spawn pointy

**Deliverable:** Plná V1 mapa hratelná v blockoutu.

### Sprint 15-20: Quest + Enemy implementation

**Goals:**
- Quest 1, 2, 3 implementace
- Puppet/Puppeteer mechaniky
- War Dog AI
- Hostile humans
- Crafting basic
- Skill tree basic

**Deliverable:** All 4 enemy types + 3 questy hratelné.

### Sprint 21-24: Art Pass

**Goals:**
- Final 3D models (environment, characters)
- Animace
- VFX (combat, EMP, drone)
- Audio (SFX, music)

**Deliverable:** Visually completed V1.

### Sprint 25-26: Playtest + Polish

**Goals:**
- Internal playtest 3×
- External playtest 5-10 lidí
- Bug fixes
- Performance optimization

**Deliverable:** V1 release-ready.

### Sprint 27-28: V1 Release

**Goals:**
- Code freeze
- Final build
- Internal presentation
- V2 scope decisions

**Deliverable:** **V1 VERTICAL SLICE COMPLETE**

## 3.3 Total Sprint Estimate

```
28 sprints × 2 weeks = 56 weeks ≈ 13-14 měsíců

S buffer (industry standard +30%): ~17-18 měsíců

S team 2-3 lidí: realistic
S team 4-6 lidí: 9-12 měsíců (komprese)
S solo: 18-24 měsíců (rozšíření)
```

[Adjustovat po Decision Log Q1]

---

# ČÁST 4 — RISK ASSESSMENT

## 4.1 Technical Risks

### Risk 1: HDRP performance

**Pravděpodobnost:** Vysoká
**Dopad:** Vysoký

**Popis:** HDRP s open-world (i 1×1 km V1) může mít FPS problémy na cílovém HW.

**Mitigation:**
- Performance testing od Sprint 5
- LOD groups povinné
- Object pooling od začátku
- Backup plan: switch na URP pokud HDRP nezvládne

**Status:** ⚠ Active monitoring

### Risk 2: Multiplayer architecture decision later refactor

**Pravděpodobnost:** Středně
**Dopad:** Vysoký

**Popis:** Pokud Decision Log Q3 = "passive MP-ready," ale pak chceme aktivní MP.

**Mitigation:**
- Rozhodni Decision Log Q3 ASAP
- Architektura connectovat na začátku, ne pozdě

**Status:** ⚠ Pending decision

### Risk 3: Save system scope creep

**Pravděpodobnost:** Středně
**Dopad:** Středně

**Popis:** "Free save" zní jednoduše, ale full state serialization (mapa decay, AI states, quest progress) je komplex.

**Mitigation:**
- V1 začít s simplified save (essential state only)
- Iterovat během development

**Status:** ⚠ Tech Spec needs detail

## 4.2 Design Risks

### Risk 4: Lethality balancing

**Pravděpodobnost:** Vysoká
**Dopad:** Vysoký

**Popis:** Lethality combat je hard to balance — frustrating vs. enjoyable.

**Mitigation:**
- Greybox playtest od Sprint 9
- Iterate damage thresholds (Decision Log Q4)
- External playtest pro reality check

**Status:** 🟢 Plan in place (Greybox Phase 1.5)

### Risk 5: Multiple solutions failure

**Pravděpodobnost:** Středně
**Dopad:** Vysoký

**Popis:** Rules-based design vyžaduje, aby všechny path options fungovaly. Pokud jedna dominuje, design fails.

**Mitigation:**
- Quest design template (3+ paths required)
- Greybox testing measures path diversity
- Iterate balance per quest

**Status:** 🟢 Plan in place (Quest Design Doc)

### Risk 6: Lore inconsistency

**Pravděpodobnost:** Středně
**Dopad:** Středně

**Popis:** Při expansi Lore Bible mohou vzniknout rozpory.

**Mitigation:**
- Glosář-driven approach
- Per-update consistency check
- Single owner pro lore (autor)

**Status:** 🟢 Plan in place (Lore Bible v0.1)

## 4.3 Production Risks

### Risk 7: Solo dev burnout

**Pravděpodobnost:** Vysoká (pokud solo)
**Dopad:** Catastrophic

**Popis:** Solo dev na ambitious project = burnout risk během 6-12 měsíců.

**Mitigation:**
- Realistic timeline (no crunch)
- Regular breaks
- AI tooling pro multiplikace produktivity
- Early hire, pokud financial allows

**Status:** ⚠ Pending Decision Log Q1

### Risk 8: Scope creep

**Pravděpodobnost:** Vysoká
**Dopad:** Vysoký

**Popis:** Open world ambition může bobtnat ("mohli bychom přidat...").

**Mitigation:**
- Strict V1 scope per PRD
- Decision Log explicit "out of scope"
- Per-sprint scope review

**Status:** 🟢 Plan in place (PRD section 13)

### Risk 9: Deadline slippage

**Pravděpodobnost:** Vysoká (industry standard)
**Dopad:** Středně-Vysoký

**Popis:** Game dev běžně překračuje deadliny o 50-100%.

**Mitigation:**
- Buffer 30% v každém estimate
- Milestone gates (no advance until current done)
- Honest retrospectives

**Status:** 🟢 Plan in place

## 4.4 Business Risks (pokud relevantní)

### Risk 10: Funding (pokud externí)

**[TBD podle Decision Log Q1]**

### Risk 11: Market changes

**Popis:** Žánr může v 2-3 letech změnit popularity.

**Mitigation:**
- Focus na unique features (rules-based, lethality, alien path)
- Nicht hra pro stable audience (Tarkov, STALKER fans)

---

# ČÁST 5 — BUDGET FRAMEWORK

⚠ **Závislé na Decision Log Q1.**

## 5.1 Cost Categories

### Software & Licenses

| Item | Cost (yearly) |
|---|---|
| Unity Pro (pokud) | $185/měsíc per seat |
| Asset Store assets (V1) | $500-2000 (one-time) |
| Cloud services (Git LFS, build server) | $20-100/měsíc |
| Other software (Substance, etc.) | $50-200/měsíc |

**V1 estimate:** $3000-10000

### Asset purchases (V1)

| Category | Estimated cost |
|---|---|
| 3D environment (HDRP) | $200-1000 |
| Characters (alien, hrdina) | $100-500 |
| VFX | $100-300 |
| Audio | $200-500 |
| Music | $200-1000 |

**V1 total:** $800-3300

### Team costs (per Decision Log Q1)

**Solo:** $0 direct (opportunity cost not counted)

**Tým 2-3:** $5000-15000/měsíc per person
- Junior: $3000-5000/měsíc
- Mid: $5000-8000/měsíc
- Senior: $8000-15000/měsíc

**V1 timeline 12 měsíců × 3 lidi mid:** ~$200-300k

### Other

- Marketing (pre-launch): $5000-50000
- Beta testing: $1000-5000
- PR / events: $2000-10000

## 5.2 Total V1 Budget Range

| Scenario | Estimate |
|---|---|
| **Solo + minimal assets** | $5,000-15,000 |
| **Mikro-tým 2-3 lidi (12m)** | $250,000-400,000 |
| **Indie tým 4-6 (9m)** | $500,000-1,000,000 |

## 5.3 Funding Strategy

**[TBD podle Decision Log Q1]:**
- Self-funded (solo)
- Bootstrapped (small team, savings)
- Investors / publisher (medium-large team)
- Crowdfunding (Kickstarter)
- Government grants (CZ Filmový fond, regional)

---

# ČÁST 6 — TEAM PLAN

## 6.1 Current State

**[Vyplň podle Decision Log Q1]:**

```
Aktuální tým:
- [Tvoje jméno]: Lead designer + ?
- [TBD]: ?
- [TBD]: ?

Outsourcing:
- ?
```

## 6.2 Hires Needed (V1)

**[Závislé na Decision Log Q1]:**

| Role | When | Type | Skills required |
|---|---|---|---|
| Unity programmer | Sprint 1 (start) | Full-time / freelance | Unity HDRP, C#, AI |
| 3D artist | Sprint 11 (art pass) | Freelance | Character + environment |
| Audio designer | Sprint 21 (art pass) | Freelance | SFX + music |
| QA tester | Sprint 25 (playtest) | Volunteer / freelance | Game testing experience |

## 6.3 Onboarding Plan (per role)

### For new hires

**Day 1:**
- Welcome + project overview (PRD)
- Tooling setup (Unity, Git, task tracker)
- Repo access

**Week 1:**
- Read PRD, Game Bible, Tech Spec
- Review Quest Design Doc
- First task (small, low-risk)

**Week 2-4:**
- Increasing responsibility
- Sprint participation
- First meaningful contribution

---

# ČÁST 7 — RELEASE STRATEGY

## 7.1 Target Platforms (priority order)

| Platform | When | Priority |
|---|---|---|
| Steam | V1 alpha + Release | 🔴 Primary |
| GOG | Release | 🟡 Secondary |
| Epic Games Store | Post-release | 🟢 Optional |
| Console (PS5/Xbox) | Post-release V2+ | 🟢 Future |

## 7.2 Release Phases

### Pre-release

**6-12 měsíců před release:**
- Steam page setup (basic)
- Devlog začíná (YouTube, blog, X)
- Community building (Discord)

**3-6 měsíců před:**
- Closed beta
- Influencer keys
- Press kit
- Trailer #1

**1-2 měsíce před:**
- Open beta
- Marketing push
- Trailer #2 (release)
- Reviews start

### Day 0 — Release

- Steam release
- Press release
- Streamers playing
- Day-1 patch ready

### Post-launch (Month 1-6)

- Monitor reviews + feedback
- Bug fix patches (weekly)
- Balance updates
- Content updates (free DLCs?)

### Long-term (Year 1+)

- V2 expansion (paid DLC)
- Community engagement
- Sale events
- Console ports (pokud rozhodnuto)

## 7.3 Pricing Strategy

**[TBD]:**
- $19.99 — indie standard
- $29.99 — mid-indie
- $39.99 — premium indie
- Early Access pricing (lower) → full release pricing

## 7.4 Localization Plan

**[TBD podle Decision Log v0.2]:**

| Language | When |
|---|---|
| Czech | V1 (native) |
| English | V1 (essential) |
| German, French, Spanish | Post-V1 |
| Russian, Polish | Post-V1 |
| Asian markets | V2+ |

---

# ČÁST 8 — KEY MILESTONES

## 8.1 Pre-production milestones

| Milestone | Status | Target Date |
|---|---|---|
| Vize konsolidovaná (PRD v0.1) | ✅ Done | Apr 2026 |
| Designové filozofie (PRD v0.2) | ✅ Done | Apr 2026 |
| Documentation foundation (all v0.x) | ✅ Done | Apr 2026 |
| Decision Log top 10 closed | ⚠ Open | [TBD] |
| Engine setup + folder structure | ⚠ Open | [TBD] |

## 8.2 V1 Production milestones

| Milestone | Estimated date | Status |
|---|---|---|
| **M1: Core Systems Done** (Sprint 8) | [Now + ~4 měsíce] | 🔘 Planned |
| **M2: Greybox Validated** (Sprint 10) | [Now + ~5 měsíců] | 🔘 Planned |
| **M3: World Blockout Done** (Sprint 14) | [Now + ~7 měsíců] | 🔘 Planned |
| **M4: Quests Implemented** (Sprint 20) | [Now + ~10 měsíců] | 🔘 Planned |
| **M5: Art Pass Done** (Sprint 24) | [Now + ~12 měsíců] | 🔘 Planned |
| **M6: V1 Released** (Sprint 28) | [Now + ~14 měsíců] | 🔘 Planned |

## 8.3 Post-V1 milestones

| Milestone | Estimated date | Status |
|---|---|---|
| **Alpha Build Complete** | [V1 + 12-18m] | 🔘 Planned |
| **Beta Open** | [V1 + 18-24m] | 🔘 Planned |
| **Full Release** | [V1 + 24-36m] | 🔘 Planned |

---

# ČÁST 9 — TRACKING & METRICS

## 9.1 Sprint Metrics

**Track each sprint:**
- Velocity (story points / tasks completed)
- Bugs introduced vs. fixed
- Documentation updates
- Playtester feedback (if greybox+)

## 9.2 Project Health Metrics

**Monthly:**
- Burndown chart (work remaining vs. timeline)
- Risk register status (which risks are escalating?)
- Team morale (1-10 self-report)
- Quality metrics (FPS, bugs, polish %)

## 9.3 Decision Velocity

**Track:**
- Open TBDs in PRD/Game Bible/Tech Spec
- Decision Log open questions
- Monthly: how many were closed?

**Target:** 2-5 decisions closed per month.

---

# ČÁST 10 — POST-LAUNCH ROADMAP

## 10.1 Month 1-3 post-launch

- Bug fixes (priority)
- Balance patches (per community feedback)
- Quality of life improvements
- Performance optimizations

## 10.2 Month 4-6

- Content updates (new questy, locations)
- Community events (developer streams, Q&A)
- Modding tools release (pokud rozhodnuto)

## 10.3 Year 1+

- Major expansion (paid DLC)
- Console ports (pokud rozhodnuto)
- Sequel pre-production (pokud project succeeds)

---

# CO JEŠTĚ CHYBÍ (priority pro v0.2)

## 🔴 Kritické

- Decision Log Q1, Q2, Q10 closed → tento dokument lze finalizovat
- Konkrétní team composition
- Konkrétní budget breakdown
- Konkrétní deadliny

## 🟡 Důležité

- Detail Sprint 1-10 (next 5 měsíců)
- Risk register update procedurum
- Communication plan (with stakeholders)

## 🟢 Žádoucí

- Detailní marketing plan
- Localization strategy detail
- Community management plan
- Post-launch content roadmap

---

# DESIGN NOTES (autoři)

## Při updatu roadmapu

✅ Aktualizuj **per sprint retrospektivu** (2 weeks)
✅ Aktualizuj **risk register** monthly
✅ Aktualizuj **milestones** quarterly
✅ Aktualizuj **timeline** po každém milestone

## Anti-patterns

❌ "Plan everything in detail upfront" (waterfall — hra dev nefunguje)
❌ "No deadlines, just vibes" (scope creep)
❌ "Hide risks until they explode" (transparency je key)
❌ "Solo dev plans like AAA studio" (unrealistic)

## Reality check otázky

Pravidelně si klást:
- Děláme to, co jsme plánovali?
- Plánujeme to, co bychom měli dělat?
- Říkáme si pravdu o pokroku?
- Měníme plán podle reality, nebo realitu podle plánu?

---

Production Roadmap v0.1 — Siege of the Blue World

*Aktualizovat po každém sprintu a milníku.*

*Související dokumenty: PRD (vize), Quest Design (obsah), Decision Log (rozhodnutí).*
