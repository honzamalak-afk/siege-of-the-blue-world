# UCAF — Unity Claude Assisted Framework
## Technická specifikace v4.2-EF

---

## 1. Architektura

UCAF je file-based IPC protokol mezi Claude a Unity Editorem. Unity Editor spouští `UCAF_Listener` každých 500 ms, který čte soubory z fronty, vykonává příkazy a zapisuje výsledky.

```
Claude (ty)                         Unity Editor
    │                                     │
    │  píše JSON do pending/              │
    │ ──────────────────────────────────► │
    │                                     │  Poll() každých 500ms
    │                                     │  ExecuteCommand(cmd)
    │                          píše JSON  │
    │ ◄──────────────────────────────────  │
    │  čte výsledek z done/               │
```

### Soubory a složky

```
ucaf_workspace/
├── commands/
│   ├── pending/       ← Claude sem píše příkazy (.json)
│   ├── done/          ← Unity sem zapisuje výsledky (.json)
│   └── errors/        ← Unity sem zapisuje neočekávané chyby
├── screenshots/       ← PNG screenshoty (take_screenshot, screenshot_after)
├── streams/           ← NDJSON streamy konzole (console_subscribe)
├── recordings/        ← PNG sequence záznamy (recording_start)
├── sessions/frames/   ← extrahované framy (recording_extract_frames)
├── schemas/           ← JSON schémata pro validaci příkazů
├── logs/buffer.ndjson ← log buffer Unity konzole
└── memory/
    ├── bugs.ndjson    ← bug ledger záznamy
    └── bugs_index.json
```

---

## 2. Formáty zpráv

### Příkaz (pending/*.json)

```json
{
  "id": "muj-prikaz-001",
  "type": "nazev_prikazu",
  "timestamp": "2026-04-27T10:00:00Z",
  "params_list": [
    { "key": "param1", "value": "hodnota1" },
    { "key": "param2", "value": "hodnota2" }
  ]
}
```

- `id` — libovolný unikátní řetězec; výsledek se zapíše do `done/{id}.json`
- `type` — název příkazu (viz sekce 4)
- `params_list` — všechny hodnoty jsou stringy; příkaz si je parsuje sám

### Výsledek (done/*.json)

```json
{
  "success": true,
  "message": "Popis výsledku",
  "screenshot_path": "",
  "data_json": "{...}"
}
```

- `success` — `true`/`false`
- `message` — krátký popis výsledku nebo chyby
- `screenshot_path` — vyplněno pokud byl použit `screenshot_after` nebo `take_screenshot`
- `data_json` — JSON string s detailními daty (parsuj jako vnitřní objekt)

### Cross-cutting parametr: `screenshot_after`

Jakýkoliv příkaz může dostat `"screenshot_after": "true"` (nebo `"scene"`/`"game"`). Pokud příkaz uspěje, Unity připojí screenshot scény do výsledku.

---

## 3. Synchronní vs. asynchronní příkazy

Většina příkazů je **synchronní** — Unity vykoná příkaz a okamžitě zapíše do `done/`.

**Asynchronní příkazy** vracejí `null` z dispatcheru, samy si zápisují výsledek, a přežívají domain reload pomocí `SessionState`:

| Příkaz | Čeká na |
|--------|---------|
| `compile_and_wait` | konec kompilace + domain reload |
| `take_screenshot` | rendering frame |
| `add_package` / `remove_package` / `update_package` | Package Manager resolution |
| `set_build_target` | přepnutí build platformy |
| `playmode_enter` / `playmode_exit` / `playmode_run` | vstup/výstup z Play Mode |
| `run_tests` / `list_tests` | TestRunner |
| `profiler_snapshot` / `profiler_record` | sběr dat profileru |

Async příkazy **nelze vkládat do `batch`** — způsobí chybu sub-příkazu.

---

## 4. Přehled příkazů

### 4.1 Scéna

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `create_scene` | Vytvoří novou scénu | `name`, `path` |
| `open_scene` | Otevře existující scénu | `path` |
| `save_scene` | Uloží aktivní scénu | — |
| `list_scene` | Vrátí strom objektů ve scéně | — |

