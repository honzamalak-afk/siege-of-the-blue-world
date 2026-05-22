# PRD – Unity + Claude Code AI-Assisted Framework
**Version:** 4.4
**Author:** Jan Malák
**Date:** 2026-05-21
**Status:** Delta IMPLEMENTOVÁNA jako UCAF package v1.1.0 — commit `9f65073` na lokálním clone v `/workspace/_ucaf_dev/`. Fáze H-závislé položky (FR-221, FR-222) odloženy do Fáze H z v4.3.
**Předchozí verze:** PRD_Unity_Claude_Framework_v4_3.md (v4.3, stále platná pro Fáze A–J)

## Implementační status v4.4

| FR | Command/feature | Status |
|----|-----------------|--------|
| FR-219 | `profiler_diagnose` | ✅ DONE — built-in 11 perf patterns |
| FR-220 | `console_diagnose` | ✅ DONE — built-in 20 console patterns |
| FR-220a | `diagnose_list_patterns` | ✅ DONE |
| FR-221 | `generate_cubemap` | ❌ blokováno Fází H (nezačala) |
| FR-222 | `generate_terrain_layer` | ❌ blokováno Fází H |
| FR-223 | AI-metadata utility + `list_generated_assets` | ✅ DONE — utility ready, scanner aktivní |
| FR-224 | `get_build_context` | ✅ DONE |
| FR-225 | `mcp_server_start` | ✅ DONE — TCP JSON-RPC, MCP-style semantics |
| FR-226 | `mcp_server_stop` | ✅ DONE |
| FR-227 | `mcp_server_status` | ✅ DONE + extended `protocol_capabilities` |
| FR-228 | `read_only=true` cross-cutting | ✅ DONE — hardcoded whitelist |

**Total commit:** 19 souborů, +1844 řádků C# + JSON katalogů.

**Soubory v balíčku v1.1.0 navíc proti v1.0.0:**
- `Editor/UCAF_Listener.BuildContext.cs`
- `Editor/UCAF_Listener.ReadOnly.cs`
- `Editor/UCAF_Listener.Diagnose.cs`
- `Editor/UCAF_Listener.AIMetadata.cs`
- `Editor/UCAF_Listener.MCPServer.cs`
- `Editor/UCAF_Types.v44.cs`
- `Editor/diagnose/perf_patterns.json`
- `Editor/diagnose/console_patterns.json`

**Edits do existujících souborů:**
- `Editor/UCAF_Listener.cs` — read_only check před schema validací; nové case statements v dispatcheru; extended `protocol_capabilities`; bump startup log na v4.4
- `package.json` — version 1.0.0 → 1.1.0

> Tento dokument NENAHRAZUJE v4.3 — pouze přidává delta. Vše, co není zmíněno, platí dle v4.3.

---

## 0. Co je nového oproti v4.3

| Oblast | Změna |
|--------|-------|
| **4.51** | Nová sekce: Diferenciace UCAF vs Unity AI (strategická pozice) |
| **4.52** | Nová sekce: Dosud nedokumentované existující moduly (Batch, BugLedger, CodeGrep, EditFile, Files, Query, InspectorV3, Screenshot, ExecuteMenuItem, ProtocolCapabilities) |
| **4.53** | Rozšíření Fáze D o diagnose vrstvu (FR-219, FR-220) — strukturované hinty nad raw daty |
| **4.54** | Rozšíření Fáze H o cubemap, terrain layer a AI-metadata tagging (FR-221, FR-222, FR-223) |
| **4.55** | Nová: `get_build_context` (FR-224) — discovery target platformy + render pipeline pro Claude kontext |
| **4.56** | Fáze K (nová): MCP transport layer jako paralelní IPC kanál (FR-225, FR-226, FR-227) |
| **4.57** | Cross-cutting `read_only=true` — Ask/Agent parita bez modifikujících operací |
| **4.58** | Errata pro v4.3 (FR-192 kolize, duplicitní 4.44, sekce 7 status) |
| **9** | Aktualizovaný Out of Scope se zdůvodněním proč ne (Sentis, chat UI, AI Gateway, cloud) |
| **11** | Stav v4.3 → v4.4 tabulka |

