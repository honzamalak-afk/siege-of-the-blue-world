# PRD – Unity + Claude Code AI-Assisted Development Framework
**Version:** 4.1
**Author:** Jan Malák
**Date:** 2026-04-25
**Status:** OBSOLETE — viz PRD_Unity_Claude_Framework_v4_2.md
**Předchozí verze:** PRD_Unity_Claude_Framework_v4.md (v4.0, OBSOLETE)
**Nástupce:** PRD_Unity_Claude_Framework_v4_2.md (v4.2)

---

## 0. Změny oproti v4.0 (TL;DR)

v4.0 přinesl tři kotvy: bezpečnost (Undo groups), autonomní verifikaci (testy, runtime introspekce, schema), protokolovou zralost (versioning, named pipe, reload survival).

v4.1 je **ergonomický release** — dovařuje DX díry, které se v praxi ukázaly jako třecí body. Žádná architektonická změna; rozšiřuje existující commandy a přidává 4 nové ze série "levné, ale velký payoff".

**Tematicky:**
- **Lepší error hinty** (jaké hodnoty by byly platné, ne jen "selhalo")
- **Idempotence vynucená** přes parametry, ne jen jako NFR
- **Cross-cutting screenshot** a per-command dry_run pro audit i "vidím okamžitě"
- **`edit_file`** s diff/replace semantikou jako jeden command místo read+write+compile triády
- **JSON schema validace** vstupů — Claude vidí "missing required: X" před tím, než se něco rozjede
- **Console subscribe** — streamování místo pollingu
- **`find_assets_by_content`** — grep nad C# pro rychlou orientaci
- **Bug ledger** — perzistentní paměť bugů, které Claude způsobil, jejich symptomů a fixu; konzultována před každým netriviálním fixem

Detail v sekcích 4.20–4.26 a 8.

---

## 1. Overview

### 1.1 Project Name
**UCAF** – Unity Claude Assisted Framework

### 1.2 Purpose
Beze změny vůči v4.0. UCAF nadále spojuje Claude Code s Unity Editorem tak, aby Claude pracoval jako autonomní junior dev s plnou verifikační smyčkou (vidí, testuje, rollbackuje).

### 1.3 Role split
Beze změny vůči v4.0.

### 1.4 Vision Statement
*"Junior co dělá, vidí, testuje, umí to vrátit zpět — a navíc se neptá tě dvakrát na to samé pole."*

### 1.5 Co UCAF není
- Není autopilot ani game designer
- Negaruje game feel ani vizuální polish
- Není CI server — testy se spouští on-demand z Claudova workflow, ne na každý commit

> **Poznámka v4.1:** Předchozí verze explicitně vyřazovaly oblast vizuálního/grafického designu (shadery, VFX, animace, post-process) ze scope. Tato vyřazení byla v4.1 odstraněna — k tomuto tématu se vrátíme v některé z budoucích verzí.

---

## 2. Goals and Non-Goals

### 2.1 Goals v4.1 (delta proti v4.0)
- **Strukturované error hinty:** chybové response u `set_field` a CRUD obsahují `expected_type`, `valid_values_sample`, `hint` — Claude se neučí trial-and-error
- **Vynucená idempotence:** všechny `create_*` commandy mají `if_exists=skip|replace|error|rename` (default `error`)
- **Cross-cutting screenshot:** každý write command přijímá `screenshot_after=true` — odpadá samostatný `take_screenshot` v junior loopech
- **Per-command dry_run:** všechny destruktivní/write commandy přijímají `dry_run=true` — vrátí "co by se stalo" bez aplikace
- **`edit_file` jako atomický edit:** diff/replace + volitelný `compile=true` v jednom commandu místo read → modify → write → compile
- **Schema-first validation:** každý command má JSON schema; listener validuje vstup před exekucí, vrací standardizovaný `validation_error` s missing/invalid fields
- **Console streaming:** `subscribe_console` pumpuje řádky do `ucaf_workspace/streams/console.ndjson`; Claude tail-uje místo pollingu
- **Code grep:** `find_assets_by_content` — regex nad textovými assety (.cs, .shader, .uxml, .asmdef)
- **Bug ledger (paměť bugů):** každý bug, který Claude způsobí + způsob řešení, se ukládá do strukturované paměti; konzultuje se před fixem podobného symptomu — "tohle už jsem viděl, posledně to byl X"