### 4.2 Objekty

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `create_object` | Vytvoří GameObject | `name`, `parent_path`, `position`, `tag`, `layer` |
| `modify_object` | Změní vlastnosti objektu | `path` nebo `name`, pak `position`/`rotation`/`scale`/`active`/`tag`/`layer` |
| `delete_object` | Smaže objekt | `path` nebo `name` |
| `reparent_object` | Přesune v hierarchii | `path`, `new_parent_path` |
| `duplicate_object` | Zduplikuje objekt | `path` |
| `find_objects` | Najde objekty dle filtru | `component_type`, `tag`, `layer`, `name_contains`, `include_inactive` |
| `get_object_info` | Detail objektu | `path` nebo `name` |
| `select_object` | Vybere objekt v editoru | `path` |
| `focus_scene_view` | Zaměří scene view | `path` |

### 4.3 Komponenty a inspektor

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `add_component` | Přidá komponentu | `path`, `component_type` |
| `remove_component` | Odebere komponentu | `path`, `component_type` |
| `list_components` | Vypíše komponenty objektu | `path` |
| `list_fields` | Vypíše serializovaná pole | `path`, `component` |
| `get_field` | Čte hodnotu pole | `path`, `component`, `field` |
| `set_field` | Nastaví hodnotu pole | `path`, `component`, `field`, `value` |
| `append_array_element` | Přidá prvek do array | `path`, `component`, `field`, `value` |

### 4.4 Skripty a kompilace

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `create_script` | Vytvoří C# soubor | `class_name`, `content`, `folder` |
| `edit_file` | Upraví existující soubor | `path`, `mode` (`replace`/`anchor`), `old_string`, `new_string`, **`compile=true`** |
| `attach_script` | Připojí skript k objektu | `path`, `class_name` |
| `compile_and_wait` | Zkompiluje a čeká na výsledek | `timeout` (def. 45s), **`verbose=true`** |
| `compile_check` | Spustí kompilaci fire-and-forget | — |
| `asset_refresh` | Obnoví AssetDatabase | — |
| `read_file` | Přečte obsah souboru | `path` |
| `delete_file` | Smaže soubor | `path`, `confirm=true` |
| `find_assets_by_content` | Grep přes Assets | `pattern`, `glob`, `max` |

#### Pravidlo: edit_file vždy s compile=true

```json
{
  "id": "edit-001",
  "type": "edit_file",
  "params_list": [
    { "key": "path",       "value": "Assets/Scripts/PlayerController.cs" },
    { "key": "mode",       "value": "replace" },
    { "key": "old_string", "value": "void Update() {" },
    { "key": "new_string", "value": "void Update() { // fixed" },
    { "key": "compile",    "value": "true" }
  ]
}
```

Sloučí edit + compile do jediného async výsledku. Nikdy neposílej `edit_file` bez `compile=true` jako samostatný krok před `compile_and_wait`.

#### compile_and_wait: výsledek

```json
{
  "has_errors": false,
  "error_count": 0,
  "warning_count": 12,
  "errors": [],
  "warnings": [ ... ],
  "elapsed_ms": 2300
}
```

- `warnings` defaultně obsahuje **max 3 vzorky** + celkový `warning_count`. Pro plný seznam: `"verbose": "true"`.

### 4.5 Prefaby

| Příkaz | Popis |
|--------|-------|
| `create_prefab` | Uloží objekt jako prefab |
| `apply_prefab` | Aplikuje overrides na prefab asset |
| `revert_prefab` | Vrátí overrides na instanci |

### 4.6 Materiály a assety

| Příkaz | Popis |
|--------|-------|
| `create_material` | Vytvoří materiál |
| `assign_material` | Přiřadí materiál objektu |
| `set_material_prop` | Nastaví property materiálu |
| `create_scriptable` | Vytvoří ScriptableObject |
| `list_scriptables` | Vypíše ScriptableObjects |
| `list_assets` | Vypíše assety dle glob vzoru |
| `import_asset` | Importuje .unitypackage |

### 4.7 Konzole a logování

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `get_console` | Čte log buffer | **`since`** (povinné), `severity`, `max` |
| `clear_console` | Vymaže log buffer | — |
| `console_subscribe` | Spustí NDJSON stream do souboru | `level`, `pattern`, `from_assembly` |
| `console_unsubscribe` | Zastaví stream | `subscription_id` |

#### Pravidlo: get_console vždy s since

```json
{
  "id": "console-001",
  "type": "get_console",
  "params_list": [
    { "key": "since",    "value": "2026-04-27T10:05:00Z" },
    { "key": "severity", "value": "error" }
  ]
}
```

`since` je **schématem označeno jako required** — bez něj vrátí celý buffer (stovky záznamů).

#### console_subscribe výsledek

```json
{ "subscription_id": "sub-abc123", "stream_path": "ucaf_workspace/streams/sub-abc123.ndjson" }
```