**Co jsem ZÁMĚRNĚ neaplikoval z Unity AI** (kolidovalo by s UCAF logikou):
- Conversational chat UI v Editoru — rozbíjí file-based reproducibilitu (commandy musí zůstat ukládatelné a replay-ovatelné JSON soubory)
- Cloud Dashboard / credit billing — UCAF je offline-first, žádný vendor account
- AI Gateway (multi-LLM switching uvnitř Unity) — UCAF je LLM-agnostický bez vlastní mediace; volba modelu je odpovědnost klienta (Claude Code, Cursor, atd.), ne UCAF
- "Project-wide trained on 20+ let docs" semantic context — to je vlastnost LLM, ne nástroje; UCAF dodává fakta, ne znalost
- Sentis / runtime ML inference — jiná doména (game runtime, ne dev tooling); zůstává v Out of Scope, ale s explicitním zdůvodněním
- 3D object a animation generation z text promptu — zůstává v Out of Scope s **kritérii re-evaluace** místo trvalého zamítnutí

---

## 4.51 Diferenciace UCAF vs Unity AI

Tato sekce explicitně pojmenovává strategickou pozici UCAF. Důvod: po Fázích G–J bude funkční překryv velký a je nutné mít jasno, **proč UCAF dál existuje** vedle Unity AI.

### 4.51.1 Co UCAF dělá a Unity AI ne (a nebude)

| Schopnost | UCAF | Důvod proč Unity AI nebude mít |
|-----------|------|--------------------------------|
| Plná verifikační smyčka (compile → playmode → input simulation → runtime introspekce → exit → assert) | ✅ Fáze B | Unity AI Agent mode dělá akci a vrací výsledek, nepouští playmode automatizovaně |
| PlayMode runtime introspekce (`playmode_runtime_get/set/call`, Signal subscribe, predicate eval) | ✅ | Unity AI nedeklaruje runtime API |
| Test Runner integrace (run_tests, list_tests, create_test, register_test_assembly) | ✅ | Unity AI je conversational nástroj, ne CI driver |
| Input simulace (keyboard/mouse/gamepad/sequence) | ✅ | Mimo scope Unity AI |
| Video recording + frame extraction pro vizuální bugy | ✅ Fáze F | Mimo scope Unity AI |
| Build Player & run | ✅ | Mimo scope Unity AI |
| Reproducibilní JSON commandy (commit-ovatelné, replay-ovatelné) | ✅ | Conversational chat je z definice ne-deterministický |
| Headless / CI použitelnost | ✅ | Unity AI vyžaduje GUI Editor + Dashboard auth |
| Offline provoz, žádný cloud, žádné credity | ✅ | Unity AI je cloud-bound by design |
| LLM-agnostický backend (Claude, Cursor, libovolný IDE agent) | ✅ | Unity AI je Unity-managed gateway |
| Git operace integrované do workflow | ✅ | Mimo scope Unity AI |
| Bug Ledger (perzistentní bug tracking) | ✅ | Mimo scope Unity AI |

### 4.51.2 Kde Unity AI vede a UCAF to úmyslně nedohání

| Schopnost | Unity AI | UCAF rozhodnutí |
|-----------|----------|-----------------|
| 3D object generation | ✅ Generators | Out of Scope dokud externí API nedosáhne produkční kvality (re-evaluace 2027 H1) |
| Animation clip generation | ✅ Generators | Out of Scope dokud externí API nedosáhne produkční kvality |
| Sentis runtime ML | ✅ | Out of Scope — jiná doména |
| In-Editor chat panel | ✅ | Out of Scope — Claude Code / IDE = frontend, UCAF = backend |
| Cloud Dashboard, credit billing | ✅ | Out of Scope — offline-first princip |
| AI Gateway (LLM switching) | ✅ | Out of Scope — volba LLM je odpovědnost klienta |

### 4.51.3 Kde UCAF doženě Unity AI (Fáze G–J + tato delta)

Po dokončení Fáze G+H+I+J + delta v4.4 bude UCAF mít:
- Editor GUI automation (Fáze G)
- Texture / sprite / SFX / music generation + cubemap + terrain layer (Fáze H + delta)
- Plan/Approval workflow (Fáze I)
- Scene from image (Fáze I)
- Skills architecture (Fáze J)
- Performance & console diagnose hints (delta 4.53)
- MCP transport pro IDE interop (delta 4.56)
- Read-only Ask mode (delta 4.57)

**Roadmapový závěr:** UCAF se nesnaží být lepší Unity AI. UCAF je *test-driven dev tool* s podmnožinou autoringových schopností Unity AI plus celá testovací smyčka, kterou Unity AI nemá.