### 2.2 Non-Goals (v4.1)
- Automatický download assetů — Jan vybírá a importuje
- Game feel tuning — Claude změní hodnotu na příkaz, nehodnotí zda se to dobře hraje
- Multiplayer networking, jiné enginy
- CI/CD pipeline — UCAF testy běží lokálně na vyžádání, ne v cloudu
- Coverage gating — testy reportují pass/fail, ne metriky kvality kódu

> **Poznámka v4.1:** Vyřazení oblasti vizuálního/grafického designu (shader/VFX/animace/HDRP look tuning) z předchozích verzí bylo z této sekce odstraněno — vrátíme se k tomu později.

---

## 3. Users and Use Cases

### 3.1 Primary User
Beze změny vůči v4.0.

### 3.2 Rozdělení odpovědnosti
Beze změny vůči v4.0.

### 3.3 Core Use Cases (delta v4.1)

#### UC-09 (NEW): Iterativní debug s console streamem
> "Zkus, co se děje, když hráč skočí dvakrát rychle za sebou."

Claude:
1. `subscribe_console severity=warning,error` — start streamu
2. `enter_play_mode`
3. `simulate_input action=Jump` (2× rychle)
4. **Tail `ucaf_workspace/streams/console.ndjson`** — vidí logy téměř real-time, žádný 0.5 s polling
5. Identifikuje issue, opraví, znovu test

#### UC-10 (NEW): Atomický edit existujícího skriptu
> "V `PlayerController.cs` přejmenuj `_speed` na `_moveSpeed`."

Claude:
1. `edit_file path=Assets/Scripts/PlayerController.cs replace="_speed" with="_moveSpeed" all=true compile=true`
2. Listener: read → replace → write → AssetDatabase.Refresh → compile_and_wait → vrátí výsledek včetně compile errorů
3. Jediný roundtrip místo 4

#### UC-11 (NEW): Mass-create s idempotencí
> "Vytvoř 10 spawn pointů `Spawn_01` až `Spawn_10`."

Claude:
1. `batch` s 10× `create_object name=Spawn_NN if_exists=skip`
2. Pokud spustí Jan stejný command znovu, neudělá 20 objektů ani neselže — skipne existující
3. Report: "vytvořeno 4, přeskočeno 6 (existovaly)"

#### UC-12 (NEW): Orientace přes code grep
> "Kde se používá `OnEnemyDeath`?"

Claude:
1. `find_assets_by_content pattern=OnEnemyDeath glob=*.cs`
2. Vrátí: `Assets/Scripts/Combat/Enemy.cs:42`, `Assets/Scripts/UI/KillCounter.cs:18`, ...
3. Žádné AssetDatabase, jen Directory + Regex — sub-100ms i ve velkých projektech

#### UC-13 (NEW): Učení z předchozích bugů
> "Hráč zase nereaguje na input, jako minule."

Claude:
1. `find_similar_bugs symptom="hráč nereaguje na input" tags=input,playmode` — paměť vrátí předchozí incidenty
2. Najde záznam: *"Bug #BUG-0034: PlayerInput komponenta byla disabled při enter Play Mode kvůli race condition v Awake — fix: přesunout enable do Start, viz commit abc1234, undo_group `fix-input-race`"*
3. Aplikuje známý fix (nebo ho rozšíří, pokud se symptom liší)
4. **Pokud bug byl skutečně nový → `log_bug` po vyřešení** — záznam pro příště
5. Pokud byl recurrence stejného → `update_bug occurrences=+1` — viditelnost, že fix je nestabilní / je třeba hlubší řešení

---

## 4. Functional Requirements

### 4.1 – 4.19
**Beze změny vůči v4.0.** Všechny FR-01 až FR-64 zůstávají v platnosti.

### 4.20 Strukturované error hinty (NEW v4.1)

