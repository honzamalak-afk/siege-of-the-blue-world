# PRD – Unity + Claude Code AI-Assisted Framework
**Version:** 4.2
**Author:** Jan Malák
**Date:** 2026-04-25
**Status:** Active — Fáze A+B+C+D+E+F dokončena
**Předchozí verze:** PRD_Unity_Claude_Framework_v4_1.md (v4.1, OBSOLETE)

---

## 0. Stav implementace (aktuální snapshot)

```
Fáze A  ✅ DONE  — Asset Import, Package Manager (+ search), Project Settings, Build Settings, Git
Fáze B  ✅ DONE  — Runtime Bridge, PlayMode loop, Test Runner, Input simulace
Fáze C  ✅ DONE  — Animator Controller, Build Player, NavMesh, Lightmap
Fáze D  ✅ DONE  — Profiler, Timeline/Cinemachine, ShaderGraph/VFX, Terrain
Fáze E  ✅ DONE  — JSON Schema, Console streaming, Editor Preferences
Fáze F  ✅ DONE  — Video záznam gameplay + frame extrakce na event timestamps (com.unity.recorder 5.1.6)
```

**Architektonická rozhodnutí učiněná při implementaci Fáze B:**
- PRD původně plánoval **named pipe** (`\\.\pipe\ucaf_runtime`) pro runtime queries. Implementováno jako **file-based IPC** (`runtime_pending/` / `runtime_done/`) — jednodušší, bez cross-platform FIFO problémů, dostatečná latence pro stávající use case. Named pipe se neimplementuje.
- NFR-12 (≤50 ms) proto není splněno; typická latence je 1–2 editor ticky (~32–100 ms). Pro současné scénáře dostačující.
- WebSocket pro console streaming (FR-179–182) nebyl implementován — zůstává v Fázi E jako file-based streaming (bez WS varianty).

---

## 1. Overview

### 1.1 Project Name
**UCAF** – Unity Claude Assisted Framework

### 1.2 Purpose
UCAF spojuje Claude Code s Unity Editorem tak, aby Claude pracoval jako autonomní junior dev s plnou verifikační smyčkou: **vidí, testuje, hraje, rollbackuje**. Od v4.2 zahrnuje "testuje" i runtime introspekci a simulovaný vstup, ne jen statickou kompilaci.

### 1.3 Role split
- **Claude (junior dev)**: implementace, refactor, fix bugů, runtime debugging, regression checks.
- **Lidský dev (lead)**: design rozhodnutí, game feel, finální QA, architektonické směry.

### 1.4 Vision Statement
*"Junior co dělá, vidí, testuje, hraje, umí to vrátit zpět — a sám pozná, že se mu mechanika rozbila."*

### 1.5 Co UCAF není
- Není autopilot ani game designer.
- Negaruje game feel ani polish — pouze umožňuje rychle iterovat.
- Není CI server, ale od v4.2 se k němu blíží (Test Runner integrace).
- Není náhrada za vizuální editory; **je doplněk**, který umožňuje 80 % práce dělat headless.

---

## 2. Goals & Success Criteria

### 2.1 Functional Goals (v4.2)
- ✅ Claude dokáže napsat fix → zkompilovat → spustit playmode → simulovat input → přečíst runtime stav → ověřit, že fix funguje → exit playmode — vše v jednom workflow bez lidského zásahu.
- ✅ Claude dokáže spustit existující Unity testy a získat výsledky.
- ✅ Claude dokáže nastavit import settings tak, aby loopTime bug ze Session 1 nemohl vzniknout.
- ❌ Claude dokáže buildnout Player a spustit ho — verifikace mimo Editor. (Fáze C)
- ❌ Claude dokáže vytvořit a editovat Animator Controller bez ručního klikání. (Fáze C)
- ❌ Claude dokáže nahrát gameplay sekvenci a analyzovat vizuální bugy přes extrahované framy. (Fáze F)

