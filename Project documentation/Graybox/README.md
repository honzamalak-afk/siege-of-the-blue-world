# Greybox YAML Specification — Quick Start

## Co to je

YAML reprezentace dokumentu `SiegeBlueWorld_GreyboxSpec_v0_4_FINAL.md` rozdělená do logických celků pro Claude Code.

**Cíl:** Každý soubor = 1 implementační prompt. "Vezmi tento YAML → naimplementuj → 80%+ hotové na první pokus."

## Struktura

```
yaml_output/
├── 00_index.yaml              ← Master index, pořadí implementace, závislosti
├── 01_world_map.yaml          ← Mapa, zóny, NavMesh, color coding
├── 02_player.yaml             ← Player controller, inventář, NPC velitel
├── 03_drone.yaml              ← Recon Drone companion
├── 04_enemies.yaml            ← Alien, Puppet, Puppeteer, Wardog
├── 05_combat.yaml             ← Body-part damage, weapon energy, sound
├── 06_spawn_system.yaml       ← Kaskádové hody, soft cap, migrace
├── 07_quest_flow.yaml         ← Quest beats 1-5, win/fail states
├── 08_telemetry.yaml          ← Unity Analytics, CSV, debug console
└── schemas/
    ├── 00_index.schema.json
    ├── 01_world_map.schema.json
    └── ... (1 schema per yaml)
```

## Pořadí implementace (z 00_index.yaml)

1. **01_world_map.yaml** → mapa & terén
2. **02_player.yaml** → player + inventář + NPC
3. **05_combat.yaml** → combat systém (musí být před enemies)
4. **04_enemies.yaml** → 4 typy enemy + AI
5. **03_drone.yaml** → drone companion
6. **06_spawn_system.yaml** → spawn manager
7. **07_quest_flow.yaml** → quest flow
8. **08_telemetry.yaml** → telemetrie + debug console

## Doporučené použití s Claude Code

### Možnost A — Jeden soubor per prompt (doporučeno)

```
"Otevři 01_world_map.yaml a implementuj greybox mapu v Unity."
```

### Možnost B — Skupina souborů

```
"Otevři 04_enemies.yaml + 05_combat.yaml a implementuj enemy systém s body-part damage."
```

### Možnost C — Plný workflow

```
"Otevři 00_index.yaml a postupuj podle implementation_order.
Každý soubor implementuj samostatně, čekej na můj review po každém kroku."
```

## Validace YAML

Pokud chceš ověřit, že YAML soubory jsou syntakticky správné a odpovídají schématům:

```bash
pip install pyyaml jsonschema
python3 validate.py   # nebo viz validation script v původním vygenerování
```

## Co je v každém YAML

Každý soubor obsahuje:

- **Implementační prompt** v hlavičce (komentář) — copy/paste do Claude Code
- **Zdroj** — odkaz na sekce v původní specifikaci
- **Dependencies** — co musí být hotové předtím
- **Design intent** — proč to dělat tak (rationale tam, kde má smysl)
- **Konkrétní data** — čísla, parametry, pravidla
- **Pseudocode** — implementační vodítko (kde užitečné)
- **Acceptance criteria reference** — odkaz do xlsx checklistu

## Konvence

- Jednotky jsou v názvech polí: `radius_m`, `time_s`, `speed_kmh`
- Booleany pro feature flags: `enabled_in_greybox: false` pro V1+ věci
- ⚠ ZMĚNA VŮČI — kde se YAML liší od staršího dokumentu
- Greybox simplifications jsou explicitně označeny

## Zdroj

`SiegeBlueWorld_GreyboxSpec_v0_4_FINAL.md` v0.4