---

## 4.52 Existující moduly mimo PRD v4.3 (errata)

Tyto moduly jsou implementovány v `com.ucaf@1.0.0`, ale PRD v4.3 sekce 3.4 je neuvádí. Tato sekce je doplňuje retroaktivně.

| Soubor | Commandy | Účel |
|--------|----------|------|
| `UCAF_Listener.Batch.cs` | `batch` | Sekvenční exekuce listu commandů v jednom payloadu — řeší atomicitu vícestupňových operací (např. create_object + set_field + add_component). |
| `UCAF_Listener.BugLedger.cs` | `log_bug`, `update_bug`, `query_bugs`, `find_similar_bugs`, `get_bug`, `close_bug`, `purge_bug` | Perzistentní bug tracking (project CLAUDE.md to vyžaduje "po každém vyřešeném bugu"). Sekce 4.45 v v4.3 zmiňuje "kategorie", ale ne kompletní command set. |
| `UCAF_Listener.CodeGrep.cs` | `find_assets_by_content` | Grep nad textovými assety / scripty. PRD ho zmiňuje jen jako prerekvizitu Plan mode (FR-206) bez vlastního FR. |
| `UCAF_Listener.EditFile.cs` | `edit_file` (operace `anchor`/`patch`/`replace`) | Inkrementální editace souborů bez full rewrite. PRD ho zmiňuje jen v Risks jako fallback pro Animator. |
| `UCAF_Listener.Files.cs` | `read_file`, `delete_file` | Obecné file I/O omezené na `Assets/`, `ucaf_workspace/`, `ProjectSettings/`. |
| `UCAF_Listener.Query.cs` | `find_objects`, `get_object_info` | Discovery v scéně — výpis objektů dle predikátu, detail objektu (komponenty + transform). |
| `UCAF_Listener.InspectorV3.cs` | `append_array_element` | Rozšíření Inspector bridge o pole — v4.3 sekce 4.5 popisuje set_field, ne pole. |
| `UCAF_Listener.Screenshot.cs` | `take_screenshot`, `select_object`, `focus_scene_view` | Vizuální feedback loop. PRD ho zmiňuje jen jako menu item. |
| `UCAF_Listener.cs` (dispatcher) | `execute_menu_item`, `protocol_capabilities` | `execute_menu_item` volá Unity menu (`EditorApplication.ExecuteMenuItem`). `protocol_capabilities` vrací seznam podporovaných commandů — discovery pro klienta. |

**FR-227 doplnění:** `protocol_capabilities` → vrátí seznam všech registrovaných commandů + jejich verzi. Důležité pro forward-compat (klient zjistí, co aktuální UCAF umí, místo guess-work z hardcoded katalogu). Pravděpodobně už existuje — tato sekce ho jen kanonizuje.

---

## 4.53 Rozšíření Fáze D — diagnose vrstva

**Záměr:** Unity AI nabízí "console error explanation" a "performance optimization advice" v přirozeném jazyce. UCAF tohle dělat nemůže (NL je v LLM, ne v UCAF), ale může poskytnout **strukturované hinty** nad raw daty z profileru a console — pattern-matching katalog častých problémů. Claude (jako frontend) si nad nimi pak napíše vysvětlení.

### FR-219 `profiler_diagnose`
- **Input:** `snapshot_id` (z `profiler_snapshot` ✅ existuje) nebo `before_id` + `after_id` (z `profiler_compare` ✅).
- **Output:** Seznam `UCAFPerfHint` `{category, severity, evidence_metric, evidence_value, threshold, suggestion_id}`.
- **Příklady hintů:**
  - `category=GC, evidence=alloc>1MB/frame, suggestion=check_string_concat_in_update_loop`
  - `category=DrawCalls, evidence=count>2000, suggestion=enable_batching_or_atlas_textures`
  - `category=ScriptTime, evidence=Update>5ms, suggestion=cache_GetComponent_calls`
- **Pattern catalog: 3-vrstvý lookup s precedencí** (rozhodnutí v4.4, sekce 13):
  1. **Built-in** v UCAF package — `com.ucaf/Editor/diagnose/perf_patterns.json`, ships s UCAF (~30 nejčastějších Unity perf patternů)
  2. **Workspace override** — `ucaf_workspace/diagnose/perf_patterns.json`, git-tracked, project-specific; entry s matching `id` override built-in, nové entries se appendnou
  3. **Inline** — volitelný request parametr `extra_patterns=[...]` pro one-off testy bez perzistence