### 2.2 Non-Functional Goals
- ✅ Reload survival: všechny async operace přežijí domain reload.
- ✅ Bezpečnost: žádná destruktivní operace bez explicit confirmation parametru.
- ✅ Backward compat: všechny v4.1 commandy fungují identicky.
- ❌ Latence runtime field read ≤ 50 ms (file-based polling: ~32–100 ms — rozhodnutí finální, named pipe se neimplementuje).

### 2.3 Success Criteria
- ✅ Claude opraví regresi v gameplay mechanice end-to-end bez lidského "zkus to" pingu. (možné od Fáze B)
- ✅ Test Runner výsledek dostupný do 60 s pro typický PlayMode test.
- ❌ Profiler snapshot detekuje 30 % framerate drop. (Fáze D)

---

## 3. Architecture

### 3.1 Komunikační kanály

**Implementováno:**
- `commands/{pending,done,errors}/` — hlavní file-based IPC (beze změny od v4.1)
- `commands/runtime_pending/` + `commands/runtime_done/` — file-based proxy pro RuntimeBridge během PlayMode
- `commands/runtime_signals.json` — log signálů emitovaných user kódem

**Neimplementováno (zamítnuto po implementaci Fáze B):**
- Named pipe `\\.\pipe\ucaf_runtime` — nahrazen file-based IPC, dostatečný
- WebSocket `ws://localhost:7373/ucaf` — Fáze E (pouze file-based varianta)

### 3.2 Runtime bridge ✅

Singleton `UCAF_RuntimeBridge` (MonoBehaviour, DontDestroyOnLoad) v `Assets/Scripts/UCAF/`. Instantiuje se při enter PlayMode přes `UCAF_Listener.PlayMode2.cs`. Exposuje get/set field a call method na live runtime objektech přes `runtime_pending/` soubory.

`UCAF_Listener` (Editor) je master — RuntimeBridge je jeho proxy během PlayMode.

### 3.3 Async pattern

Všechny async commandy: vrací `null`, `Check*` metoda na `EditorApplication.update` polluje stav, výsledek jde do `done/{id}.json`. SessionState zajišťuje přežití domain reload.

**Registrované async checkery:**
- `CheckPendingCompile` — compile_and_wait
- `CheckPendingScreenshot` — take_screenshot
- `CheckPendingPackageManager` — add/remove/update_package
- `CheckPendingPackageManagerOnStartup` — domain reload recovery po add_package
- `CheckPendingPlayModeEnter` / `CheckPendingPlayModeExit` / `CheckPendingPlayModeRun`
- `CheckPendingRuntimeProxy` — playmode_runtime_get/set/call (5s timeout)
- `CheckPendingTestRun` — run_tests timeout watchdog
- `CheckPendingTestList` — list_tests async fallback
- `CheckPendingBuildTargetOnStartup` — domain reload recovery po set_build_target

### 3.4 Soubory implementace

| Soubor | Obsah | Status |
|--------|-------|--------|
| `UCAF_Listener.cs` | Dispatcher, konstruktor, Poll loop | ✅ |
| `UCAF_Types.cs` | Všechny `[Serializable]` typy | ✅ |
| `UCAF_Listener.AssetImport.cs` | FR-142–146 | ✅ |
| `UCAF_Listener.PackageManager.cs` | FR-147–150 | ✅ |
| `UCAF_Listener.ProjectSettings.cs` | FR-127–133 | ✅ |
| `UCAF_Listener.BuildSettings.cs` | FR-136–137, FR-171–174 | ✅ |
| `UCAF_Listener.PlayMode2.cs` | FR-104–110 | ✅ |
| `UCAF_Listener.TestRunner.cs` | FR-111–114 | ✅ |
| `UCAF_Listener.Input.cs` | FR-115–118 | ✅ |
| `Assets/Scripts/UCAF/UCAF_RuntimeBridge.cs` | runtime_get/set/call, Signal() | ✅ |
| `UCAF_Listener.BugLedger.cs` | FR-89–103 (v4.1) | ✅ |
| *(ostatní soubory v4.1)* | FR-1–88 | ✅ |

