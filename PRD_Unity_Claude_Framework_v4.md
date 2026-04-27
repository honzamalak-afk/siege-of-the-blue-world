# PRD – Unity + Claude Code AI-Assisted Development Framework
**Version:** 4.0
**Author:** Jan Malák
**Date:** 2026-04-25
**Status:** OBSOLETE — nahrazeno PRD_Unity_Claude_Framework_v4_1.md (v4.1)
**Předchozí verze:** PRD_Unity_Claude_Framework_v3.md (v3.0, OBSOLETE)

---

## 0. Změny oproti v3.0 (TL;DR)

v3.0 zavřel feedback smyčku přes screenshot, ale Claude pořád pracoval **bez záchranné sítě**: žádný undo, žádné autonomní testy, žádná introspekce polí, žádná protection proti domain reloadu. v4.0 mění UCAF z "junior co vidí" na "junior co umí ověřit a rollbacknout".

**Tři kotvy v4.0:**
1. **Bezpečnost** — každá změna v Undo group, batch je transakce, dry-run pro mass-edit
2. **Autonomní verifikace** — EditMode + PlayMode test runner, runtime introspekce, find_references
3. **Protokolová zralost** — schema introspekce, versioning, domain reload survival, named pipe transport

Detail v sekcích 4.12–4.16 a 8.

---

## 1. Overview

### 1.1 Project Name
**UCAF** – Unity Claude Assisted Framework

### 1.2 Purpose
UCAF spojuje Claude Code s Unity Editorem na Windows 11 tak, aby se Claude choval jako **autonomní junior Unity developer**. Dostane zadání, udělá ho, vidí výsledky vlastní práce, **ověří je testy nebo runtime introspekcí**, a iteruje — bez nutnosti manuálního zásahu Jana v Editoru při běžných implementačních úkolech.

V3.0 zavřel **vizuální** feedback smyčku (screenshot, find_objects, read_file, batch). V4.0 zavírá **verifikační** smyčku: Claude umí spustit EditMode test, přečíst stav komponenty za běhu, najít kdo odkazuje na asset, popsat schéma pole bez hádání — a každou změnu provede tak, že ji jde rollbacknout přes standardní Unity Undo.

### 1.3 Role split (nezměněno z v3.0)
- **Claude = junior developer**: implementuje podle zadání, verifikuje, reportuje, ptá se
- **Jan = senior + art director + game designer**: zadává, rozhoduje o designu, drží vizuální nástroje

### 1.4 Vision Statement
*"Junior co dělá, vidí, testuje, a když to rozbije, umí to vrátit zpět — bez tebe."*

### 1.5 Co UCAF není
- Není autopilot ani game designer
- Nenahrazuje Shader Graph, VFX Graph, Timeline, Animator, Cinemachine — vizuální nástroje zůstávají u Jana
- Negaruje game feel ani vizuální polish
- Není CI server — testy se spouští on-demand z Claudova workflow, ne na každý commit

---

## 2. Goals and Non-Goals

### 2.1 Goals v4.0 (delta proti v3.0)
- **Reverzibilita:** každá UCAF operace je v Undo group, batch je atomický undo step — Jan i Claude umí Ctrl+Z
- **Autonomní testy:** EditMode + PlayMode test runner volaný jako command, výsledky strukturované
- **Schema introspekce:** Claude umí dotázat strukturu komponenty (pole + typy + atributy) místo hádání
- **Runtime introspekce:** v Play Mode umí číst stav MonoBehaviour instancí, ne jen serializované defaulty
- **Reference graph:** `find_references` + `find_missing_scripts` + `find_broken_references`
- **Dry-run pro batch:** mass-edit operace umí vrátit diff k schválení před aplikací
- **Domain reload survival:** listener přežije recompile/reload uprostřed dlouhé operace
- **Schema versioning:** `protocol_version` v každém commandu, drift detekovatelný
- **Screenshot metadata:** každý screenshot vrací kameru, FOV, selection bounds, viewport rect
- **Named pipe transport:** sub-ms latence vedle file-based fallbacku