Stream je živý NDJSON soubor — čti ho lineárně jako append-only log.

### 4.8 Screenshoty

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `take_screenshot` | Zachytí snímek | `view` (`scene`/`game`), `width`, `height` |

`take_screenshot` je async — výsledek obsahuje `screenshot_path`.

### 4.9 Play Mode

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `play_mode` | Jednoduché enter/exit | `enter=true/false` |
| `playmode_enter` | Enter s čekáním | `timeout` |
| `playmode_exit` | Exit s čekáním | `timeout` |
| `playmode_run` | Enter → čekej na podmínku → Exit | `wait_for_seconds`, `wait_for_signal`, `record_video`, `timeout` |
| `playmode_runtime_get` | Čte pole runtime objektu | `path`, `component`, `field` |
| `playmode_runtime_set` | Nastaví pole runtime objektu | `path`, `component`, `field`, `value` |
| `playmode_runtime_call` | Zavolá metodu runtime objektu | `path`, `component`, `method`, `args_json` |
| `playmode_signal_subscribe` | Naslouchá signálům z kódu | `signal_name` |

#### playmode_run s automatickým záznamem

```json
{
  "id": "run-001",
  "type": "playmode_run",
  "params_list": [
    { "key": "wait_for_seconds", "value": "5" },
    { "key": "record_video",     "value": "true" }
  ]
}
```

Výsledek obsahuje `console_stream_path` (auto-subscribed NDJSON) a `recording_session_id`.

### 4.10 Test Runner

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `run_tests` | Spustí testy | `mode` (`EditMode`/`PlayMode`), `filter`, `assembly` |
| `list_tests` | Vypíše dostupné testy | `mode` |
| `create_test` | Vytvoří test soubor | `class_name`, `folder`, `mode` |
| `register_test_assembly` | Zaregistruje assembly | `name`, `folder`, `mode` |

### 4.11 Vstup (Input simulation)

| Příkaz | Popis |
|--------|-------|
| `input_press_key` | Stiskne klávesu |
| `input_move_mouse` | Pohyb myší |
| `input_gamepad_stick` | Gamepad stick |
| `input_sequence` | Sekvence vstupů |

Funguje pouze v Play Mode.

### 4.12 Import nastavení assetů

| Příkaz | Popis |
|--------|-------|
| `set_texture_import` | Nastavení textury (compression, sRGB, …) |
| `set_model_import` | Nastavení modelu (normals, scale, …) |
| `set_animation_clip_import` | Nastavení animačního klipu (loopTime, …) |
| `set_audio_import` | Nastavení audia (compression, …) |
| `validate_imports` | Ověří import nastavení dle pravidel |

### 4.13 Package Manager

| Příkaz | Popis | Poznámka |
|--------|-------|----------|
| `list_packages` | Vypíše nainstalované balíčky | sync |
| `search_packages` | Hledá v registru | sync |
| `add_package` | Nainstaluje balíček | **async** |
| `remove_package` | Odinstaluje balíček | **async** |
| `update_package` | Aktualizuje balíček | **async** |

### 4.14 Project Settings

| Příkaz | Popis |
|--------|-------|
| `add_tag` / `add_layer` | Přidá tag nebo layer |
| `set_physics_collision` / `set_physics2d_collision` | Collision matrix |
| `set_quality_setting` | Quality level nastavení |
| `set_graphics_setting` | Graphics API, color space… |
| `set_input_axis` / `add_input_action` | Legacy Input / New Input System |
| `set_build_scenes` | Scény v build settings |
| `set_build_target` | Přepne build platformu (**async**) |

### 4.15 Git

| Příkaz | Popis |
|--------|-------|
| `git_status` | Status repo |
| `git_diff` | Diff (staged + unstaged) |
| `git_commit` | Commit s message |
| `git_branch` | Vypíše / vytvoří větve |

### 4.16 Animator Controller

| Příkaz | Popis |
|--------|-------|
| `create_animator_controller` | Vytvoří .controller asset |
| `add_animator_state` | Přidá stav |
| `add_animator_transition` | Přidá přechod s podmínkami |
| `add_animator_parameter` | Přidá parametr |
| `add_blend_tree` | Přidá blend tree do stavu |
| `set_animator_layer` | Nastaví vrstvu |
| `list_animator_states` | Vypíše stavy, přechody, parametry |
| `validate_animator` | Zkontroluje konzistenci |

### 4.17 Profiler