---

## 4. Functional Requirements

### 4.1–4.26 — v4.1 commandy ✅ (beze změny)

Všech 88 FR z v4.1 funguje identicky.

---

### 4.27 PlayMode test loop ✅ DONE (FR-104 až FR-110)

**FR-104 `playmode_enter`** ✅ — vstoupit do play mode. Async, vrací `playmode_session_id`.

**FR-105 `playmode_exit`** ✅ — opustit play mode. Async, vrací frames/elapsed.

**FR-106 `playmode_run`** ✅ — high-level: enter + wait_for + exit.
  - `wait_for=time:5` — čekat N sekund
  - `wait_for=frames:120` — čekat N framů
  - `wait_for=event:OnPlayerDied` — čekat na signal z `UCAF_RuntimeBridge.Signal()`
  - `wait_for=predicate:Player/PlayerHealth:health<=0` — eval přes runtime_get (best-effort)

**FR-107 `playmode_runtime_get`** ✅ — přečíst pole na komponentě za běhu. Async (~1–2 editor ticky).

**FR-108 `playmode_runtime_set`** ✅ — zapsat pole na komponentě za běhu.

**FR-109 `playmode_runtime_call`** ✅ — volat metodu (int/float/string/bool/Vector2/Vector3 args).

**FR-110 `playmode_signal_subscribe`** ✅ — smaže starý signal log; `UCAF_RuntimeBridge.Signal("name")` v user kódu zapisuje do `runtime_signals.json`; po exit_playmode seznam signálů v datech.

---

### 4.28 Unity Test Runner ✅ DONE (FR-111 až FR-114)

**FR-111 `run_tests`** ✅ — `mode=editmode|playmode|all`, `filter=`, `timeout_seconds=120`. Async, per-test výsledky.

**FR-112 `list_tests`** ✅ — výpis testů bez spuštění. Callback sync nebo async (deferred fallback).

**FR-113 `create_test`** ✅ — kostry NUnit `[Test]` nebo `[UnityTest]` souborů.

**FR-114 `register_test_assembly`** ✅ — vytvoří `Assets/Tests/Tests.asmdef` pokud neexistuje.

---

### 4.29 Input simulace ✅ DONE (FR-115 až FR-118)

Vyžaduje `com.unity.inputsystem` (`#if ENABLE_INPUT_SYSTEM`). Bez něj jasný error s instrukcí. Pouze v PlayMode.

**FR-115 `input_press_key`** ✅ — `key=Space|W|...`, `duration_ms=100`. `InputSystem.QueueStateEvent(Keyboard, KeyboardState)`.

**FR-116 `input_move_mouse`** ✅ — `to=x,y` nebo `delta=dx,dy`, `duration_ms=200` (smooth lerp).

**FR-117 `input_gamepad_stick`** ✅ — `stick=left|right`, `value=x,y` (-1..1), `duration_ms`.

**FR-118 `input_sequence`** ✅ — JSON pole: `[{"type":"press_key","key":"W","delay_ms":0,"duration_ms":200},...]`.

---

### 4.30 Animator Controller editor ✅ DONE — Fáze C (FR-119 až FR-126)

**FR-119 `create_animator_controller`** — `path=Assets/Animators/PlayerAnim.controller`.

**FR-120 `add_animator_state`** — `controller_path`, `state_name`, `motion_path`, `layer=0`, `as_default=false`.

**FR-121 `add_animator_transition`** — `from_state`, `to_state`, `conditions=[{param,op,value}]`, `has_exit_time`, `duration`.

**FR-122 `add_animator_parameter`** — `param_name`, `type=Float|Int|Bool|Trigger`, `default_value`.

**FR-123 `add_blend_tree`** — `blend_type=Simple1D|2DCartesian`, `param_x/y`, `motions=[...]`.

**FR-124 `set_animator_layer`** — weight, mask, blending mode.