- **FR-65:** Error response z `set_field`, `set_property`, `add_component`, `create_object`, CRUD nad SO obsahuje strukturovaný payload:
  ```json
  {
    "success": false,
    "error_code": "INVALID_VALUE",
    "message": "Cannot parse 'foo' as Color",
    "context": {
      "field": "tintColor",
      "expected_type": "Color",
      "valid_format_examples": ["#RRGGBB", "#RRGGBBAA", "1,0,0,1"],
      "hint": "Use hex string or comma-separated RGBA floats 0–1"
    }
  }
  ```
- **FR-66:** Pro typově citlivá pole platí dodatečné hinty:
  - **LayerMask** → `valid_values_sample: ["Default", "TransparentFX", "Player", ...]` (z `LayerMask.LayerToName`)
  - **ObjectReference** → `expected_component_type: "AudioClip"` + `hint: "Use asset_path to assign"`
  - **Enum** → `valid_values: [...]` (z reflexe enum typu)
  - **Tag** → `valid_values: [...]` (z `InternalEditorUtility.tags`)
- **FR-67:** Error code je stabilní enum (`INVALID_VALUE`, `MISSING_TARGET`, `TYPE_MISMATCH`, `OUT_OF_RANGE`, `READONLY_FIELD`, `COMPILATION_ERROR`, ...) — Python helper na něj může `match`.

### 4.21 Idempotence & dry_run cross-cutting (NEW v4.1)

- **FR-68:** Všechny `create_*` commandy přijímají `if_exists=skip|replace|error|rename`:
  - `error` (default, BC s v4.0) — selže pokud cíl existuje
  - `skip` — vrátí success bez akce, `data_json.skipped=true`
  - `replace` — smaže existující (v Undo groupu) a vytvoří nový
  - `rename` — vytvoří s automatickým suffixem, vrátí finální název
- **FR-69:** Všechny write commandy (create, set_field, modify, delete, edit_file, …) přijímají `dry_run=true`:
  - Listener provede validaci + spočítá co by se změnilo, ale nezapíše
  - Vrátí `data_json.would_change` = list `{target, field?, before, after}`
  - Sub-set `batch dry_run=true` (FR-29a) zůstává; tohle je generalizace na single command
- **FR-70:** Všechny write commandy přijímají `screenshot_after=true|game|scene|both`:
  - Po úspěšném provedení listener pořídí screenshot s metadaty (FR-24a) a přidá `screenshot_path` + `camera_meta` do response
  - V kombinaci s `select_object` před commandem dává Claudovi vizuální potvrzení v jednom roundtripu

### 4.22 `edit_file` (NEW v4.1)

- **FR-71:** `edit_file path=<asset_path>` s jedním z módů:
  - **`replace`** — `old_string` + `new_string` + `all=false|true` (jako Edit tool)
  - **`patch`** — unified diff payload, listener aplikuje
  - **`anchor`** — `before_line` / `after_line` regex + `insert` content
- **FR-72:** Volitelný `compile=true` parametr — po zápisu spustí `compile_and_wait` a zahrne výsledek (errory/warningy) do response
- **FR-73:** Volitelný `screenshot_after=true` (FR-70) — pro UI/Editor skripty co něco kreslí
- **FR-74:** `edit_file` je atomický uvnitř Undo groupu (FR-39 až FR-43); pokud `compile=true` a kompilace selže s `auto_rollback=true`, edit se vrátí
- **FR-75:** Sandbox dle NFR-07 (jen `Assets/`, `ucaf_workspace/`, `ProjectSettings/`)

### 4.23 JSON schema validation (NEW v4.1)

- **FR-76:** Každý command type má JSON schema (`Draft 2020-12`) v `ucaf_workspace/schemas/<command_type>.json`
- **FR-77:** Listener validuje příchozí command proti schématu **před exekucí**:
  - Missing required field → `validation_error` s `missing: ["field_name"]`
  - Wrong type → `validation_error` s `invalid: [{field, expected_type, got_type}]`
  - Extra unknown field (strict mode) → `validation_error` s `unknown: ["field_name"]` (warning, ne fail, default)
- **FR-78:** `protocol_capabilities` (FR-61) vrací i schema URLs — Python helper i Claude se mohou self-bootstrap
- **FR-79:** Schema je **single source of truth** pro CLAUDE.md command reference — generováno z JSON schemas, ne ručně udržováno

### 4.24 Console subscribe & streaming (NEW v4.1)