| Příkaz | Popis | Poznámka |
|--------|-------|----------|
| `profiler_snapshot` | Snímek metrik (FPS, MS, Memory…) | **async** |
| `profiler_record` | Nahrávání po N framů | **async** |
| `profiler_compare` | Porovná dva snapshoty | sync |

### 4.18 Timeline a Cinemachine

| Příkaz | Popis |
|--------|-------|
| `create_timeline` | Vytvoří Timeline asset a PlayableDirector |
| `add_timeline_track` | Přidá track |
| `add_timeline_clip` | Přidá klip do tracku |
| `create_cinemachine_camera` | Vytvoří Virtual Camera |
| `set_vcam_property` | Nastaví vlastnost vcam |
| `cinemachine_dolly_path` | Vytvoří dolly dráhu |

### 4.19 ShaderGraph a VFX

| Příkaz | Popis |
|--------|-------|
| `get_shadergraph_properties` | Vypíše properties Shader Graphu |
| `set_shadergraph_property` | Nastaví default hodnotu |
| `get_shadergraph_info` | Info o grafu (passes, keywords…) |
| `set_vfx_property` / `get_vfx_properties` | VFX Graph properties na instanci |

### 4.20 Terrain

| Příkaz | Popis |
|--------|-------|
| `create_terrain` | Vytvoří Terrain objekt |
| `set_terrain_heightmap` | Nanese heightmap z PNG |
| `paint_terrain_layer` | Nanese terrain texturu |
| `add_tree_prototype` | Přidá strom prototyp |
| `paint_trees` | Rozmístí stromy |

### 4.21 Lightmap

| Příkaz | Popis | Poznámka |
|--------|-------|----------|
| `lightmap_bake` | Spustí bake | **async** |
| `lightmap_clear` | Smaže lightmapy | sync |

### 4.22 NavMesh

| Příkaz | Popis |
|--------|-------|
| `navmesh_bake` | Bake NavMeshe |
| `navmesh_query_path` | Dotaz na cestu A→B |
| `navmesh_sample_position` | Nejbližší bod na NavMeshi |
| `add_offmesh_link` | Přidá OffMesh Link |

### 4.23 Build Player

| Příkaz | Popis | Poznámka |
|--------|-------|----------|
| `build_player` | Sestaví hru | **async** |
| `run_player` | Spustí sestavenou hru | sync |

### 4.24 Záznam (Recording) — Fáze F

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `recording_start` | Spustí záznam PNG sekvence | `session_id`, `fps` (def. 30), `width`, `height` |
| `recording_stop` | Zastaví záznam | — |
| `recording_extract_frames` | Zkopíruje framy kolem eventů | `session_id`, `events` (JSON array), `context_frames` |
| `recording_list` | Vypíše nahrané sessions | — |
| `recording_delete` | Smaže session nebo staré nahrávky | `session_id` nebo `older_than_days`, `confirm=true` |

Záznam funguje **pouze v Play Mode**. Nejjednodušší cesta: `playmode_run record_video=true`.

### 4.25 Editor Preferences a Layout — Fáze E

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `set_editor_pref` | Nastaví EditorPrefs hodnotu | `key`, `value`, `type` (`string`/`int`/`float`/`bool`) |
| `get_editor_pref` | Čte EditorPrefs hodnotu | `key`, `type`, `default` |
| `apply_editor_layout` | Načte layout okna | `layout_path`, `confirm=true` |

### 4.26 JSON Schema a validace

| Příkaz | Popis |
|--------|-------|
| `get_command_schema` | Vrátí schema pro daný typ příkazu |
| `validate_command` | Validuje payload bez exekuce |

Schémata jsou v `ucaf_workspace/schemas/*.schema.json`. Pokud pro příkaz existuje soubor, **pre-dispatch validátor automaticky ověří** `required_params` a `enum_values` před vykonáním.

### 4.27 Bug Ledger

| Příkaz | Popis | Klíčové parametry |
|--------|-------|-------------------|
| `log_bug` | Zaznamená bug | `title`, `symptom`, `files`, `components` |
| `update_bug` | Aktualizuje záznam | `bug_id`, libovolná pole |
| `close_bug` | Uzavře bug | `bug_id`, `fix`, `fix_commits` |
| `get_bug` | Detail bugu | `bug_id` |
| `query_bugs` | Filtrovaný seznam | `status`, `tag`, `component`, `file` |
| `find_similar_bugs` | Hledá podobné bugy | `query` |
| `purge_bug` | Trvale smaže | `bug_id`, `confirm=true` |