**FR-125 `list_animator_states`** — diagnostika: states, transitions, parametry.

**FR-126 `validate_animator`** — "state bez motion", "parametr bez transition", atd.

---

### 4.31 Project Settings API ✅ DONE (FR-127 až FR-133)

**FR-127 `add_tag`** ✅ — idempotentní, TagManager.asset přes SerializedObject.

**FR-128 `add_layer`** ✅ — auto-alokuje první volný slot.

**FR-129 `set_input_axis`** ✅ — legacy InputManager.

**FR-130 `add_input_action`** ✅ — přidá binding do `.inputactions` JSON souboru.

**FR-131 `set_physics_collision`** ✅ / **`set_physics2d_collision`** ✅ — collision matrix.

**FR-132 `set_quality_setting`** ✅ — `level`, `key`, `value`.

**FR-133 `set_graphics_setting`** ✅ — RenderPipeline asset, colorSpace (vyžaduje `confirm=true`).

---

### 4.32 Build Player & run ✅ / ❌ (FR-134 až FR-137)

**FR-134 `build_player`** ✅ DONE

**FR-135 `run_player`** ✅ DONE

**FR-136 `set_build_scenes`** ✅ DONE — `scenes=[paths]`, `enabled=[bool,...]`.

**FR-137 `set_build_target`** ✅ DONE — async, domain reload safety přes SessionState.

---

### 4.33 NavMesh bake & query ✅ DONE — Fáze C (FR-138 až FR-141)

**FR-138 `navmesh_bake`** — async, NavMeshSurface.Build().

**FR-139 `navmesh_query_path`** — `from`, `to`, `area_mask`. Vrací corners + length.

**FR-140 `navmesh_sample_position`** — nejbližší walkable bod.

**FR-141 `add_offmesh_link`** — `from_obj_path`, `to_obj_path`, `bidirectional`.

---

### 4.34 Asset Import Settings API ✅ DONE (FR-142 až FR-146)

**FR-142 `set_texture_import`** ✅ — `textureType`, `compression`, `srgb`, `mipmap`, `wrapMode`, `filterMode`, `maxSize`, `alphaSource`, `readable`.

**FR-143 `set_model_import`** ✅ — `animationType`, `avatarSetup`, `copyAvatarFrom`, `optimizeGameObjects`, `importMaterials`, `meshCompression`, `readWriteEnabled`.

**FR-144 `set_animation_clip_import`** ✅ — `clip_name`, `loopTime`, `loopPose`, `cycleOffset`. **Řeší loopTime bug ze Session 1.**

**FR-145 `set_audio_import`** ✅ — `loadType`, `compressionFormat`, `quality`, `forceToMono`, `loadInBackground`.

**FR-146 `validate_imports`** ✅ — pravidla v `ucaf_workspace/import_rules.json`.

---

### 4.35 Package Manager ✅ DONE (FR-147 až FR-150, FR-192)

**FR-147 `list_packages`** ✅ — name, version, source — výpis nainstalovaných balíčků.

**FR-148 `add_package`** ✅ — async, domain reload safety (výsledek se zapíše po reloadu).

**FR-149 `remove_package`** ✅ — async.

**FR-150 `update_package`** ✅ — `Client.Add(name@version)`, domain reload safety.

**FR-192 `search_packages`** ✅ — `query=keyword nebo com.unity.xxx`. Prohledá Unity Registry přes `Client.Search()`. Vrátí `UCAFPackageList` s name, displayName, version, description, category. Použití: nejdřív vyhledat → pak `add_package` s přesným ID.

---

### 4.36 Profiler snapshot ✅ DONE — Fáze D (FR-151 až FR-153)

**FR-151 `profiler_capture`** — `duration_seconds`, `categories=[Render,Scripts,Physics,GC]`. Vrací avg/min/max ms, draw_calls, GC alloc.

**FR-152 `profiler_compare`** — `before_id`, `after_id`. Delta + `regression=true` flag.

**FR-153 `profiler_record_to_file`** — uložit .data soubor pro Unity Profiler.