### 2.2 Non-Goals (v4.0)
- Cinematic produkce (Timeline, Cinemachine, Recorder) — řídí Jan
- Automatický download assetů — Jan vybírá a importuje
- Autorství shaderů, VFX, animací — vizuální nástroje u Jana
- HDRP post-process / lighting finální tuning — Claude nastaví výchozí stav, Jan ladí look
- Game feel tuning — Claude změní hodnotu na příkaz, nehodnotí zda se to dobře hraje
- Multiplayer networking, jiné enginy
- **CI/CD pipeline** — UCAF testy běží lokálně na vyžádání, ne v cloudu
- **Coverage gating** — testy reportují pass/fail, ne metriky kvality kódu

---

## 3. Users and Use Cases

### 3.1 Primary User
**Jan Malák** — kreativní vedoucí projektu, zadává práci česky. Claude je junior: dostane zadání, udělá, ověří, ukáže výsledek, přijme korekci.

### 3.2 Rozdělení odpovědnosti (rozšířeno z v3.0)

| Oblast | Claude (junior) | Jan (senior/art/design) |
|---|---|---|
| Gameplay C# skripty | ✅ píše, čte, opravuje | dává zadání a review |
| Scene setup | ✅ dle popisu | finální kompozice |
| Inspector wiring | ✅ dle specifikace + introspekce | rozhoduje co kam patří |
| ScriptableObject data | ✅ CRUD + dry-run pro mass-edit | designuje schéma |
| Asset discovery | ✅ hledá co tam je | vybírá co tam patří |
| Screenshot pro ověření | ✅ pořídí + interpretuje metadata | není potřeba zasahovat |
| **EditMode/PlayMode testy** | ✅ **píše a spouští** | rozhoduje co testovat |
| **Refactor + reference search** | ✅ `find_references` + safe rename | review výsledku |
| **Rollback po failu** | ✅ Undo group automaticky | manuální redo když chce |
| Lighting / post-process | základní setup | finální look |
| Materiály / textury | přiřazení hotových | autorství |
| Shadery, VFX, animace | ❌ | ✅ |
| Game feel, balancing | upraví hodnoty na požádání | rozhoduje hodnoty |
| Asset výběr / import | ❌ | ✅ |

### 3.3 Core Use Cases (rozšířeno z v3.0)

#### UC-01: Implementace gameplay systému s testem
> "Udělej resource systém — hráč má dřevo, kámen, zlato, ukládá se to v ScriptableObjectu, UI v rohu obrazovky to zobrazuje. Napiš k tomu test."

Claude:
1. `find_objects` + `list_assets` — orientace
2. `describe_component component_type=ResourceManager` (pokud existuje) — schéma
3. Vytvoří `ResourceData` SO + `ResourceManager` + UI skript + **`ResourceManagerTests.cs`** (EditMode)
4. `compile_and_wait`
5. **`run_tests mode=EditMode filter=ResourceManagerTests`** — autonomní verifikace
6. `select_object` + `focus_scene_view` + `take_screenshot` — vizuální kontrola
7. Reportne: ✅ kód, ✅ testy (3/3), 📷 screenshot

#### UC-02: Úprava existujícího skriptu s rollbackem při failu
> "V PlayerController zvyš rychlost na 8 a přidej sprint na Shift."

Claude:
1. `read_file` PlayerController
2. `begin_undo_group name="Sprint feature"`
3. Upraví, zapíše, `compile_and_wait`
4. **Pokud kompilace selže** → `undo_group` (rollback) → reportne, neříká "spravil jsem"
5. Při úspěchu: Play Mode, `get_component_state at_runtime=true` na PlayerController — ověří `currentSpeed`
6. Screenshot, report

#### UC-03: Scene setup s validací
> "Připrav testovací scénu s terénem 100×100, hráčem uprostřed, třemi spawn pointy na okrajích."

Claude:
1. Vytvoří scénu v jednom `batch` s undo group
2. **`validate_scene`** — najde missing scripty, broken refs, neuložené overrides
3. Screenshot s metadaty (kamera + bounds) — pozná, jestli hráč skutečně je uprostřed terénu
4. Report

#### UC-04: Ladění chyby s runtime introspekcí
> "Když stisknu útok, nic se neděje."

Claude:
1. `find_objects component_type=PlayerCombat`
2. `read_file`, `read_console`
3. `enter_play_mode` → simulace inputu
4. **`get_component_state at_runtime=true path=Player`** — vidí `isAttacking`, `cooldownRemaining` za běhu
5. Identifikuje root cause, opraví, znovu test

#### UC-05: Hromadné wiring s dry-run
> "Všech 20 wave configů nastav easy/medium/hard podle pořadí."