- **FR-80:** `subscribe_console severity=info|warning|error|all filters=[regex,...]` — listener spustí background log handler
- **FR-81:** Listener appenduje JSONL řádky do `ucaf_workspace/streams/console.ndjson`:
  ```json
  {"ts":"2026-04-25T10:23:11.412Z","severity":"error","message":"...","stack":"...","scene":"Main","frame":1234}
  ```
- **FR-82:** `unsubscribe_console subscription_id=...` — vypne stream; auto-unsubscribe po 30 min idle
- **FR-83:** Stream přežívá domain reload (FR-60); listener po reloadu re-attachne handler a appenduje dál
- **FR-84:** `get_console` (FR-21) zůstává — pro single-shot polling. Subscribe je pro debugging session.

### 4.25 Code grep (NEW v4.1)

- **FR-85:** `find_assets_by_content pattern=<regex> glob=*.cs|*.shader|... folder=Assets/...` — vrátí `[{path, line, match, context_before, context_after}]`
- **FR-86:** Implementace: `Directory.EnumerateFiles` + `Regex` proti file content; ne přes AssetDatabase (rychlejší)
- **FR-87:** Limit defaultně 200 hitů; `max_results` parametr; `head_limit` jako další ergonomie
- **FR-88:** Komplementární k FR-51 `find_references` (asset GUID grafy) — tohle pokrývá textové identifikátory a komentáře

### 4.26 Bug ledger / paměť bugů (NEW v4.1)

Cílem je perzistentní paměť bugů, které Claude **sám způsobil** (regrese po edit_file, broken wiring, špatně použitý API call, race condition), spolu se způsobem, jakým je opravil. Smysl: nedělat tu samou chybu dvakrát, a když ji udělá, najít předchozí fix dřív než začne improvizovat.

Bug ledger **není** to samé jako audit log:
- **Audit log** (FR-62 až FR-64) — mechanický záznam **každé operace** (write, menu item, latence). Strojová stopa.
- **Bug ledger** — kurátorovaný záznam **incidentů** (symptom, root cause, fix). Učení.

#### Storage

- **FR-89:** Bug ledger žije v `ucaf_workspace/memory/bugs.ndjson` — append-only JSONL, jeden bug per řádek. Existující záznamy se needitují přepsáním souboru, ale appendem nového "update" záznamu (immutable history).
- **FR-90:** Index `ucaf_workspace/memory/bugs_index.json` — denormalizovaný snapshot pro rychlé dotazy (latest verze každého bugu, agregované tagy). Listener jej regeneruje z ndjson při startu a po každém zápisu.
- **FR-91:** Záznam má strukturu:
  ```json
  {
    "bug_id": "BUG-0034",
    "created_at": "2026-04-25T10:23:11Z",
    "updated_at": "2026-04-25T10:45:02Z",
    "status": "fixed | recurring | open | wontfix",
    "title": "PlayerInput disabled po Enter Play Mode",
    "symptom": "Hráč nereaguje na input v prvních 0.2 s po vstupu do Play Mode",
    "scope": {
      "files": ["Assets/Scripts/Player/PlayerInput.cs"],
      "scenes": ["Assets/Scenes/Main.unity"],
      "components": ["PlayerInput", "PlayerController"]
    },
    "root_cause": "Race condition v Awake() — PlayerInput.enabled se nastavovalo dřív než byl device assigned",
    "fix": "Přesunuto z Awake() do Start(); přidán null-check na inputDevice",
    "fix_commits": ["abc1234"],
    "fix_undo_group": "fix-input-race",
    "tags": ["input", "playmode", "race-condition", "lifecycle"],
    "occurrences": 1,
    "introduced_by": "edit_file PlayerInput.cs (cmd_a1b2c3d4)",
    "verification": {
      "tests": ["PlayerInputTests.HandlesEarlyEnter"],
      "manual": "Vstoupit do Play Mode 5× rychle, ověřit že input reaguje od 1. framu"
    },
    "lessons": "Před `edit_file` na lifecycle metodách zkontrolovat ostatní komponenty na stejném objektu — Awake je race-prone."
  }
  ```

#### Commandy