- **Důvod, proč to není v LLM:** thresholdy musí být deterministické a verzované, ne závislé na halucinaci.

### FR-220 `console_diagnose`
- **Input:** `since` (ISO timestamp) nebo `subscription_id` z `console_subscribe` ✅.
- **Output:** Seznam `UCAFConsoleHint` `{log_entry_id, error_type, suggestion_id, links_to_docs[]}`.
- **Příklady:**
  - `NullReferenceException at X.Y.Z` → `suggestion=check_inspector_reference_assigned_for_field`
  - `MissingReferenceException` → `suggestion=object_destroyed_check_lifecycle`
  - `Shader X is not supported on this platform` → `suggestion=check_build_target_render_pipeline_compat`
- **Pattern catalog:** stejná 3-vrstvá architektura jako FR-219, soubory `console_patterns.json`.

### FR-220a `diagnose_list_patterns`
- **Output:** merged katalog (built-in + workspace + inline) s polem `source` (`builtin|workspace|inline`) na každém pattern entry. Důležité pro debug "proč můj custom pattern nezabral / co override-uje co".
- Žádný side-effect, pure read.

**Co tyto commandy NEDĚLAJÍ:** negenerují prosu psanou odpověď. Vrací jen `suggestion_id` + evidence. Claude si z toho udělá vysvětlení pro uživatele.

---

## 4.54 Rozšíření Fáze H — Unity AI-equivalent assety

**Záměr:** Unity AI Generators podporují cubemaps a terrain layers, které PRD v4.3 v Fázi H nemá. Tyto typy mají stejnou architekturu jako FR-199 `generate_texture` — jen jiný post-processing (cubemap konverze, layer asset wrap).

### FR-221 `generate_cubemap`
- `prompt`, `resolution=512|1024|2048`, `output_path=Assets/Textures/Cubemaps/...`, `projection=equirectangular|cube_faces`.
- Volá image API stejně jako FR-199, post-processuje na `TextureImporter.textureShape=Cube`.
- Použití: skyboxes, reflection probes.

### FR-222 `generate_terrain_layer`
- `prompt` (např. "moss covered cobblestone, top-down PBR"), `output_path=Assets/Terrain/Layers/...`.
- Generuje albedo + normal + roughness (stejně jako FR-200 PBR set), wrapuje do `TerrainLayer` assetu, nastaví `tileSize`, `tileOffset`.

### FR-223 AI-metadata tagging (cross-cutting pro Fázi H)
- Každý `generate_*` command rozšířen o **povinný** side-effect (žádný opt-out flag v API, rozhodnutí v4.4 sekce 13): zapsat `m_UserData` (Unity Editor metadata) na vytvořený asset s JSON markerem:
  ```json
  {
    "ucaf_generated": true,
    "ucaf_version": "1.0.0",
    "ucaf_command": "generate_texture",
    "ucaf_prompt": "...",
    "ucaf_api": "dalle3",
    "ucaf_timestamp": "2026-05-21T..."
  }
  ```
- **Důvod:** Unity AI dělá totéž (embedded metadata). Užitečné pro:
  - Audit: "které assety jsme vygenerovali, ne kreslili"
  - Legal: copyright / licensing audit
  - Cleanup: smazat všechny AI-gen draft assety před release buildem
- **Proč žádný opt-out:** audit/legal hodnota tagu vzniká právě 100% spolehlivostí. Opt-out by ji devaluoval. Escape hatch existuje na úrovni Unity (ruční edit `.meta`) — vědomá akce, ne silent skip.
- Nový pomocný command `list_generated_assets` (`folder=Assets/`, optional `since`) → vrátí všechny taggované.

---

## 4.55 Build context discovery

### FR-224 `get_build_context`
- **Input:** none.
- **Output:** `UCAFBuildContext`:
  ```json
  {
    "unity_version": "6000.4.3f1",
    "render_pipeline": "HDRP",
    "render_pipeline_version": "17.4.0",
    "build_target": "StandaloneWindows64",
    "build_target_group": "Standalone",
    "scripting_backend": "Mono2x",
    "api_compat_level": ".NETStandard2.1",
    "color_space": "Linear",
    "input_system": "new",
    "active_scene": "Assets/Scenes/SampleScene.unity"
  }
  ```