---

### 4.37 Timeline / Cinemachine ✅ DONE — Fáze D (FR-154 až FR-159)

**FR-154 `create_timeline`** — `path`.

**FR-155 `add_timeline_track`** — `track_type=Animation|Audio|Activation|Cinemachine|Signal`, `binding`.

**FR-156 `add_timeline_clip`** — `track_index`, `start`, `duration`, `clip_asset_path`.

**FR-157 `create_cinemachine_camera`** — `name`, `priority`, `follow`, `look_at`, `body`.

**FR-158 `set_cinemachine_blend`** — `from_cam`, `to_cam`, `style`, `time`.

**FR-159 `cinemachine_brain_set`** — brain settings na main kameře.

---

### 4.38 ShaderGraph / VFX Graph ✅ DONE — Fáze D (FR-160 až FR-164)

> Scope: **read & basic edit** (properties, ne nodes).

**FR-160 `create_shadergraph`** — `path`, `template=Lit|Unlit|UI|Decal`.

**FR-161 `set_shadergraph_property`** — `property_name`, `default_value`, `exposed=true`.

**FR-162 `list_shadergraph_properties`** — exposed properties.

**FR-163 `create_vfx_graph`** — `path`, `template=Particles|Mesh|Trail`.

**FR-164 `set_vfx_property`** — exposed property na VFX asset.

---

### 4.39 Lightmap bake ✅ DONE — Fáze C (FR-165, FR-166)

**FR-165 `lightmap_bake`** — async, vrací duration, atlas count, peak memory, errors.

**FR-166 `lightmap_clear`** — vyčistit baked data pro scénu.

---

### 4.40 Terrain ✅ DONE — Fáze D (FR-167 až FR-170)

**FR-167 `create_terrain`** — `name`, `size=512,30,512`, `heightmap_resolution=513`.

**FR-168 `terrain_set_heightmap`** — PNG/EXR nebo JSON heights array.

**FR-169 `terrain_paint_layer`** — `layer_index`, `mask_image_path`, `layer_asset_path`.

**FR-170 `terrain_add_tree_prototype`** / **`terrain_paint_trees`** — placement.

---

### 4.41 Git operace ✅ DONE (FR-171 až FR-174)

**FR-171 `git_status`** ✅ — modified/added/deleted soubory.

**FR-172 `git_diff`** ✅ — `path=optional`, vrací patch.

**FR-173 `git_commit`** ✅ — `message`, `paths=[]`, vyžaduje `confirm=true`.

**FR-174 `git_branch`** ✅ — `action=list|create|switch`, `name`.

> Záměrně vynecháno: `push`, `reset --hard`, `force`, `rebase` — dělá člověk z terminálu.

---

### 4.42 JSON Schema validace ✅ DONE — Fáze E (FR-175 až FR-178)

**FR-175** Schema soubory v `ucaf_workspace/schemas/{command_type}.schema.json`.

**FR-176** `get_command_schema(type)` — vrátí JSON Schema dokument.

**FR-177** Pre-dispatch schema validace; `UCAFErrorContext` s `error_code=schema_violation`.

**FR-178** `validate_command(payload)` — dry validace bez exekuce.

---

### 4.43 Console subscribe / streaming ✅ DONE — Fáze E (FR-179 až FR-182)

> WebSocket varianta zamítnuta (Fáze B rozhodnutí). Implementuje se pouze file-based.

**FR-179** `console_subscribe(filter)` — subscription_id, log eventy do `ucaf_workspace/streams/{id}.ndjson`.

**FR-180** `console_unsubscribe(subscription_id)`.

**FR-181** Filter: `level=Error|Warning|Log|All`, `pattern=regex`, `from_assembly=name`.

**FR-182** `playmode_run` automaticky vytvoří dočasnou subscription → console report v datech.

---

### 4.44 Video záznam gameplay ✅ DONE — Fáze F (FR-186 až FR-191)