### 4.28 Ostatní

| Příkaz | Popis |
|--------|-------|
| `ping` | Test spojení (vrací "pong") |
| `protocol_capabilities` | Verze protokolu a schopnosti |
| `set_lighting` | Mlha, ambient, directional light |
| `execute_menu_item` | Spustí Unity menu item |
| `batch` | Spustí N příkazů v jednom souboru |

---

## 5. Batch — více příkazů najednou

Batch je **efektivní způsob** jak odeslat více nezávislých příkazů. Async příkazy (`compile_and_wait`, `take_screenshot` atd.) uvnitř batche **nejsou podporovány**.

```json
{
  "id": "batch-001",
  "type": "batch",
  "params_list": [
    { "key": "payload", "value": "{\"stop_on_error\":false,\"commands\":[{\"id\":\"b1\",\"type\":\"save_scene\",\"params_list\":[]},{\"id\":\"b2\",\"type\":\"git_status\",\"params_list\":[]}]}" }
  ]
}
```

Výsledek `data_json` obsahuje `UCAFBatchResult` se sub-výsledky pro každý příkaz.

---

## 6. JSON Schema soubory

Soubory v `ucaf_workspace/schemas/` definují validační pravidla. Pokud soubor existuje, validátor zkontroluje příkaz před vykonáním.

Existující schémata:
- `add_package.schema.json`
- `console_subscribe.schema.json`
- `get_console.schema.json` — `since` je required
- `recording_start.schema.json`
- `set_editor_pref.schema.json`

Formát schématu:

```json
{
  "command_type": "nazev_prikazu",
  "description": "...",
  "required_params": [
    { "key": "param1", "type": "string", "description": "...", "enum_values": [] }
  ],
  "optional_params": [
    { "key": "param2", "type": "enum", "description": "...", "default_value": "a", "enum_values": ["a","b","c"] }
  ]
}
```

---

## 7. Efektivní práce — klíčová pravidla

### Pravidlo 1: edit_file compile=true vždy

Nikdy neposílej dvě zprávy (`edit_file` + `compile_and_wait`). Jeden příkaz = jeden výsledek.

### Pravidlo 2: batch pro ≥ 2 nezávislé příkazy

Pokud víš, že budeš posílat více příkazů, které na sobě nezávisí, dej je do `batch`.

### Pravidlo 3: get_console vždy s since

Bez `since` dostaneš celý buffer (stovky záznamů). Pamatuj si `timestamp` posledního záznamu a předávej ho.

### Pravidlo 4: compile_and_wait výchozí chování potlačuje warnings

Výsledek defaultně obsahuje max 3 warning vzorky + celkový počet. Pro plný seznam předej `verbose=true`. Toto je záměrné — warnings z celé session jsou akumulovány v log souboru.

---

## 8. Typické workflow recepty

### Oprava C# chyby

```json
// 1. Uprav soubor a kompiluj v jednom kroku
{ "id": "fix-001", "type": "edit_file", "params_list": [
    { "key": "path",       "value": "Assets/Scripts/Player.cs" },
    { "key": "mode",       "value": "replace" },
    { "key": "old_string", "value": "badCode()" },
    { "key": "new_string", "value": "goodCode()" },
    { "key": "compile",    "value": "true" }
]}
// Počkej na done/fix-001.json → zkontroluj has_errors
```

### Přidání nového skriptu

```json
// 1. Vytvoř soubor
{ "id": "new-001", "type": "create_script", "params_list": [
    { "key": "class_name", "value": "EnemyAI" },
    { "key": "folder",     "value": "Scripts/AI" },
    { "key": "content",    "value": "public class EnemyAI : MonoBehaviour { }" }
]}
// 2. Zkompiluj (async)
{ "id": "cmp-001", "type": "compile_and_wait", "params_list": [] }
// 3. Připoj ke gameobjectu
{ "id": "att-001", "type": "attach_script", "params_list": [
    { "key": "path",       "value": "Enemy/Boss" },
    { "key": "class_name", "value": "EnemyAI" }
]}
```

### Test gameplay sekvence s logováním

```json
// 1. Spusť Play Mode, zaznamenej konsoli, zaznamenej video
{ "id": "run-001", "type": "playmode_run", "params_list": [
    { "key": "wait_for_seconds", "value": "10" },
    { "key": "record_video",     "value": "true" }
]}
// Výsledek obsahuje console_stream_path a recording_session_id

// 2. Přečti konzolní chyby (po skončení run)
{ "id": "log-001", "type": "get_console", "params_list": [
    { "key": "since",    "value": "2026-04-27T10:00:00Z" },
    { "key": "severity", "value": "error" }
]}
```