- **Důvod:** Unity AI dostává target platformu jako context automaticky. UCAF to musí explicitně exposovat, aby Claude před generováním shader / build / settings kódu věděl, na co míří, bez guess-work. Eliminuje class of bugs typu "Claude napsal URP shader v HDRP projektu".

---

## 4.56 Fáze K — MCP transport layer

**Záměr:** Unity AI exposuje commandy přes Model Context Protocol — standardizovaný protokol, který umí Cursor, Continue, VS Code agenti, atd. UCAF má vlastní file-based IPC, který je primární a zůstává. MCP server bude **paralelní transport** nad stejným command dispatcherem — `UCAF_Listener.Dispatch(UCAFCommand)` zůstává master, MCP je jen jiný způsob, jak commandy doručit.

**Architektonický princip:** žádná nová byznys logika, jen jiný transport. Reproducibilita zachována: MCP requesty se logují do `ucaf_workspace/mcp_log.ndjson` ve stejném formátu jako file-IPC commandy.

### Konfigurace v `ucaf_config.json` (rozhodnutí v4.4 sekce 13)

```json
{
  "mcp_server": {
    "enabled": false,
    "auto_start": true,
    "port": 7374,
    "bind": "127.0.0.1"
  }
}
```

- `enabled=false` (default) → MCP server vůbec neběží, žádný TCP port se neotevře. Stávající UCAF instalace nepocítí žádnou změnu.
- `enabled=true` + `auto_start=true` → server se spustí automaticky při načtení Editoru. MCP klient (Cursor, VS Code, Continue) dostane Unity hned, bez nutnosti volat `mcp_server_start` v každé session.
- `enabled=true` + `auto_start=false` → server musí být spuštěn explicitně přes `mcp_server_start`. Užitečné pro projekty, kde se MCP zapíná jen občas.
- `bind=127.0.0.1` (default) → pouze localhost. Remote přístup vyžaduje `bind=0.0.0.0` v configu + manuální confirm; nikdy automaticky.

### FR-225 `mcp_server_start`
- Runtime override configu — spustí server i když je v configu `enabled=false`.
- `port`, `bind` (default z configu, jinak 7374/127.0.0.1).
- Spustí MCP server v Editoru, vystaví všechny commandy z `protocol_capabilities` jako MCP tools.
- Async (čeká na první accept), vrací `{port, pid, transport_id}`.
- **Bezpečnost:** remote bind (`0.0.0.0`) vyžaduje `confirm=true`.

### FR-226 `mcp_server_stop`
- `transport_id` (z `mcp_server_start`) nebo nic (zastaví single running instanci).
- Ukončí server, dokončí běžící requesty.

### FR-227 `mcp_server_status`
- Vrátí stav: běží/neběží, port, počet zpracovaných requestů, last_error, `started_by=config|command`.

**Out of scope Fáze K:**
- MCP client (volání externích MCP serverů z UCAF) — UCAF je MCP server, ne klient
- MCP resource subscriptions — pouze tools (request/response)
- Auth/TLS — pouze localhost binding; pro remote setup odpovědný uživatel

**Závislost:** žádná Unity package. MCP protokol je JSON-RPC over stdio nebo TCP — implementovatelný v čistém C#.

**Kdy implementovat:** PO Fázích G+H+I+J. Fáze K je organizační vrstva, nemá samostatnou hodnotu bez pokrytí command katalogu.

---

## 4.57 Cross-cutting `read_only` parametr (Ask mode parita)

**Záměr:** Unity AI má separaci Ask mode (read-only) vs Agent mode (modifying). Užitečné pro:
- Bezpečné exploration / discovery bez rizika nechtěné modifikace
- Onboarding nového developera, který ještě nechce povolit modifikace
- CI / audit jobs, které jen kontrolují stav

### FR-228 (cross-cutting) `read_only=true`
- Volitelný parametr na libovolném commandu. **Jediný mechanismus** — žádný session/config default (rozhodnutí v4.4 sekce 13).
- Když `read_only=true`, dispatcher porovná command type proti **read-only whitelistu** (viz níže). Pokud command je modifikující, vrátí `UCAFErrorContext` s `error_code=read_only_mode_blocked` před exekucí (žádný partial state, NFR-26).
- Whitelist je hardcoded v dispatcheru (ne externí soubor — žádná konfigurace, kterou by uživatel musel udržovat). Default obsahuje všechny `get_*`, `list_*`, `find_*`, `query_*`, `validate_*`, `ping`, `protocol_capabilities`, `compile_check`, `console_subscribe`, `console_unsubscribe`, `get_console`, `take_screenshot`, `get_build_context`, `diagnose_list_patterns`, `mcp_server_status`.
- **CI / audit use case** — klient (wrapper script) přidává `read_only=true` ke všem voláním. UCAF backend zůstává jednoduchý, žádný session state.