Záměr: zachytit vizuální bugy (ragdoll anomálie, 2-frame glitche, náhodné jevy) které nejdou zachytit kompilací, Test Runnerem ani `playmode_runtime_get`. Claude **nečte video přímo** — pipeline extrahuje PNG framy kolem event timestamps a Claude analyzuje obrázky přes vision.

**Architektura:**
- Unity Recorder package (`com.unity.recorder`) nahrává PNG sekvenci nebo MP4 do `ucaf_workspace/recordings/{session_id}/`
- NDJSON event log (`recording_events.ndjson`) zaznamenává timestampy Signálů, physics eventů a checkpointů v reálném čase
- Po skončení záznamu se extrahují PNG framy ±N framů kolem každého logu eventu → `ucaf_workspace/recordings/{session_id}/frames/`
- Claude čte event log pro orientaci, poté analyzuje PNG framy vizuálně

**FR-186 `recording_start`** — `session_id`, `format=png_sequence|mp4`, `fps=30`, `resolution=game_view`. Vyžaduje `com.unity.recorder`; bez něj jasný error s instrukcí pro instalaci. Spustitelné samostatně i jako část `playmode_run record_video=true`.

**FR-187 `recording_stop`** — ukončí záznam, vrátí `session_id`, `frame_count`, `duration_s`, `path`.

**FR-188 `recording_extract_frames`** — `session_id`, `timestamps=[{t, label}]` nebo `events=all|signal|physics`. Extrahuje PNG framy ±`context_frames=5` kolem každého timestampu. Vrátí seznam cest k PNG souborům.

**FR-189 `recording_list`** — výpis existujících záznamů v `ucaf_workspace/recordings/` — id, datum, frame count, velikost na disku.

**FR-190 `recording_delete`** — `session_id` nebo `older_than_days=N`. Vyžaduje `confirm=true`.

**FR-191 `playmode_run` rozšíření** — nový parametr `record_video=true` spustí `recording_start` při enter a `recording_extract_frames events=all` při exit. Framy přiloženy do výsledku vedle runtime snapshotu.

**Integrace s Bug Ledgerem:**
- Nový typ bugu: `visual_glitch` — obsahuje `session_id`, `timestamp`, seznam cest k PNG framům
- `playmode_run record_video=true` automaticky vytvoří `visual_glitch` entry pokud Signal obsahuje prefix `"visual:"`

**Omezení (out of scope Fáze F):**
- Přímá analýza MP4 — Claude čte pouze extrahované PNG
- Audio analýza
- Síťový streaming záznamu

---

### 4.44 Editor Preferences / Layout ✅ DONE — Fáze E (FR-183 až FR-185)

**FR-183 `apply_editor_layout`** — `layout_path`. Vyžaduje confirmation.

**FR-184 `set_editor_pref`** — `key`, `value`. Typed wrappery pro standardní klíče.

**FR-185 `get_editor_pref`** — read.

---

### 4.45 Bug ledger rozšíření (navazuje na v4.1) ✅

Tři nové kategorie:
- **runtime_regression** — bug z `playmode_run` (predikát selhal)
- **test_failure** — bug z Test Runner
- **import_drift** — bug z `validate_imports`

Pole `verification.tests` propojené s Test Runner / PlayMode scénáři.

---

### 4.46 Cross-cutting parametry

K `if_exists`, `dry_run`, `screenshot_after` (v4.1) se přidává:
- **`record_runtime=true`** — při playmode_* commandu pořídí runtime snapshot.
- **`undo_group=name`** — pojmenování undo group.

---

## 5. Non-Functional Requirements

| NFR | Požadavek | Status |
|-----|-----------|--------|
| NFR-12 | Runtime field read ≤ 50 ms | ❌ ~32–100 ms (file-based; named pipe zamítnut) |
| NFR-13 | PlayMode enter→exit UCAF overhead ≤ 1 s | ✅ |
| NFR-14 | Build Player log streaming (progress) | ❌ Fáze C |
| NFR-15 | Test Runner per-test výsledky | ✅ |
| NFR-16 | Frame extrakce dokončena do 10 s po `recording_stop` (pro záznam ≤60 s, PNG sequence) | ❌ Fáze F |
| NFR-17 | Disk usage: automatický cleanup záznamů starších 7 dní | ❌ Fáze F |