- **FR-92:** `log_bug` — zapíše nový záznam. Required: `title`, `symptom`, `root_cause`, `fix`. Optional: `scope`, `tags`, `verification`, `fix_undo_group`, `lessons`. Listener doplní `bug_id` (auto-increment), `created_at`, `introduced_by` (z aktuálního command kontextu pokud je k dispozici).
- **FR-93:** `update_bug bug_id=... patch={...}` — appendne update řádek (status change, +1 occurrences, doplnění lessons). Předchozí verze zůstávají v ndjson.
- **FR-94:** `query_bugs` — filtrace podle:
  - `status`, `tags` (any/all), `scope.files` / `scope.components` (path match)
  - `since` / `until` (timestamp)
  - `text` (full-text přes title + symptom + root_cause + lessons)
  - `limit`, `offset`
- **FR-95:** `find_similar_bugs symptom=<text> [tags=...] [scope=...]` — sémantická podobnost. **První implementace:** TF-IDF / token overlap nad symptom + title + lessons (žádné embedding závislosti). Vrátí top-N s `similarity_score`. Pokud Jan později přidá embedding model, listener má pluggable backend.
- **FR-96:** `get_bug bug_id=...` — plná historie záznamu (initial + všechny updates).
- **FR-97:** `close_bug bug_id=... resolution=fixed|wontfix|duplicate [duplicate_of=...]` — explicit lifecycle.

#### Workflow integrace

- **FR-98:** Junior protokol (sekce 6) **vyžaduje** `find_similar_bugs` před netriviálním fixem (definice "netriviální" = víc než `set_field` jednoho pole, nebo cokoli zahrnující `edit_file`/skript change v rámci debug session).
- **FR-99:** Po vyřešení bugu, který Claude způsobil, je `log_bug` **povinný** krok reportu — ne volitelný. Reportovací formát (sekce 6) má povinné políčko `🐞 Bug ledger entry: BUG-XXXX` pokud byl bug zjištěn.
- **FR-100:** Pokud `find_similar_bugs` vrátí match s `similarity_score >= 0.7`, Claude **musí** v reportu uvést, jestli aplikuje předchozí fix nebo proč ne. To zviditelňuje recurrence — když se stejný bug objeví potřetí, je to signál, že fix nefungoval.

#### Bezpečnost a hygiena

- **FR-101:** Záznamy jsou výhradně append; mazání jen přes admin command `purge_bug bug_id=... reason=...` (logováno do auditu). Default je nemazat — historie regresí má hodnotu.
- **FR-102:** Velikost `bugs.ndjson` rotována size-based (default 5 MB → archivace do `memory/archive/bugs_<date>.ndjson`); index pokrývá všechny archivy.
- **FR-103:** Sandbox: čtení i zápis omezen na `ucaf_workspace/memory/`.

---

## 5. Non-Functional Requirements

| ID | Požadavek | Cíl | Změna v4.1 |
|----|-----------|-----|------------|
| NFR-01 až NFR-12 | (z v4.0) | – | beze změny |
| NFR-13 | Validation error response | < 50 ms (žádná Editor work) | nové |
| NFR-14 | `edit_file` (replace, < 1 MB soubor, bez compile) | < 200 ms | nové |
| NFR-15 | Console stream latence (řádek → ndjson) | < 100 ms | nové |
| NFR-16 | `find_assets_by_content` (10k souborů, jednoduchý regex) | < 500 ms | nové |
| NFR-17 | Schema completeness | 100 % command types má JSON schema, validation pre-exec | nové |
| NFR-18 | `find_similar_bugs` latence (1k záznamů) | < 200 ms | nové |
| NFR-19 | `log_bug` / `update_bug` perzistence | < 100 ms (ndjson append + index update) | nové |
| NFR-20 | Bug ledger rotace | Auto-archive při 5 MB; index pokrývá archivy | nové |

---

## 6. Junior dev protokol (aktualizováno pro v4.1)

### Před změnou
1. `find_objects` / `list_scene` — orientace
2. `get_object_info` / `list_components` na targetech
3. **`describe_component`** — schéma, ne hádání
4. **`find_assets_by_content`** při refaktoru / orientaci v kódu
5. `read_file` pokud upravuji existující skript (nebo rovnou `edit_file` viz níže)
6. `find_references` při refaktoru — dopad
7. **`find_similar_bugs symptom="..." tags=...`** pokud řeším bug nebo regresi — možná to už bylo viděno; aplikuj předchozí fix, nebo si všimni že předchozí fix nedržel