**Důvod, proč ne `mode=ask|agent` enum:** parametr `read_only=true|false` je jasnější, neguje single concern, a nemusí být na začátku všech operací.

**Důvod, proč ne session default:** přidává druhý code path (default + override semantika), který se musí testovat a dokumentovat. Pokud emerguje real demand, lze přidat aditivně později bez breaking change.

---

## 4.58 Errata pro v4.3

Tato sekce nahrazuje nebo opravuje konkrétní body v PRD v4.3.

| Errata | Co opravit |
|--------|-----------|
| **E-1: FR-192 kolize** | V v4.3 je `search_packages` (sekce 4.35) **i** `ui_click` (sekce 4.47) jako FR-192. Nové číslování pro Fázi G: `ui_click`→**FR-292**, `ui_set_field`→**FR-293**, `ui_drag_asset`→**FR-294**, `ui_open_window`→**FR-295**, `ui_get_element_state`→**FR-296**, `ui_list_elements`→**FR-297**, `ui_computer_use_fallback`→**FR-298**. `search_packages` zůstává FR-192. |
| **E-2: Duplicitní 4.44** | V v4.3 jsou dvě sekce "4.44" — pro Recording (FR-186–191) a Editor Preferences (FR-183–185). Recording sekce přečíslovat na **4.44a Video záznam**, Editor Prefs na **4.44b Editor Preferences**. |
| **E-3: Sekce 7 nekonzistence** | Sekce "Implementation Plan" v v4.3 uvádí Fáze C/D/E jako "❌ TODO", ale sekce 0 a 3.4 je značí ✅ DONE. Sekce 7 by měla odrážet skutečný stav (= sekce 0). |
| **E-4: Out of Scope dual statement** | "Asset Store integrace" v sekci 9 — pouze placené balíčky. Zdarma `.unitypackage` import je v sekci `import_asset` (Misc), což platí. Vyjasnit v sekci 9. |
| **E-5: Backlog `instantiate_prefab`** | Sekce 12.5 — backlog položka. Doporučení: povýšit do FR-229 a implementovat ve stejné fázi jako runtime introspekce (Fáze B rozšíření), protože je runtime-only a logicky tam patří. |

---

## 5. Non-Functional Requirements — delta

| NFR | Požadavek | Status |
|-----|-----------|--------|
| NFR-23 | `profiler_diagnose` / `console_diagnose` výsledek do 2 s pro snapshot ≤60 s | ❌ delta 4.4 |
| NFR-24 | `get_build_context` ≤100 ms (read-only metadata) | ❌ delta 4.4 |
| NFR-25 | MCP server response latence ≤200 ms p99 (pro read commandy) | ❌ Fáze K |
| NFR-26 | `read_only=true` zablokuje modifikující command před side-effect (no partial state) | ❌ delta 4.4 |
| NFR-27 | AI-metadata tag na 100 % assetů vygenerovaných přes `generate_*` (žádný silent skip) | ❌ delta 4.4 |

---

## 9. Out of Scope (v4.4) — s explicitním zdůvodněním

Tato sekce nahrazuje "Out of Scope" z v4.3 — doplňuje **proč** a **kritéria re-evaluace**.