---

## 6. Risks & Mitigations

| Riziko | Mitigace |
|--------|----------|
| Runtime bridge crashne hru, Editor zůstane v PlayMode | 5s timeout v `CheckPendingRuntimeProxy` ✅ |
| Build Player zaplní disk | Cleanup po 5 buildech (Fáze C) |
| Animator API změny mezi Unity verzemi | Version detection + fallback na `edit_file` (Fáze C) |
| ShaderGraph rozbití přes API | Pouze properties, ne nodes; git revert (Fáze D) |
| Unity Recorder package chybí | `recording_start` vrátí jasný error s `add_package com.unity.recorder` instrukcí (Fáze F) |
| PNG sekvence zaplní disk | NFR-17 auto-cleanup + `recording_delete older_than_days=7` (Fáze F) |
| MP4 encoding blocker na headless | Fallback na `format=png_sequence`; MP4 je optional (Fáze F) |
| Input System absentuje | `#if ENABLE_INPUT_SYSTEM` + jasný error s instrukcí ✅ |
| Domain reload ztrácí PM state | SessionState backup + `CheckPendingPackageManagerOnStartup` ✅ |
| Partial class field init order | WorkspacePath-závislé fieldy v UCAF_Listener.cs nebo static ctor ✅ |

---

## 7. Implementation Plan

**Fáze A ✅ DONE**
- FR-142–146 Asset Import Settings API
- FR-147–150 Package Manager
- FR-127–133 Project Settings API
- FR-136–137 Build Settings
- FR-171–174 Git operace

**Fáze B ✅ DONE**
- UCAF_RuntimeBridge (MonoBehaviour, file-based IPC)
- FR-104–110 PlayMode loop
- FR-111–114 Test Runner
- FR-115–118 Input simulace

**Fáze C ❌ TODO — Authoring tools**
- FR-119–126 Animator Controller editor
- FR-134–135 Build Player & run
- FR-138–141 NavMesh bake & query
- FR-165–166 Lightmap bake

**Fáze D ❌ TODO — Heavy lifting**
- FR-151–153 Profiler snapshot
- FR-154–159 Timeline / Cinemachine
- FR-160–164 ShaderGraph / VFX (basic)
- FR-167–170 Terrain

**Fáze E ❌ TODO — V4.1 odložené + streaming**
- FR-175–178 JSON Schema validace
- FR-179–182 Console subscribe / streaming (file-based)
- FR-183–185 Editor Preferences / Layout

**Fáze F ❌ TODO — Video záznam gameplay + vizuální analýza**
- FR-186 `recording_start`
- FR-187 `recording_stop`
- FR-188 `recording_extract_frames`
- FR-189 `recording_list`
- FR-190 `recording_delete`
- FR-191 `playmode_run` rozšíření o `record_video=true`
- Závislost: `com.unity.recorder` package

---

## 8. Bug Ledger

Platí vše z v4.1 + tři nové typy z 4.45. ✅

---

## 9. Out of Scope (v4.2)

- Asset Store integrace (placené balíčky, license keys)
- Cloud Build / Unity Cloud
- VR / XR specifické flows
- Mobile signing / certificate management
- Plnotučný ShaderGraph node editor
- AI/ML inference (Sentis, ONNX)
- Multiplayer / NetCode tooling
- Named pipe runtime channel (zamítnut — file-based IPC dostatečný)
- WebSocket console streaming (zamítnut — file-based streaming v Fázi E)
- Přímá analýza MP4 videa (Fáze F: pouze extrahované PNG framy)
- Audio analýza záznamů
- Unity UI automation přes Computer Use (křehké, IMGUI pokrytí ~50 %; přeskočeno)