### Při změně
1. `begin_undo_group name="<co>"` pro netriviální / víc-krokové
2. Pro mass-edit: `batch dry_run=true` → schválení → `batch dry_run=false`
3. **Pro single risky write: `<command> dry_run=true` (FR-69)** → projít diff → znovu bez dry_run
4. **Místo `read_file` + úprava + `write_script` + `compile_and_wait` použít `edit_file mode=replace compile=true` v jednom kroku**
5. **`<create_*> if_exists=skip|replace|rename` místo manuální kontroly existence**
6. **Volitelně `screenshot_after=true` u commandu, který má vizuální dopad** — odpadá další `take_screenshot`

### Po změně
1. `compile_and_wait` (pokud nepoužito `compile=true`)
2. Při errorech v Undo groupu → `undo_group` (rollback)
3. **`subscribe_console` při debugging session**, jinak `get_console since=...`
4. `run_tests` pokud relevantní
5. `get_component_state at_runtime=true` v Play Mode
6. Screenshot (samostatný nebo ze `screenshot_after`)
7. `end_undo_group`
8. **Pokud byla v této session opravena regrese, kterou Claude sám způsobil → `log_bug`** (povinné, FR-99). Pokud `find_similar_bugs` vrátil shodu před fixem → `update_bug occurrences=+1` místo nového záznamu.

### Reportovací formát (rozšířeno)
```
✅ Co jsem udělal
📁 Změněné soubory / objekty
🧪 Testy: X/Y passed
👁️ Co vidím (screenshot path + camera meta)
🔁 Undo group: "<jméno>" — Ctrl+Z to vrátí
🪵 Console stream: <subscription_id>  (pokud aktivní)
🐞 Bug ledger: BUG-XXXX (logged | updated | similar match aplikován | žádný)
⚠️ Problémy / chybějící vstupy
➡️ Co dál / co potřebuji
```

### Kdy říct "stop" (nezměněno z v4.0)

---

## 7. System Architecture

```
[Jan zadává v přirozené řeči]
        ↓
[Claude Code CLI]
        ↓
[Python helper]
   ├── named pipe → UCAF_Listener (primary, < 50 ms)
   └── ucaf_workspace/commands/pending/ (fallback, < 1 s)
        ↓
[UCAF_Listener.cs]
   ├── JSON schema validator (pre-exec, FR-76 až FR-79)
   ├── command dispatcher (with protocol_version check)
   ├── Undo group manager
   ├── Domain reload survival (state in ucaf_workspace/state/)
   ├── Audit logger (ucaf_workspace/logs/audit_*.jsonl)
   ├── Console stream pump → ucaf_workspace/streams/console.ndjson
   └── Bug ledger store → ucaf_workspace/memory/bugs.ndjson + bugs_index.json
     ↓
   ┌──────────┬──────────┬───────────┬──────────┬──────────────┐
[Scene]  [Inspector]  [Scripts]  [Console]  [Test Runner]  [Reference]
  ops      bridge      r/w/edit   reader     EditMode/Play    graph
                       + diff      (poll                       + code grep
                       + atomic    + stream)                   (FR-85)
        ↓
[Result + structured error hints + screenshot+meta + would_change (dry_run)]
        ↓
[Claude vidí, ověří, screenshot je in-line, error má hint, iteruje]
```

---

## 8. Command Reference (v4.1 delta proti v4.0)