| Položka | Proč mimo scope | Kritérium re-evaluace |
|---------|-----------------|----------------------|
| **3D object generation** z text promptu | Externí API (Meshy, Tripo, Luma) k 2026-05 produkují topologii nevhodnou pro Unity rig, vyžadují retopo. | Re-evaluovat když některé API dosáhne ≥80 % kvality manuálně modelovaného assetu (kontrola Q1 2027). |
| **Animation clip generation** | Text-to-animation API nedosahuje produkční kvality (jitter, foot sliding, no Humanoid retargeting). | Re-evaluovat když Unity AI Generators nebo externí API dodá Humanoid-compatible output (kontrola Q2 2027). |
| **Sentis / runtime ML** | Jiná doména — ML inference v shipped hře, ne dev tooling. UCAF řídí editor. | Trvale mimo scope (architektonické rozhodnutí). Pokud projekt potřebuje Sentis, integruje ho jako game feature, ne přes UCAF. |
| **Conversational chat UI v Editoru** | Rozbíjí file-based reproducibilitu. Claude Code / IDE = frontend. | Trvale mimo scope. |
| **Cloud Dashboard / credit billing** | UCAF je offline-first. | Trvale mimo scope. |
| **AI Gateway (LLM switching)** | Volba LLM je odpovědnost klienta (Claude Code, Cursor, …). UCAF je LLM-agnostický backend. | Trvale mimo scope. |
| **MCP client** (volání externích MCP serverů z UCAF) | UCAF je server, ne agent. | Re-evaluovat pokud vznikne use case "UCAF jako agent ovládající jiné systémy" — nepravděpodobné. |
| **Named pipe runtime channel** | Zamítnuto v Fázi B, file-based IPC dostačuje. | Trvale mimo scope. |
| **WebSocket console streaming** | Zamítnuto v Fázi E. | Trvale mimo scope. |
| **Přímá analýza MP4** | Fáze F: PNG framy stačí. | Trvale mimo scope. |
| **Audio analýza záznamů** | Mimo Fázi F. | Re-evaluovat pokud bude potřeba audio glitch detection. |
| **Asset Store placené balíčky** | License management mimo UCAF. Zdarma `.unitypackage` import přes `import_asset` ✅. | Trvale mimo scope. |
| **Unity Cloud Build** | UCAF buildí lokálně (`build_player` ✅). | Trvale mimo scope. |
| **VR / XR specifické flows** | Projekt SBW není VR. | Re-evaluovat pokud projekt přibere VR scope. |
| **Mobile signing / certificates** | Projekt SBW je desktop. | Re-evaluovat pokud projekt přibere mobile. |
| **Plnotučný ShaderGraph node editor** | Fáze D pokrývá properties. Node editor by zdvojoval Unity GUI. | Trvale mimo scope. |
| **Multiplayer / NetCode** | Projekt SBW je singleplayer. | Re-evaluovat pokud projekt přibere multiplayer. |
| **Schvalování každé akce** | Záměrně autonomní; `confirm=true` na destruktivní + `read_only=true` (delta 4.57) pro safe mode. | Trvale mimo scope (over-engineering). |

---

## 11. Stav v4.3 → v4.4 (delta)

| Oblast | v4.3 | v4.4 |
|--------|------|------|
| Existující moduly mimo PRD | ❌ nedokumentované | ✅ sekce 4.52 (Batch, BugLedger, CodeGrep, EditFile, Files, Query, InspectorV3, Screenshot, ExecuteMenuItem, ProtocolCapabilities) |
| Performance hints (NL ekv.) | ❌ chybí | ❌ FR-219 plán |
| Console diagnose hints | ❌ chybí | ❌ FR-220 plán |
| Cubemap generation | ❌ chybí | ❌ FR-221 plán (Fáze H rozšíření) |
| Terrain layer generation | ❌ chybí | ❌ FR-222 plán (Fáze H rozšíření) |
| AI-metadata tagging | ❌ chybí | ❌ FR-223 plán (Fáze H cross-cutting) |
| Build context discovery | ❌ chybí | ❌ FR-224 plán |
| MCP transport | ❌ chybí | ❌ FR-225–227 Fáze K plán |
| Read-only Ask mode | ❌ chybí | ❌ FR-228 cross-cutting plán |
| FR-192 kolize | ⚠️ existuje | ✅ errata E-1 |
| Duplicitní 4.44 | ⚠️ existuje | ✅ errata E-2 |
| Sekce 7 status | ⚠️ nekonzistentní | ✅ errata E-3 |
| Out of Scope zdůvodnění | ⚠️ jen seznam | ✅ + kritéria re-evaluace |
| Diferenciace vs Unity AI | ❌ implicitní | ✅ sekce 4.51 |

---

## 12. Implementation priority (delta v4.4)

Pořadí implementace delty (po dokončení v4.3 Fáze G+H+I+J):

**Quick wins (1–2 session work):**
1. **FR-224 `get_build_context`** — pár řádků, eliminuje class of bugs, použitelné okamžitě i bez ostatní delty
2. **FR-227 `protocol_capabilities`** — pravděpodobně už existuje, jen kanonizovat v PRD
3. **Errata E-1 až E-5** — čistka, ne kód