Claude:
1. `list_assets type=WaveConfig`
2. **`batch dry_run=true`** — vrátí diff: "20 assetů, 60 polí, ukázka prvních 5 změn"
3. Jan schválí (nebo opraví zadání)
4. `batch dry_run=false` — provede s undo group
5. Report kolik bylo změněno

#### UC-06: Orientace v existujícím projektu (nezměněno z v3.0)
*Beze změny.*

#### UC-07 (NEW): Refaktor s reference search
> "Přejmenuj `EnemyHealth` na `UnitHealth`."

Claude:
1. **`find_references type=Script name=EnemyHealth`** — všechny .cs + .unity + .prefab + .asset, které to zmiňují
2. `begin_undo_group name="Rename EnemyHealth → UnitHealth"`
3. Rename souboru, update všech `using` + identifikátorů + serialized references přes `MonoScript` GUID
4. `compile_and_wait`
5. **`run_tests`** — pokud testy projdou, commit; pokud ne, undo_group → report co spadlo

#### UC-08 (NEW): Validace projektu před PR
> "Zkontroluj projekt před tím, než to commitnu."

Claude:
1. `validate_scene` — missing scripts, broken refs
2. **`find_missing_scripts`** přes všechny scény
3. **`find_broken_references`** přes všechny assety
4. `compile_and_wait` → 0 errorů
5. `run_tests mode=EditMode` → all green
6. Report: ✅ / ⚠️ / ❌

---

## 4. Functional Requirements

### 4.1 Scene control (nezměněno z v3.0)
- **FR-01 až FR-06** beze změny.

### 4.2 Object query (nezměněno z v3.0)
- **FR-07** `find_objects`, **FR-08** `get_object_info` beze změny.

### 4.3 Inspector bridge (nezměněno z v3.0)
- **FR-09 až FR-14** beze změny.

### 4.4 Scripting (nezměněno z v3.0)
- **FR-15 až FR-20** beze změny.

### 4.5 Console & runtime feedback (nezměněno z v3.0)
- **FR-21 až FR-23** beze změny.

### 4.6 Screenshot & visual feedback (rozšíření z v3.0)
- **FR-24 až FR-28** beze změny.
- **FR-24a (NEW v4.0):** `take_screenshot` vrací **metadata** v `data_json`:
  - `camera`: pozice, rotace, FOV, projection (perspective/ortho)
  - `selection`: GUID/path vybraného objektu, world bounds, screen-space bounding box
  - `viewport`: width × height, render path, scene/game view
- **FR-24b (NEW v4.0):** `take_screenshot mode=both` — vyrobí dva soubory (Game + Scene) v jednom commandu
- **FR-25a (NEW v4.0):** Screenshot je synchronní přes `ScreenCapture.CaptureScreenshotAsTexture` + `EncodeToPNG` — žádné polling-na-existenci-souboru

### 4.7 Batch operace (rozšíření z v3.0)
- **FR-29 až FR-31** beze změny.
- **FR-29a (NEW v4.0):** `batch dry_run=true` — vrátí seznam zamýšlených změn (per sub-command: target, field, before-value, after-value) **bez aplikace**. Slouží pro audit a Janovo schválení mass-editu.
- **FR-29b (NEW v4.0):** Batch je vždy **jeden Undo group** s názvem (`undo_group_name` param). Defaultně `"UCAF batch [timestamp]"`.

### 4.8 Asset discovery (nezměněno z v3.0)
- **FR-32, FR-33** beze změny.