---

## 10. Glosář

- **Runtime bridge** — `UCAF_RuntimeBridge.cs`, MonoBehaviour v PlayMode, exposuje get/set/call přes `runtime_pending/` soubory.
- **PlayMode session** — období mezi `playmode_enter` a `playmode_exit`, identifikovatelné `playmode_session_id`.
- **Signal** — `UCAF_RuntimeBridge.Signal("name")` v user kódu; Claude čte přes `playmode_signal_subscribe`.
- **Test Runner** — Unity nativní API (`TestRunnerApi`, `ICallbacks`) pro NUnit EditMode/PlayMode testy.
- **Async checker** — metoda registrovaná na `EditorApplication.update`, polluje SessionState, píše do `done/`.
- **Domain reload** — Unity restart assemblies po kompilaci nebo package instalaci; SessionState přežívá, static fields ne.
- **Recording session** — období mezi `recording_start` a `recording_stop`; identifikovatelné `session_id`. PNG framy nebo MP4 v `ucaf_workspace/recordings/{session_id}/`.
- **Frame extrakce** — výběr PNG framů ±N kolem event timestampů z recording session; vstup pro Claudeovu vizuální analýzu.
- **visual_glitch** — typ bugu v Bug Ledgeru; obsahuje `session_id`, timestamp a seznam PNG cest z frame extrakce.

---

## 11. Stav v4.1 → v4.2

| Oblast | v4.1 | v4.2 skutečnost |
|--------|------|-----------------|
| Runtime introspekce | ❌ | ✅ file-based IPC (named pipe zamítnut) |
| PlayMode automatizace | ❌ | ✅ |
| Test Runner | ❌ | ✅ |
| Input simulace | ❌ | ✅ |
| Asset Import Settings | ❌ | ✅ |
| Package Manager | ❌ | ✅ list / search / add / remove / update |
| Project Settings API | ❌ | ✅ |
| Build Settings | ❌ | ✅ |
| Git operace | ❌ | ✅ |
| Animator Controller editor | ❌ | ✅ Fáze C |
| Build Player & run | ❌ | ✅ Fáze C |
| NavMesh | ❌ | ✅ Fáze C |
| Lightmap bake | ❌ | ✅ Fáze C |
| Profiler | ❌ | ✅ Fáze D |
| Timeline / Cinemachine | ❌ | ✅ Fáze D |
| ShaderGraph / VFX | ❌ | ✅ Fáze D |
| Terrain | ❌ | ✅ Fáze D |
| JSON Schema validace | (plánováno) | ✅ Fáze E |
| Console streaming | (plánováno) | ✅ Fáze E (file-based) |
| Editor Layout/Prefs | ❌ | ✅ Fáze E |
| Video záznam + frame extrakce | ❌ | ✅ Fáze F (PNG sequence, MP4 opt-in) |
| Bug ledger | ✅ | ✅ + runtime/test/import kategorie |

---

## 12. Otevřené otázky

~~1. **Fáze C priorita** — co implementovat jako první: Animator Controller, nebo Build Player?~~
**✅ Uzavřeno** — oboje implementováno v Fázi C.

~~2. **Test Runner v headless build** — zda přidat `-runTests` CLI wrapper mimo Editor.~~
**✅ Uzavřeno jako out of scope** — In-editor Test Runner (Fáze B) pokrývá stávající potřeby. Headless CI pipeline není součástí UCAF v4.x.

~~3. **ShaderGraph stabilita API** — Unity 6 má jiné internal types.~~
**✅ Uzavřeno** — minimum viable implementováno (pouze properties), stabilní pro Unity 6.

~~4. **Fáze F: PNG sequence vs. MP4**~~
**✅ Uzavřeno** — PNG sequence jako primární formát, MP4 jako opt-in (implementováno v Fázi F).

~~5. **Fáze F: `context_frames` default**~~
**✅ Uzavřeno** — default = 5 framů (≈167 ms při 30 fps), přepisatelné parametrem.