**Mid-effort (každé 1 fáze):**
4. **FR-223 AI-metadata tagging** — implementovat zároveň s Fází H (FR-199–205)
5. **FR-221, FR-222 cubemap + terrain layer** — extension Fáze H, ne samostatná fáze
6. **FR-228 read_only mode** — cross-cutting, ale jednoduchý whitelist check v dispatcheru

**Větší úsilí:**
7. **FR-219, FR-220 diagnose** — vyžaduje katalog patternů, design schema, testy
8. **FR-225–227 MCP server (Fáze K)** — nová transport vrstva, ~1–2 sessions

**Sekce 4.52** (dokumentace existujících modulů) je čistě dokumentační — žádný kód.

---

## 13. Designová rozhodnutí v4.4 (resolved)

Kritéria: nejsnadnější práce s UCAF + maximální obecnost. Pokud si protiřečí, jde se po obecnosti.

### D-1: MCP transport — opt-in přes config, default off

- `ucaf_config.json` má sekci `mcp_server` s `enabled` (default `false`) + `auto_start` (default `true`) + `port=7374` + `bind=127.0.0.1`.
- Když `enabled=true` & `auto_start=true`, server startuje s Editorem — MCP klienti dostanou Unity bez nutnosti volat `mcp_server_start`.
- `mcp_server_start/stop` zůstává jako runtime override (debug, dočasné vypnutí).
- **Důvod:** žádný TCP port se neotevře bez explicitního opt-in (security). Single config flag pokrývá oba use cases (manuální vs auto). Stávající UCAF instalace nepocítí žádnou změnu.
- **Lokace v PRD:** sekce 4.56.

### D-2: Diagnose pattern katalogy — 3-vrstvý lookup

1. **Built-in** v UCAF package (`com.ucaf/Editor/diagnose/*.json`) — ships s UCAF, ~30 nejčastějších Unity patternů, hodnota hned
2. **Workspace** (`ucaf_workspace/diagnose/*.json`) — git-tracked, project-specific; entry s matching `id` override built-in, nové entries se appendnou
3. **Inline** (`extra_patterns=[...]` na requestu) — one-off testy bez perzistence

Plus `diagnose_list_patterns` (FR-220a) — vrátí merged katalog s `source` polem per entry, pro debug "co override-uje co".
- **Důvod:** nový uživatel dostává hodnotu ze built-in vrstvy bez setup, projekty rozšiřují bez forku UCAF, ad-hoc testy bez perzistence.
- **Lokace v PRD:** sekce 4.53.

### D-3: AI-metadata tagging — vždy zapnuté, žádný opt-out

- Každý `generate_*` command zapisuje `m_UserData` JSON marker povinně. Žádný `mark_generated=false` parametr v API.
- Escape hatch: ruční edit `.meta` souboru — vědomá akce, ne silent skip přes flag.
- **Důvod:** audit/legal hodnota tagu vzniká právě 100% spolehlivostí. Opt-out flag by ji devaluoval. API stays minimal (méně parametrů k dokumentaci, testování, validaci). NFR-27 (100% tagging) je tím garantován.
- **Lokace v PRD:** sekce 4.54 FR-223.

### D-4: Read-only mode — pouze per-command, žádný session default

- Jediný mechanismus: `read_only=true` parametr na requestu. Žádný `default_mode` v configu.
- Whitelist je hardcoded v dispatcheru (žádný externí soubor pro uživatele).
- CI / audit use case: klient (wrapper script) přidává flag ke všem voláním. UCAF backend stays jednoduchý.
- **Důvod:** jeden code path = jednodušší testování a dokumentace. Per-command pokrývá oba use cases (ad-hoc + CI wrapper). Pokud emerguje real demand pro session default, lze přidat aditivně bez breaking change.
- **Lokace v PRD:** sekce 4.57 FR-228.

---

## 14. Co tato delta NENÍ

- **Nový sprint** — Fáze A–F implementovány, Fáze G–J z v4.3 stále primární backlog
- **Náhrada Unity AI** — UCAF Fáze G+H+I+J+delta poskytuje *podmnožinu* autoringu + plnou testovací smyčku
- **Migrace na MCP** — file-IPC zůstává master transport (NFR-12 reload survival je tam ověřená)
- **Rewrite** — všechny existující commandy fungují identicky