### 4.9 Generic escape hatch (omezení z v3.0)
- **FR-34 (UPRAVENO v4.0):** `execute_menu_item menu_path=...` — spustí menu item, ALE:
  - Volá se proti **allowlistu** (`ucaf_workspace/menu_allowlist.json`); cokoli mimo → error
  - Každé volání je zalogováno s WARNING severity v UCAF audit logu
  - Defaultní allowlist obsahuje bezpečné položky (Window/*, Tools/UCAF/*); destruktivní (File/Build*, Edit/Project Settings save) musí Jan explicitně přidat

### 4.10 Play Mode & prefab workflow (rozšíření z v3.0)
- **FR-35, FR-36** beze změny.
- **FR-35a (NEW v4.0):** `simulate_input` — programaticky vyvolat InputAction / klávesu v Play Mode pro test smyček

### 4.11 Misc (nezměněno z v3.0)
- **FR-37, FR-38** beze změny.

### 4.12 Undo & transakce (NEW v4.0 — kritické)
- **FR-39:** Každý write command interně používá `Undo.RecordObject` / `Undo.RegisterCreatedObjectUndo` / `Undo.DestroyObjectImmediate`
- **FR-40:** `begin_undo_group name=...` / `end_undo_group` — explicitní transakce klenoucí více commandů
- **FR-41:** `undo_group` / `redo_group` — vrácení / opakování posledního UCAF undo groupu z Claudova promptu
- **FR-42:** Pokud `compile_and_wait` skončí s errory uvnitř undo groupu, listener volitelně auto-rollbackuje (`auto_rollback_on_compile_error=true` v config)
- **FR-43:** Undo group jméno je vidět v Unity Edit menu — Jan může Ctrl+Z jako u manuální editace

### 4.13 Test Runner (NEW v4.0 — kritické)
- **FR-44:** `run_tests mode=EditMode|PlayMode|All filter=<class or namespace>` — spustí Unity Test Runner přes `TestRunnerApi`
- **FR-45:** Výsledek vrací strukturovaně: `{ passed, failed, skipped, duration_ms, failures: [{name, message, stack}] }`
- **FR-46:** PlayMode test musí být schopen zachytit assembly reload — listener po reloadu pokračuje a vrátí výsledek
- **FR-47:** `write_test class_name path mode=EditMode|PlayMode template=...` — scaffolding NUnit test class do `Assets/Tests/`

### 4.14 Schema introspekce (NEW v4.0)
- **FR-48:** `describe_component component_type=Foo` — vrátí seznam serializovaných polí: `[{name, type, attributes:["Range(0,100)","Tooltip:..."], default_value, is_array}]`
- **FR-49:** `describe_scriptable_object asset_type=Foo` — totéž pro SO třídy
- **FR-50:** Schema je čerpáno přes reflexi + `SerializedObject` iteraci, ne hardcoded — vždy aktuální vůči zkompilované codebase

### 4.15 Reference graph (NEW v4.0)
- **FR-51:** `find_references target=<asset path or GUID>` — vrátí všechny assety i scény, které target zmiňují (přes `AssetDatabase.GetDependencies` reverzně + `EditorUtility.CollectDependencies`)
- **FR-52:** `find_missing_scripts scope=scene|project` — najde GameObject(y) s missing MonoBehaviour
- **FR-53:** `find_broken_references scope=scene|project` — najde serialized fields odkazující na neexistující GUID

### 4.16 Runtime introspection (NEW v4.0 — posunuto z v3.1)
- **FR-54:** `get_component_state path=<obj> component_type=Foo at_runtime=true` — v Play Mode vrátí aktuální field values (nejen serialized defaults), včetně non-serialized public/internal fields a properties (přes reflexi)
- **FR-55:** `invoke_method path=<obj> component_type=Foo method=Foo args=[...]` — zavolá public metodu na instanci za běhu (test surface, ne mainstream)

### 4.17 Validation (NEW v4.0)
- **FR-56:** `validate_scene` — vrací: missing scripts, missing prefab refs, unsaved overrides, GameObjecty bez aktivního renderable contentu když by měly být viditelné
- **FR-57:** `validate_assets folder=...` — broken refs, .meta orphans, GUID kolize

### 4.18 Protocol & transport (NEW v4.0)
- **FR-58:** `protocol_version` field v každém command JSON i response (string `"4.0"`); listener odmítne neznámou major verzi s rozumným error msg
- **FR-59:** Named pipe transport (`\\.\pipe\ucaf_<project_id>`) jako primární; file-based jako fallback při unavailable
- **FR-60:** Listener přežije domain reload — pending state v `ucaf_workspace/state/` (tracked command IDs, rozpracovaný batch progress); po reloadu pokračuje
- **FR-61:** `protocol_capabilities` command — vrátí seznam podporovaných commandů a verzí; Claude může self-detect feature gating

### 4.19 Audit log (NEW v4.0)
- **FR-62:** Každý write command + `execute_menu_item` se loguje do `ucaf_workspace/logs/audit_<date>.jsonl`
- **FR-63:** Záznam obsahuje: timestamp, command, params, result success/fail, undo_group_id, latency_ms
- **FR-64:** `get_audit since=<timestamp>` — Claude i Jan umí auditovat co se stalo

---

## 5. Non-Functional Requirements

| ID | Požadavek | Cíl | Změna |
|----|-----------|-----|-------|
| NFR-01 | Screenshot: command → soubor + metadata | < 2 s | zlepšeno z 3 s (sync API) |
| NFR-02 | Jednoduchý command response (pipe) | < 50 ms | nové (pipe transport) |
| NFR-02a | Jednoduchý command response (file fallback) | < 1 s | nezměněno |
| NFR-03 | `batch` response (10 sub-commandů) | < 3 s | zlepšeno z 5 s |
| NFR-04 | Kompilace s čekáním | reportuje čas, bez tvrdé hranice | uvolněno |
| NFR-05 | Unity verze | 6.4 (6000.4.3f1), HDRP 17.4.0 | nezměněno |
| NFR-06 | Stabilita | Žádná operace nesmí zanechat scénu v tichém broken stavu; **každá je v Undo groupu** | zpřísněno |
| NFR-07 | Bezpečnost | Zápis omezen na `Assets/`, `ucaf_workspace/`, `ProjectSettings/`; `execute_menu_item` přes allowlist | zpřísněno |
| NFR-08 | Idempotence | Opakovaný command nesmí duplikovat stav | nezměněno |
| NFR-09 | Domain reload survival | Listener přežije recompile uprostřed batch; pending command se po reloadu dokončí nebo cleanly errne | nové |
| NFR-10 | Test Runner overhead | EditMode test class < 5 s end-to-end | nové |
| NFR-11 | Audit completeness | 100 % write commandů a menu items v audit logu | nové |
| NFR-12 | Schema drift detection | Listener odmítne command s nekompatibilní `protocol_version` major | nové |

---

## 6. Junior dev protokol (aktualizováno pro v4.0)

### Před změnou
1. `find_objects` nebo `list_scene max_depth=2` — orientace, nehádám
2. `get_object_info` nebo `list_components` na dotčených objektech
3. **`describe_component component_type=...`** — když chci `set_field`, nejdřív zjistím schéma, neházím
4. `read_file` pokud upravuji existující skript
5. `find_references` při refaktoru — zjistím dopad před změnou

### Při změně
1. `begin_undo_group name="<co dělám>"` pro netriviální nebo víc-krokovou změnu
2. Pro mass-edit: `batch dry_run=true` → schválení → `batch dry_run=false`
3. Změny aplikuju

### Po změně
1. `compile_and_wait` — kontrola, že kód stojí
2. **Pokud errory + jsem v undo groupu → `undo_group` (rollback) → report, ne fix-and-pray**
3. `get_console since=<timestamp>` — žádné runtime errory
4. **`run_tests mode=EditMode filter=...`** — pokud relevantní testy existují
5. **`get_component_state at_runtime=true`** v Play Mode pokud jde o gameplay logiku
6. `select_object` + `focus_scene_view` + `take_screenshot` — vidím výsledek + metadata
7. `end_undo_group`

### Reportovací formát (rozšířeno)
```
✅ Co jsem udělal
📁 Změněné soubory / objekty
🧪 Testy: X/Y passed (failed: ...)
👁️ Co vidím (screenshot path + camera meta + relevantní hierarchie + Console)
🔁 Undo group: "<jméno>" — Ctrl+Z to vrátí
⚠️ Problémy / chybějící vstupy
➡️ Co navrhuji dál / co potřebuji od tebe
```

### Kdy říct "stop" (rozšířeno)
- Požadavek na Shader Graph / VFX Graph / Animator / Timeline
- Estetické rozhodnutí bez konkrétní hodnoty
- Chybějící asset
- **Kompilace selhala 2× po sobě v jednom undo groupu — rollback a popsat, co se nedaří, místo třetího pokusu**
- **Test selhává a nevím proč — popsat failure msg + stack, nehádat fix**

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
   ├── command dispatcher (with protocol_version check)
   ├── Undo group manager
   ├── Domain reload survival (state in ucaf_workspace/state/)
   └── Audit logger
     ↓
   ┌──────────┬──────────┬───────────┬──────────┬──────────────┐
[Scene]  [Inspector]  [Scripts]  [Console]  [Test Runner]  [Reference]
  ops      bridge      r/w/del    reader     EditMode/Play    graph
                                              + simulate_input
        ↓
[Result + screenshot+meta + console + test results + undo_group_id]
        ↓
[Claude vidí, ověří testy, iteruje, nebo rollbackuje]
        ↓
[Audit log: ucaf_workspace/logs/audit_<date>.jsonl]
```

### Klíčový rozdíl oproti v3.0
v3.0: Claude provede → vidí výsledek → iteruje
v4.0: Claude provede **v undo groupu** → **ověří testem nebo runtime introspekcí** → vidí výsledek **s metadaty** → buď `end_undo_group` (commit) nebo `undo_group` (rollback) → iteruje s plnou knowledge of impact

---

## 8. Command Reference (v4.0 delta proti v3.0)

| Command | Nový/Rozšířen | Klíčové params | Returns |
|---------|---------------|----------------|---------|
| `begin_undo_group` | 🆕 | `name` | `undo_group_id` |
| `end_undo_group` | 🆕 | – | – |
| `undo_group` | 🆕 | `undo_group_id` (default: last) | – |
| `redo_group` | 🆕 | – | – |
| `run_tests` | 🆕 | `mode`, `filter` | `{passed, failed, skipped, duration_ms, failures}` |
| `write_test` | 🆕 | `class_name`, `path`, `mode`, `template` | – |
| `describe_component` | 🆕 | `component_type` | list of fields w/ types & attributes |
| `describe_scriptable_object` | 🆕 | `asset_type` | totéž pro SO |
| `find_references` | 🆕 | `target` (path/GUID) | list of referencing assets+scenes |
| `find_missing_scripts` | 🆕 | `scope` | list of `{path, scene}` |
| `find_broken_references` | 🆕 | `scope` | list of `{owner, field, missing_guid}` |
| `get_component_state` | 🆕 | object + `component_type`, `at_runtime` | flat dict of fields+properties |
| `invoke_method` | 🆕 | object + `component_type` + `method` + `args` | return value |
| `validate_scene` | 🆕 | – | report |
| `validate_assets` | 🆕 | `folder` | report |
| `simulate_input` | 🆕 | `action`/`key`, `state` | – |
| `take_screenshot` | ✏️ | `mode` (`game`/`scene`/`both`) | `screenshot_path(s)` + camera/selection metadata |
| `batch` | ✏️ | `commands`, `stop_on_error`, `dry_run`, `undo_group_name` | results / diff (when dry_run) |
| `execute_menu_item` | ✏️ | `menu_path` | check against allowlist; logged |
| `protocol_capabilities` | 🆕 | – | list of supported commands + versions |
| `get_audit` | 🆕 | `since`, `command_filter` | audit log entries |

(Commandy z v3.0 zůstávají, jen mají v request/response navíc `protocol_version`. Detailní reference v `CLAUDE.md`.)

---

## 9. Success Criteria

- Jan zadá implementaci gameplay systému (UC-01 rozsah) a Claude ho dokončí **bez ručního zásahu v editoru** v >85 % případů (zlepšení z 80 %)
- Claude **vidí výsledek + metadata** screenshotu a iteruje bez Jana v >90 % vizuálních kontrol
- Claude správně přečte a opraví existující skript v >85 % případů (zlepšení z 80 %)
- **Když změna selže (kompilace nebo test), Claude rollbackuje undo group v >95 % případů místo "fix-and-pray"**
- **Pokud existují EditMode testy pro dotčený systém, Claude je spustí v 100 % případů**
- **`describe_component` eliminuje guess-work při `set_field` — chybovost na neznámém poli klesne pod 5 %** (proti odhadu ~25 % bez introspekce)
- Claude se nepokouší dělat věci mimo junior scope — při naražení na strop **explicitně řekne**
- Jan nikdy neskončí ve stavu, kdy musí hádat, co Claude udělal — report + audit log + undo groupy
- `batch` snižuje čekací dobu sekvencí o >60 % oproti sekvenčním commandům
- **Domain reload uprostřed batch operace nezpůsobí ztrátu výsledků v 100 % případů**

---

## 10. Roadmap

### v2.0 (dokončeno)
Inspector bridge, scene enumeration, console reader, compile_and_wait, prefab workflow

### v3.0 (dokončeno — OBSOLETE)
`take_screenshot`, `find_objects`, `read_file`/`delete_file`, `batch`, `list_assets` generalized, `append_array_element`, `execute_menu_item`

### v4.0 (tato verze — k implementaci)

**Fáze A — bezpečnost (priorita 1):**
- Undo group infrastruktura (FR-39 až FR-43)
- `execute_menu_item` allowlist (FR-34 upraveno)
- Audit log (FR-62 až FR-64)
- Schema versioning (FR-58, FR-61, NFR-12)

**Fáze B — autonomní verifikace (priorita 1):**
- Test Runner integrace (FR-44 až FR-47)
- Runtime introspekce (FR-54, FR-55)
- `describe_component` / `describe_scriptable_object` (FR-48 až FR-50)
- `find_references` + `find_missing_scripts` + `find_broken_references` (FR-51 až FR-53)
- `validate_scene` / `validate_assets` (FR-56, FR-57)

**Fáze C — protokol & DX (priorita 2):**
- Domain reload survival (FR-60, NFR-09)
- Named pipe transport (FR-59, NFR-02)
- Screenshot metadata + sync API (FR-24a, FR-24b, FR-25a)
- `batch dry_run` (FR-29a, FR-29b)
- `simulate_input` (FR-35a)

### v4.1 (plánováno)
- HDRP Volume bridge — read/write profile overrides
- `get_prefab_overrides` — explicitní seznam overridů na prefab instanci
- NavMesh bake trigger
- Lightmap bake trigger
- `import_settings` — AssetImporter (FBX rig, animation, normal map)

### v4.2 (výhled)
- Coverage report z Test Runneru (informativní, ne gating)
- Code search napříč Assets přes Roslyn (find symbol usages)
- Profiler hook — záchyt frame stats při Play Mode testu

### Mimo scope (zůstává u Jana)
- Timeline / Cinemachine / Unity Recorder
- Shader Graph / VFX Graph autorství
- Animator Controller autorství
- Asset Store / automatický download
- AI image generation
- CI/CD pipeline (UCAF testy běží lokálně)

---

## 11. Open Questions

| Téma | Stav | Poznámka |
|------|------|----------|
| Named pipe na Windows + Unity | 🟡 open | `System.IO.Pipes.NamedPipeServerStream` v Editor coroutine — testovat reload chování |
| Domain reload state serializace | 🔴 open | Pending command + undo group ID musí přežít reload; `[InitializeOnLoad]` resume hook potřeba |
| Auto-rollback při compile fail | 🟡 open | Default off / on? Default off bezpečnější (Jan vidí broken state a rozhodne), default on rychlejší smyčka — config flag |
| TestRunnerApi async results | 🟡 open | API je callback-based; listener musí počkat než vrátí response — timeout pro dlouhé PlayMode testy |
| Reflexe non-serialized polí | 🟡 open | `BindingFlags.NonPublic | Instance` — security? Pro junior dev OK, ale audit log to musí zachytit |
| Allowlist pro `execute_menu_item` | 🟢 OK | Default safe set + Jan přidává; deny-by-default |
| `protocol_version` mismatch UX | 🟢 OK | Listener vrátí structured error s vlastní version + "upgrade UCAF_Listener.cs" hint |
| Resolved z v3.0 |  |  |
| ~~`take_screenshot` async~~ | ✅ vyřešeno | FR-25a: sync přes `CaptureScreenshotAsTexture` + `EncodeToPNG` |
| ~~`batch` transakčnost~~ | ✅ vyřešeno | FR-29b: batch = jeden Undo group; dry_run pro safe preview |
| ~~`read_file` bezpečnost~~ | ✅ vyřešeno | NFR-07 sandbox |
| ~~Array element property path~~ | ✅ vyřešeno | v3.0 implementace ověřila funkčnost |

---

## 12. Migration z v3.0

Existující commandy z v3.0 zůstávají kompatibilní. Změny pro Jana:
- `ucaf_config.json` má novou sekci `protocol_version: "4.0"`, `auto_rollback_on_compile_error: false`, `transport: "pipe|file|auto"`
- Test soubory se píšou do `Assets/Tests/` (vytvořit pokud neexistuje)
- Allowlist `ucaf_workspace/menu_allowlist.json` — defaultní safe set, Jan upraví podle potřeby
- Audit log `ucaf_workspace/logs/audit_*.jsonl` — periodicky archivovat / mazat

Claude přepne automaticky na v4.0 commands po update CLAUDE.md (musí být regenerováno z FR-58 capability listu).