| Command | Nový/Rozšířen | Klíčové params | Returns |
|---------|---------------|----------------|---------|
| `edit_file` | 🆕 | `path`, `mode` (`replace`/`patch`/`anchor`), `old_string`, `new_string`, `all`, `compile`, `screenshot_after` | `data_json`: changed lines + compile result |
| `subscribe_console` | 🆕 | `severity`, `filters` | `subscription_id`, `stream_path` |
| `unsubscribe_console` | 🆕 | `subscription_id` | – |
| `find_assets_by_content` | 🆕 | `pattern`, `glob`, `folder`, `max_results` | list of hits with line+context |
| `validate_command` | 🆕 (alias k generic dry_run) | `command_type`, `params` | validation result + `would_change` |
| `log_bug` | 🆕 | `title`, `symptom`, `root_cause`, `fix`, `scope`, `tags`, `verification`, `fix_undo_group`, `lessons` | `bug_id` |
| `update_bug` | 🆕 | `bug_id`, `patch` | – |
| `query_bugs` | 🆕 | `status`, `tags`, `scope`, `since`, `until`, `text`, `limit`, `offset` | list of bugs |
| `find_similar_bugs` | 🆕 | `symptom`, `tags?`, `scope?`, `top_n` | list with `similarity_score` |
| `get_bug` | 🆕 | `bug_id` | full history |
| `close_bug` | 🆕 | `bug_id`, `resolution`, `duplicate_of?` | – |
| `purge_bug` | 🆕 (admin) | `bug_id`, `reason` | – (logováno do auditu) |
| `create_object` | ✏️ | + `if_exists`, `dry_run`, `screenshot_after` | + `data_json.skipped` / `would_change` |
| `create_scene` | ✏️ | + `if_exists`, `dry_run` | totéž |
| `add_component` | ✏️ | + `if_exists` (skip pokud už existuje) | totéž |
| `set_field` | ✏️ | + `dry_run`, `screenshot_after` | + structured error hint (FR-65, FR-66) |
| `modify_object`, `delete_object`, `reparent_object`, `duplicate_object` | ✏️ | + `dry_run`, `screenshot_after` | + structured error |
| (CRUD nad SO, prefab, asset) | ✏️ | + `if_exists`, `dry_run`, `screenshot_after` | totéž |

(v4.0 commandy zachovány. Strukturované error hinty platí pro VŠECHNY existující commandy z FR-09 až FR-14, ne jen ty výše.)

---

## 9. Success Criteria (delta v4.1)

- Cíle z v4.0 zůstávají.
- **`describe_component` + strukturované error hinty sníží neúspěch `set_field` na neznámém poli pod 2 %** (zlepšení z <5 % v v4.0)
- **Idempotentní `create_*` s `if_exists=skip` umožní opakované spuštění setup batchů bez manuální clean-up — 100 % případů neproduluje duplicitní objekty**
- **`edit_file` sníží počet roundtripů u úprav existujících skriptů o ~75 %** (4 commandy → 1)
- **Schema validation odchytí ≥90 % "missing required field" / "wrong type" chyb před exekucí** — místo Editor stack trace dostane Claude rovnou seznam co opravit
- **Console subscribe sníží debugging latency o >70 %** vs. polling `get_console` v interaktivních session
- **`find_assets_by_content` poskytne odpověď na "kde se používá X v kódu" pod 500 ms** v projektech do 10k souborů
- **Bug ledger zachytí ≥80 % bugů, které Claude způsobí** (povinný `log_bug` v post-fix protokolu)
- **`find_similar_bugs` najde shodu (similarity ≥ 0.7) u recurring bugů v ≥70 % případů** — méně improvizace, víc reuse fixu
- **Recurrence rate (stejný bug podruhé) klesne pod 15 %** po 3 měsících provozu ledgeru — pokud klesne nedostatečně, je to signál zlepšit fix kvalitu, ne ledger

---

## 10. Roadmap

### v2.0 / v3.0 (OBSOLETE)
Viz dřívější PRD.

### v4.0 (dokončeno — OBSOLETE)
Undo groups, Test Runner, schema introspekce, runtime introspekce, reference graph, named pipe, reload survival, audit log.

### v4.1 (tato verze — k implementaci)

**Levné (priorita 1, ~3 dny):**
- Strukturované error hinty (FR-65 až FR-67)
- `if_exists` napříč create commandy (FR-68)
- `screenshot_after` cross-cutting (FR-70)
- `find_assets_by_content` (FR-85 až FR-88)

**Střední (priorita 1, ~1 týden):**
- `edit_file` (FR-71 až FR-75)
- Per-command `dry_run` (FR-69)

**Střední / vyšší (priorita 2, ~1 týden):**
- JSON schema validation pipeline (FR-76 až FR-79)
- Console subscribe / streaming (FR-80 až FR-84)