### Nastavení textury

```json
{ "id": "tex-001", "type": "set_texture_import", "params_list": [
    { "key": "path",        "value": "Assets/Textures/terrain_diffuse.png" },
    { "key": "compression", "value": "DXT1" },
    { "key": "sRGB",        "value": "true" },
    { "key": "max_size",    "value": "2048" }
]}
```

### Instalace balíčku a ověření

```json
// 1. Nainstaluj (async)
{ "id": "pkg-001", "type": "add_package", "params_list": [
    { "key": "name", "value": "com.unity.cinemachine" }
]}
// Počkej na done/pkg-001.json

// 2. Ověř
{ "id": "pkg-002", "type": "list_packages", "params_list": [] }
```

---

## 9. Domain reload a SessionState

Při kompilaci Unity provede **domain reload** — všechny statické proměnné se resetují. UCAF přežívá domain reload díky:

- `SessionState` — přežívá domain reload v rámci jedné Editor session
- `UCAF_Listener` se automaticky re-registruje pomocí `[InitializeOnLoad]`
- Pending async operace jsou detekovány v CheckPending* metodách na startup

Po domain reloadu pokračuje listener normálně. Probíhající `compile_and_wait` se automaticky dokončí.

---

## 10. Implementace

| Soubor | Obsah |
|--------|-------|
| `UCAF_Listener.cs` | Hlavní polling smyčka, dispatch, WriteResult |
| `UCAF_Types.cs` | Všechny datové typy |
| `UCAF_Tools.cs` | Pomocné utility (FindType, sandbox, …) |
| `UCAF_Listener.Scene.cs` | scene_* příkazy |
| `UCAF_Listener.Inspector.cs` / `InspectorV3.cs` | add_component, set_field, … |
| `UCAF_Listener.Scripting.cs` | compile_and_wait, edit_file setup |
| `UCAF_Listener.EditFile.cs` | edit_file logika |
| `UCAF_Listener.Batch.cs` | batch |
| `UCAF_Listener.Console.cs` | get_console, clear_console |
| `UCAF_Listener.ConsoleStream.cs` | console_subscribe, console_unsubscribe |
| `UCAF_Listener.Schema.cs` | ValidateAgainstSchema, get_command_schema |
| `UCAF_Listener.EditorPrefs.cs` | set_editor_pref, apply_editor_layout |
| `UCAF_Listener.Recording.cs` | recording_* příkazy |
| `UCAF_Listener.PlayMode2.cs` | playmode_run, playmode_enter/exit |
| `UCAF_Listener.PackageManager.cs` | add_package, remove_package, … |
| `UCAF_Listener.AssetImport.cs` | set_texture_import, validate_imports, … |
| `UCAF_Listener.BuildSettings.cs` | set_build_target, git_* |
| `UCAF_Listener.ProjectSettings.cs` | add_tag, set_physics_collision, … |
| `UCAF_Listener.AnimatorController.cs` | create_animator_controller, … |
| `UCAF_Listener.BugLedger.cs` | log_bug, close_bug, … |
| `UCAF_Listener.Profiler.cs` | profiler_snapshot, profiler_record |
| `UCAF_Listener.Timeline.cs` | create_timeline, add_timeline_track, … |
| `UCAF_Listener.NavMesh.cs` | navmesh_bake, … |
| `UCAF_Listener.Lightmap.cs` | lightmap_bake, … |
| `UCAF_Listener.BuildPlayer.cs` | build_player, run_player |
| `UCAF_Listener.TestRunner.cs` | run_tests, list_tests, … |
| `UCAF_Listener.Input.cs` | input_press_key, … |
| `UCAF_Listener.ShaderGraph.cs` | get_shadergraph_properties, … |
| `UCAF_Listener.Terrain.cs` | create_terrain, … |
| `UCAF_Listener.Misc.cs` | set_lighting, play_mode, … |
| `UCAF_Listener.Screenshot.cs` | take_screenshot |
| `UCAF_Listener.Query.cs` | find_objects, get_object_info |
| `UCAF_Listener.CodeGrep.cs` | find_assets_by_content |
| `UCAF_Listener.Files.cs` | read_file, delete_file |

---

*Protokol: 4.2-EF | Unity 6 + Recorder 5.1.6 | Siege of the Blue World*