**Bug ledger (priorita 1, ~3–4 dny):**
- Append-only ndjson + index (FR-89 až FR-91)
- CRUD commandy (FR-92 až FR-97)
- Workflow integrace v junior protokolu (FR-98 až FR-100)
- Hygiena: rotace, sandbox, purge audit (FR-101 až FR-103)

Celkem ~2,5 týdne, žádná architektonická změna proti v4.0.

### v4.2 (plánováno — bývalé v4.1 z PRD v4.0)
- HDRP Volume bridge — read/write profile overrides
- `get_prefab_overrides` — explicitní seznam overridů
- NavMesh bake trigger
- Lightmap bake trigger
- `import_settings` — AssetImporter (FBX rig, animation, normal map)

### v4.3 (výhled)
- Coverage report z Test Runneru (informativní)
- Code search napříč Assets přes Roslyn (find symbol usages, ne jen text grep)
- Profiler hook — frame stats při Play Mode testu

### Mimo scope
Beze změny vůči v4.0.

---

## 11. Open Questions

| Téma | Stav | Poznámka |
|------|------|----------|
| `edit_file mode=patch` formát | 🟡 open | Unified diff vs. Unity-friendly per-line replace; první implementace jen `mode=replace`, patch později |
| Schema strict vs. lenient | 🟡 open | Defaultně warning na unknown fields; Jan může zapnout strict v `ucaf_config.json` |
| Console stream rotace | 🟡 open | `console.ndjson` může růst; rotace per session nebo size-based (10 MB)? Default size-based |
| `if_exists=replace` u objektu s dětmi | 🟡 open | Smazat i děti, nebo re-parent? Default smazat (jasné chování), Jan může override |
| `screenshot_after` u failed commandu | 🟢 OK | Nepořizuje se — screenshot jen při success |
| `find_similar_bugs` algoritmus | 🟡 open | TF-IDF / token overlap v MVP; embedding model (např. lokální sentence-transformers) jako pluggable backend, pokud TF-IDF nedrží přesnost při >500 záznamech |
| Co se počítá jako "Claude způsobil bug" | 🟡 open | Default: regrese, kde introduction commit/undo group spadá pod UCAF write. Bugy v původním kódu Jana se logují volitelně (Jan rozhodne). |
| Privacy / sdílení ledgeru | 🟢 OK | `ucaf_workspace/memory/` v gitignore default; Jan může vyjmout pokud chce sdílet napříč zařízeními. |
| Auto-close na neaktualizované bugs | 🔴 open | Nezavírat automaticky — recurrence po 6 měsících má hodnotu. Manuální `close_bug` jen. |
| Open z v4.0 |  |  |
| Named pipe + Unity | 🟡 open | (přesun z v4.0) |
| Domain reload state | 🔴 open | (přesun z v4.0) |
| Auto-rollback default | 🟡 open | (přesun z v4.0) |
| TestRunnerApi async | 🟡 open | (přesun z v4.0) |

---

## 12. Migration z v4.0

- `ucaf_config.json` — nová pole: `console_stream_max_size_mb` (default 10), `validation_strict` (default false), `default_if_exists` (default "error"; pokud chce Jan defaultně skip, lze nastavit globálně), `bug_ledger_max_size_mb` (default 5), `bug_similarity_threshold` (default 0.7)
- `ucaf_workspace/schemas/` — vygenerovat JSON schemas při prvním běhu nového listeneru
- `ucaf_workspace/streams/` — adresář vytvoří listener (gitignore doporučeno)
- `ucaf_workspace/memory/` — adresář pro bug ledger (`bugs.ndjson`, `bugs_index.json`, `archive/`); gitignore doporučeno (privátní learnings); Jan může vyjmout, pokud chce sdílet
- CLAUDE.md regenerovat ze schémat (FR-79) — žádná ruční práce
- Existující commandy z v4.0 fungují beze změny; nové parametry jsou opt-in
- Bug ledger startuje prázdný; Jan může nasypat počáteční záznamy ručním editem `bugs.ndjson` (Session 1 fix patterns z `memory/project_siege_session1.md` jsou kandidáti)
