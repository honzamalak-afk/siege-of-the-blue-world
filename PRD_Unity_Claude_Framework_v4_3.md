# PRD – Unity + Claude Code AI-Assisted Framework
**Version:** 4.3
**Author:** Jan Malák
**Date:** 2026-05-19
**Status:** Active — Fáze A+B+C+D+E+F+G dokončena | Fáze H+I+J plánováno
**Předchozí verze:** PRD_Unity_Claude_Framework_v4_2.md (v4.2, OBSOLETE)

---

## 0. Stav implementace (aktuální snapshot)

```
Fáze A  ✅ DONE  — Asset Import, Package Manager (+ search), Project Settings, Build Settings, Git
Fáze B  ✅ DONE  — Runtime Bridge, PlayMode loop, Test Runner, Input simulace
Fáze C  ✅ DONE  — Animator Controller, Build Player, NavMesh, Lightmap
Fáze D  ✅ DONE  — Profiler, Timeline/Cinemachine, ShaderGraph/VFX, Terrain
Fáze E  ✅ DONE  — JSON Schema, Console streaming, Editor Preferences
Fáze F  ✅ DONE  — Video záznam gameplay + frame extrakce na event timestamps (com.unity.recorder 5.1.6)
Fáze G  ✅ DONE  — Unity UI automatizace (UI Toolkit API + Computer Use dry-run fallback)
Fáze H  ❌ TODO  — Generování assetů (textury přes image API, zvuky přes audio API)
Fáze I  ❌ TODO  — Plan mode, Figma → Unity UI, Scene z obrázku / reference
Fáze J  ❌ TODO  — Modulární Skills architektura
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
*"Junior co dělá, vidí, testuje, hraje, umí to vrátit zpět — a sám pozná, že se mu mechanika rozbila. Od v4.3 také plánuje, generuje assety a ovládá Editor bez ručního klikání."*

### 1.5 Co UCAF není
- Není autopilot ani game designer.
- Negaruje game feel ani polish — pouze umožňuje rychle iterovat.
- Není CI server, ale od v4.2 se k němu blíží (Test Runner integrace).
- Není náhrada za vizuální editory; **je doplněk**, který umožňuje 80 % práce dělat headless.

---

## 2. Goals & Success Criteria

### 2.1 Functional Goals (v4.2 — splněno)
- ✅ Claude dokáže napsat fix → zkompilovat → spustit playmode → simulovat input → přečíst runtime stav → ověřit, že fix funguje → exit playmode — vše v jednom workflow bez lidského zásahu.
- ✅ Claude dokáže spustit existující Unity testy a získat výsledky.
- ✅ Claude dokáže nastavit import settings tak, aby loopTime bug ze Session 1 nemohl vzniknout.
- ✅ Claude dokáže buildnout Player a spustit ho — verifikace mimo Editor. (Fáze C)
- ✅ Claude dokáže vytvořit a editovat Animator Controller bez ručního klikání. (Fáze C)
- ✅ Claude dokáže nahrát gameplay sekvenci a analyzovat vizuální bugy přes extrahované framy. (Fáze F)

### 2.2 Functional Goals (v4.3 — plánováno)
- ✅ Claude dokáže ovládat libovolné Unity Editor GUI okno bez ručního klikání — klikání na tlačítka, panely, kontextová menu. (Fáze G — UI Toolkit-native windows; IMGUI vyžaduje fallback)
- ❌ Claude dokáže vygenerovat texturu z textového promptu a uložit ji přímo jako Unity asset. (Fáze H)
- ❌ Claude dokáže vygenerovat zvukový efekt z textového promptu a uložit ho jako Unity audio clip. (Fáze H)
- ❌ Claude před každou netriviální implementací vypíše strukturovaný implementační plán a čeká na schválení. (Fáze I)
- ❌ Claude dokáže importovat Figma design soubor a převést ho na Unity UI Canvas s komponentami. (Fáze I)
- ❌ Claude dokáže rozmístit objekty ve scéně podle referenčního obrázku nebo concept artu. (Fáze I)

### 2.3 Non-Functional Goals
- ✅ Reload survival: všechny async operace přežijí domain reload.
- ✅ Bezpečnost: žádná destruktivní operace bez explicit confirmation parametru.
- ✅ Backward compat: všechny v4.1 a v4.2 commandy fungují identicky.
- ❌ Latence runtime field read ≤ 50 ms (file-based polling: ~32–100 ms — rozhodnutí finální, named pipe se neimplementuje).
- ⚠️ UI automatizace: spolehlivost ≥ 90 % pro UI Toolkit-native windows ověřena na Package Manager (67/67 elementů discovery + click + set OK). IMGUI windows (NavMesh, Lighting v Unity 6.4) vyžadují execute_menu_item nebo Computer Use fallback. (Fáze G)
- ❌ Texture generování: round-trip (prompt → Unity asset) ≤ 30 s. (Fáze H)
- ❌ Plan mode: Claude vygeneruje implementační plán do 10 s od zadání. (Fáze I)

### 2.4 Success Criteria
- ✅ Claude opraví regresi v gameplay mechanice end-to-end bez lidského "zkus to" pingu. (možné od Fáze B)
- ✅ Test Runner výsledek dostupný do 60 s pro typický PlayMode test.
- ✅ Profiler snapshot detekuje 30 % framerate drop. (Fáze D)
- ⚠️ Claude nastaví NavMesh a Lightmap bake — UI Toolkit cesta vyžaduje by Unity přepsal tyto windows do UI Toolkit (v Unity 6.4 jsou IMGUI). Současný pattern: `execute_menu_item` na bake command nebo `navmesh_bake` / `lightmap_bake` UCAF appcommandy. (Fáze G)
- ❌ Claude vygeneruje kompletní sadu UI assetů (textury, ikony) pro Siege of the Blue World HUD z promptů. (Fáze H)
- ❌ Claude dostane concept art → sestaví základní Unity scénu s rozmístěnými objekty. (Fáze I)

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
| `UCAF_Listener.AnimatorController.cs` | FR-119–126 | ✅ |
| `UCAF_Listener.NavMesh.cs` | FR-138–141 | ✅ |
| `UCAF_Listener.Lightmap.cs` | FR-165–166 | ✅ |
| `UCAF_Listener.BuildPlayer.cs` | FR-134–135 | ✅ |
| `UCAF_Listener.Profiler.cs` | FR-151–153 | ✅ |
| `UCAF_Listener.Timeline.cs` | FR-154–159 | ✅ |
| `UCAF_Listener.ShaderGraph.cs` | FR-160–164 | ✅ |
| `UCAF_Listener.Terrain.cs` | FR-167–170 | ✅ |
| `UCAF_Listener.Recording.cs` | FR-186–191 | ✅ |
| `UCAF_Listener.UIAutomation.cs` | FR-292–298 (Fáze G) | ✅ v1.2.0 |
| `UCAF_Listener.AssetGeneration.cs` | FR-199–205 (Fáze H) | ❌ |
| `UCAF_Listener.PlanMode.cs` | FR-206–210 (Fáze I) | ❌ |
| `UCAF_Listener.FigmaImport.cs` | FR-211–215 (Fáze I) | ❌ |
| `UCAF_Skills/` | Skills architektura (Fáze J) | ❌ |

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

### 4.47 Unity UI automatizace ✅ DONE — Fáze G (FR-292 až FR-298)

> **Číslování:** FR-192..198 v původní v4.3 koliduje s FR-192 `search_packages` (sekce 4.35). Per v4.4 delta (sekce 5, E-1): nové číslování **FR-292..FR-298**. Implementace UCAF v1.2.0, commit `07e9caa…`.

**Záměr:** Claude ovládá Unity Editor GUI přes Unity UI Toolkit Query API — cílené příkazy na konkrétní UI prvky, ne slepá simulace myší souřadnic. Primární: UI Toolkit. Fallback pro IMGUI / nepokryté cases: Computer Use (dry-run hotov, exekuce odložena).

**Proč ne Computer Use jako primární:** Editor GUI se dynamicky mění (kontextová menu, floating windows), Claude by musel znát přesné pixelové souřadnice UI prvků. UI Toolkit API vrací reference na konkrétní pojmenované prvky — spolehlivé a verzi-stabilní.

**Co odemkne:**
- Ovládání Package Manageru bez ručního klikání (smoke-tested: 67 interaktivních prvků discovery + click + set OK)
- Project Settings / Preferences (UI Toolkit-native)
- Inspector / Hierarchy / Project Browser (hybridní; UI Toolkit části fungují)
- Kontextová menu — odemyká `Edit/Build/Window/Assets` přes `execute_menu_item`, GUI elementy přes ui_click

**FR-292 `ui_click`** — params `window`, `element_name`. Najde element přes `rootVisualElement.Q<>()`. Click strategie:
- `Button` / focusable: `NavigationSubmitEvent.GetPooled() → el.SendEvent(evt)` (verzí-stabilní; vyhne se reflection na `Clickable._clickable`)
- `Toggle`: `value = !value`
- `Foldout`: `value = !value`

**FR-293 `ui_set_field`** — params `window`, `element_name`, `value`. Type dispatch: `TextField`, `IntegerField`, `LongField`, `FloatField`, `DoubleField`, `Toggle`, `Slider`, `SliderInt`, `DropdownField`, `ObjectField` (asset path → `LoadAssetAtPath`), `EnumField` (parse by name z initial value type), `Vector2Field`/`Vector3Field`/`Vector4Field`/`ColorField` (přes reflection na `value` property + InvariantCulture parsing).

**FR-294 `ui_drag_asset`** — params `target_window`, `target_element`, `asset_path`. **Honest gap:** value-set semantics na `ObjectField` only — ekvivalentní výstup jako drag & drop ale interně přes `ObjectField.value = AssetDatabase.LoadAssetAtPath<>(path)`. Non-ObjectField target vrací `error_code=drag_target_not_supported` (fail-fast s hint). Pro IMGUI / generic drops → `ui_computer_use_fallback`.

**FR-295 `ui_open_window`** — param `window_type` (key z hardcoded mapping: PackageManager, ProjectSettings, Preferences, Inspector, Hierarchy, Project, Console, Scene, Game, Lighting, LightingExplorer, NavMesh, Animation, Profiler, TestRunner). Strategie:
1. Reflection: `Type.GetType("UnityEditor.X, UnityEditor")` → `EditorWindow.GetWindow(t)`
2. Fallback: `EditorApplication.ExecuteMenuItem("Window/...")` → `EditorWindow.focusedWindow`

Returns `{title, type_name, has_root_visual_element, was_already_open, strategy}` — `has_root_visual_element=false` signalizuje IMGUI okno (Q<>() nebude fungovat).

**FR-296 `ui_get_element_state`** — params `window`, `element_name`. Vrátí `{name, label, type, enabled, visible, value}` pro single element. Verifikační read po FR-292 click.

**FR-297 `ui_list_elements`** — param `window`. Discovery: výpis všech interaktivních prvků (Button, Toggle, *Field, Foldout, Slider, ObjectField, EnumField, DropdownField, RadioButton). Returns `{name, label, type, enabled, visible, value}` pro každý — name + label v obou variantách, takže matching v dalších commandech přes label fallback nepotřebuje prior list.

**FR-298 `ui_computer_use_fallback`** — params `instruction`, `window` (optional), `dry_run` (default true), `model`, `max_tokens`. Volá Anthropic Messages API s `computer_20250124` toolem, screenshotem Scene View. Návrat `{actions[], stop_reason, input_tokens, output_tokens}`. **Honest gap:** `dry_run=true` (default) je single-turn — vrátí proposed actions, neexekuuje. `dry_run=false` vrací `error_code=not_implemented` — multi-turn agent loop + mouse event injection do EditorWindow odložen na další session. Vyžaduje `ANTHROPIC_API_KEY` v Unity process env.

**Element matching:**
1. Exact name (`rootVisualElement.Q<VisualElement>(name)`) first
2. Case-sensitive label/text fallback (Button.text, Foldout.text, BaseField.label přes reflection)
3. Case-insensitive label fallback

Tj. `ui_click element_name=Bake` najde `Button` s `name="m_BakeButton"` AND `text="Bake"` bez prior list call.

**Architektonická omezení (potvrzená v praxi):**
- **IMGUI windows** (Unity 6.4 stále: Lighting, NavMesh, Animation, většina Inspector pomocných panelů) nemají reachable `rootVisualElement.Q<>()` výsledky — `has_root_visual_element=true` ale 0 elementů, nebo přímo `false`. Pro tyto:
  - Použít `execute_menu_item` na shortcut commands (Build, Asset menu) — UCAF FR-186
  - Použít specifický UCAF command (`navmesh_bake`, `lightmap_bake`, `add_animator_state`)
  - Future: `ui_computer_use_fallback dry_run=false` po multi-turn implementaci
- **Drag & drop** je v Unity GUI silně bound na pointer events; FR-294 obchází to value-set semantics na ObjectField (cca 70 % real use case). Mimo ObjectField (Project tab drop, Scene View drop) zatím neřešeno.
- **Modální dialogy** (FileBrowser, ImportPackageDialog) běží mimo UI Toolkit world — žádný API access. Workaround: vyhnout se modálním dialogům, používat C# API (AssetDatabase) místo GUI dialogu.

**Smoke test evidence (UCAF v1.2.0, hash `07e9caa…`, 2026-05-21):**
- FR-295 ui_open_window PackageManager → reflection strategy, ui_toolkit=true
- FR-297 ui_list_elements PackageManager → 67 elementů s name + label
- FR-296 ui_get_element_state toolbarFiltersMenu → {type=ToolbarWindowMenu, enabled=true}
- FR-292 ui_click toolbarFiltersMenu → action=submit (otevřel dropdown)
- FR-293 ui_set_field "Sources" → value=False (z True), restored. **Pozor:** "Sources" label se v Package Manageru shoduje na 2 prvky (Foldout + Toggle) — UI_FindElement vrátil Foldout first → kód šel přes reflection-default branch, ne přes explicit `case Foldout`. Typed dispatch pro TextField/IntegerField/FloatField/EnumField/ObjectField/Vector*Field/ColorField **nebyl empiricky ověřen v této session** — funkční po-construction (compile pass + reflection fallback ověřen), ale první real use case může odhalit per-type bug.
- FR-294 ui_drag_asset → drag_target_not_supported (negative path expected)
- FR-298 ui_computer_use_fallback dry_run=true → missing_api_key (expected; honest gap zdokumentován)
- read_only=true mode: ui_get_element_state + ui_list_elements allowed; ui_click + ui_set_field + ui_drag_asset + ui_open_window + ui_computer_use_fallback všechny blocked s `read_only_mode_blocked` (sdílí stejný dispatcher gate — všech 5 ověřeno)

**Známé omezení matchování labels:** UI_FindElement vrací **první** element s matching label při Query walk. Pokud má více elementů stejný label (Foldout + Toggle "Sources", multiple slots labeled "Asset"), není současný způsob jak adresovat n-tý match. Workaround: použít exact `name` (UI Toolkit unique identifier) místo label. Future enhancement: param `match_index` nebo `match_type` (typeName filter).

---

### 4.48 Generování assetů přes external API ❌ TODO — Fáze H (FR-199 až FR-205)

**Záměr:** Claude generuje textury a zvuky z textových promptů a ukládá je přímo jako Unity assety. Překonává klíčovou výhodu Unity AI Assistant (generators). Implementováno jako volání externích API — žádné nové ML modely v UCAF.

**Proč textury a zvuky — ne animace ani 3D:** Text-to-animation a text-to-3D API jsou k 2026-05 nezralé (špatná topologie pro Unity rig, nestabilní výsledky). Textury a zvuky mají robustní, produkčně stabilní API (DALL-E 3, Stable Diffusion, ElevenLabs Sound Effects, Suno).

#### Textury (FR-199 až FR-202)

**FR-199 `generate_texture`** — `prompt`, `style=realistic|stylized|pbr_albedo|normal_map|roughness`, `resolution=512|1024|2048`, `output_path=Assets/Textures/...`. Volá image API (DALL-E 3 nebo SD API, konfigurovatelné v `ucaf_config.json`). Výsledek uložen jako PNG, automaticky nastaveny import settings dle `style`.

**FR-200 `generate_texture_set`** — `prompt`, generuje sadu PBR map najednou: albedo + normal + roughness + metallic. Automaticky vytvoří HDRP Lit Material s napojenými texturami.

**FR-201 `generate_sprite`** — `prompt`, `style=icon|ui_element|character_portrait`, `background=transparent`. Výstup PNG s alpha kanálem, import settings `TextureType=Sprite`.

**FR-202 `texture_variation`** — `source_asset_path`, `variation_prompt`. Vezme existující texturu, vygeneruje variaci (jiná barva, poškození, worn look).

#### Zvuky (FR-203 až FR-205)

**FR-203 `generate_sfx`** — `prompt`, `duration_seconds=1-10`, `output_path=Assets/Audio/...`. Volá ElevenLabs Sound Effects API nebo Suno API (konfigurovatelné). Výstup WAV/OGG, automaticky nastaveny audio import settings (`LoadType`, `CompressionFormat`).

**FR-204 `generate_music`** — `prompt`, `duration_seconds=30-180`, `style=ambient|combat|menu`. Pro herní hudbu. Delší generování (~30–60 s), async.

**FR-205 `generate_sfx_variation`** — `source_asset_path`, `variation_count=3`. Vygeneruje N variací stejného zvuku (kroky na různých površích, variace výstřelů) pro přirozenější audio.

**Konfigurace v `ucaf_config.json`:**
```json
{
  "asset_generation": {
    "image_api": "dalle3",
    "image_api_key_env": "OPENAI_API_KEY",
    "audio_api": "elevenlabs",
    "audio_api_key_env": "ELEVENLABS_API_KEY",
    "default_texture_resolution": 1024
  }
}
```

---

### 4.49 Plan mode, Figma import, Scene z obrázku ❌ TODO — Fáze I (FR-206 až FR-215)

#### Plan mode (FR-206 až FR-208)

**Záměr:** Claude před každou netriviální implementací (>1 soubor, >50 řádků, architektonické rozhodnutí) vygeneruje strukturovaný implementační plán a uloží ho do `ucaf_workspace/plans/{id}.md`. Lidský lead plán schválí jedním příkazem, teprve pak Claude implementuje. Odpovídá Unity AI "Plan mode".

**FR-206 `plan_task`** — `description`, `scope=quick|standard|architecture`. Claude analyzuje popis, prohledá existující kód (`code_grep`), vygeneruje plán: affected files, steps, risks, estimated commands. Uloží do `ucaf_workspace/plans/{plan_id}.md`. Vrátí `plan_id`.

**FR-207 `plan_approve`** — `plan_id`. Označí plán jako schválený, Claude začne implementovat. Bez schválení Claude implementaci nespustí (pokud `scope != quick`).

**FR-208 `plan_list`** — výpis plánů: pending, approved, completed, abandoned.

#### Figma → Unity UI (FR-209 až FR-211)

**Záměr:** Import Figma design souboru do Unity jako UI Canvas s komponentami. Kritické pro Siege of the Blue World HUD, inventory screen, quest log.

**FR-209 `figma_import`** — `figma_url` nebo `figma_file_path` (lokální export JSON). Parsuje Figma JSON, mapuje frames → Canvas, rectangles → Image komponenty, text → TextMeshPro, groups → CanvasGroup. Výsledek: prefab v `Assets/UI/{frame_name}.prefab`.

**FR-210 `figma_sync`** — `figma_url`, `target_prefab`. Aktualizuje existující prefab při změně Figma souboru — diffuje změny, zachovává C# script binding.

**FR-211 `figma_list_frames`** — diagnostika: výpis framů v Figma souboru před importem. Pro výběr co importovat.

**Omezení:**
- Komplexní Figma komponenty (Auto Layout, Variants) mapovány na best-effort základ
- Custom Figma pluginy a efekty ignorovány
- Vyžaduje Figma REST API token nebo lokální JSON export

#### Scene z obrázku / reference (FR-212 až FR-215)

**Záměr:** Claude dostane referenční screenshot nebo concept art → analyzuje rozmístění objektů přes vision → vydá sérii příkazů pro sestavení Unity scény. Řeší vizuální mezeru — Claude nemusí "vidět" Scene View v reálném čase, ale interpretuje statický referenční obraz.

**FR-212 `scene_from_reference`** — `image_path` nebo `image_url`, `target_scene`, `asset_library_path=Assets/`. Claude přes vision analyzuje obrázek, identifikuje typy objektů (budova, strom, cesta, postava), mapuje na dostupné prefaby v `asset_library_path`, vydá sérii `create_gameobject` + `set_component` příkazů s aproximovanými pozicemi.

**FR-213 `scene_reference_diff`** — `reference_image`, `scene_screenshot`. Porovná referenci s aktuálním stavem scény (screenshot z UCAF), identifikuje co chybí nebo je špatně umístěno.

**FR-214 `scene_layout_from_description`** — `description` (text místo obrázku). Claude navrhne rozmístění objektů z textového popisu ("les s cestou uprostřed, modul lodi v pozadí"). Generuje placement plán, čeká na schválení před exekucí (propojeno s Plan mode FR-206).

**FR-215 `scene_annotate`** — pořídí screenshot scény, Claude přes vision anotuje objekty (jméno GameObjectu, komponenty, pozice). Pro orientaci v komplexní scéně.

---

### 4.50 Modulární Skills architektura ❌ TODO — Fáze J

**Záměr:** Refaktorizace UCAF na modulární skills systém — každá doména (Cinemachine, NavMesh, ShaderGraph, UI, ...) jako samostatný skill modul s vlastní dokumentací, příklady a kontextem. Odpovídá Unity AI "specialized skills". Umožňuje Claude vybrat a načíst relevantní kontext per task místo monolitického toolsetu.

**Architektura:**
- `UCAF_Skills/` složka — každý skill je složka s `skill.md` (popis, příklady, omezení) + případně helper C# kód
- Claude před netriviálním taskem identifikuje relevantní skills a načte jejich `skill.md` do kontextu
- Skills jsou verzované a aktualizovatelné nezávisle na core UCAF

**Plánované skills:**
- `skills/cinemachine/` — TPS kamera, virtuální kamery, blend profily
- `skills/navmesh/` — pathfinding, agent konfigurace, NavMesh Surfaces
- `skills/shadergraph/` — HDRP materiály, properties, URP konverze
- `skills/ui/` — Canvas setup, TextMeshPro, responsive layout
- `skills/animator/` — Blend trees, AvatarMask, locomotion setup
- `skills/vfx/` — VFX Graph templates, particle systems
- `skills/audio/` — Audio Mixer, spatial audio, footstep systém
- `skills/terrain/` — heightmap, texture layers, tree placement

**FR-216 `skill_list`** — výpis dostupných skills.

**FR-217 `skill_load`** — `skill_name`. Načte `skill.md` do Claude kontextu pro aktuální task.

**FR-218 `skill_suggest`** — `task_description`. Claude na základě popisu tasku doporučí které skills načíst.

---

## 5. Non-Functional Requirements

| NFR | Požadavek | Status |
|-----|-----------|--------|
| NFR-12 | Runtime field read ≤ 50 ms | ❌ ~32–100 ms (file-based; named pipe zamítnut) |
| NFR-13 | PlayMode enter→exit UCAF overhead ≤ 1 s | ✅ |
| NFR-14 | Build Player log streaming (progress) | ✅ Fáze C |
| NFR-15 | Test Runner per-test výsledky | ✅ |
| NFR-16 | Frame extrakce dokončena do 10 s po `recording_stop` (pro záznam ≤60 s, PNG sequence) | ✅ Fáze F |
| NFR-17 | Disk usage: automatický cleanup záznamů starších 7 dní | ✅ Fáze F |
| NFR-18 | UI automatizace: spolehlivost ≥ 90 % pro standardní Editor operace | ❌ Fáze G |
| NFR-19 | Texture generování: round-trip (prompt → Unity asset) ≤ 30 s | ❌ Fáze H |
| NFR-20 | SFX generování: round-trip ≤ 15 s pro clip ≤ 5 s | ❌ Fáze H |
| NFR-21 | Plan mode: plán vygenerován do 10 s | ❌ Fáze I |
| NFR-22 | Figma import: frame s ≤50 prvky zpracován do 30 s | ❌ Fáze I |

---

## 6. Risks & Mitigations

| Riziko | Mitigace |
|--------|----------|
| Runtime bridge crashne hru, Editor zůstane v PlayMode | 5s timeout v `CheckPendingRuntimeProxy` ✅ |
| Build Player zaplní disk | Cleanup po 5 buildech ✅ Fáze C |
| Animator API změny mezi Unity verzemi | Version detection + fallback na `edit_file` ✅ Fáze C |
| ShaderGraph rozbití přes API | Pouze properties, ne nodes; git revert ✅ Fáze D |
| Unity Recorder package chybí | `recording_start` vrátí jasný error s `add_package com.unity.recorder` instrukcí ✅ Fáze F |
| PNG sekvence zaplní disk | NFR-17 auto-cleanup + `recording_delete older_than_days=7` ✅ Fáze F |
| MP4 encoding blocker na headless | Fallback na `format=png_sequence`; MP4 je optional ✅ Fáze F |
| Input System absentuje | `#if ENABLE_INPUT_SYSTEM` + jasný error s instrukcí ✅ |
| Domain reload ztrácí PM state | SessionState backup + `CheckPendingPackageManagerOnStartup` ✅ |
| Partial class field init order | WorkspacePath-závislé fieldy v UCAF_Listener.cs nebo static ctor ✅ |
| UI Toolkit nepokrývá IMGUI prvky (~20 %) | Computer Use fallback (FR-198) — pomalejší ale funkční (Fáze G) |
| External image API rate limiting | Retry s exponential backoff; error s čitelnou instrukcí (Fáze H) |
| External image API klíče v projektu | Klíče pouze přes env proměnné (`OPENAI_API_KEY`), nikdy v kódu nebo `ucaf_config.json` commitovaném do gitu (Fáze H) |
| Figma JSON format se mění | Parser verzovaný dle Figma API verze; fallback na manuální mapping (Fáze I) |
| Scene z obrázku — špatné rozpoznání objektů | Plan mode preview před exekucí; `scene_reference_diff` pro verifikaci (Fáze I) |
| Skills architektura zvyšuje context window | Skills načítány selektivně per task, ne všechny najednou (Fáze J) |

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

**Fáze G ❌ TODO — Unity UI automatizace**
- FR-192–198 UI Toolkit API + Computer Use fallback
- Priorita: `ui_click`, `ui_set_field`, `ui_open_window` jako první
- Závislost: Unity 2022.3+ (UI Toolkit stabilní)

**Fáze H ❌ TODO — Generování assetů**
- FR-199–202 Texture generování (DALL-E 3 / SD API)
- FR-203–205 SFX + Music generování (ElevenLabs / Suno)
- Priorita: FR-199 `generate_texture` + FR-203 `generate_sfx` jako první — největší okamžitá hodnota
- Závislost: external API klíče v env proměnných

**Fáze I ❌ TODO — Plan mode + Figma + Scene z obrázku**
- FR-206–208 Plan mode
- FR-209–211 Figma → Unity UI
- FR-212–215 Scene z obrázku / reference
- Priorita: FR-206 Plan mode jako první — nejjednodušší implementace, vysoká hodnota
- Závislost: Figma REST API token (pro Figma import)

**Fáze J ❌ TODO — Modulární Skills architektura**
- FR-216–218 Skills system
- Obsahuje refaktorizaci: vytvoření `UCAF_Skills/` složky s 8 skill moduly
- Závislost: všechny předchozí fáze stabilní (J je organizační vrstva nad nimi)

---

## 8. Bug Ledger

Platí vše z v4.1 + tři nové typy z 4.45. ✅

---

## 9. Out of Scope (v4.3)

- Asset Store integrace (placené balíčky, license keys)
- Cloud Build / Unity Cloud
- VR / XR specifické flows
- Mobile signing / certificate management
- Plnotučný ShaderGraph node editor (pouze properties)
- AI/ML inference (Sentis, ONNX)
- Multiplayer / NetCode tooling
- Named pipe runtime channel (zamítnut — file-based IPC dostatečný)
- WebSocket console streaming (zamítnut — file-based streaming v Fázi E)
- Přímá analýza MP4 videa (Fáze F: pouze extrahované PNG framy)
- Audio analýza záznamů
- Text-to-animation generování (trh nezralý, špatná topologie pro Unity rig)
- Text-to-3D generování (kvalita API nedostatečná pro produkci)
- Schvalování každé akce (záměrně autonomní; volitelný `--confirm` flag pro destruktivní operace)
- Unity Cloud / Unity AI Assistant integrace (souběžný, ne nadřazený nástroj)

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
- **UI Toolkit API** — Unity interní API pro automatizaci Editor GUI přes pojmenované prvky (`rootVisualElement.Q<Button>("Bake")`); základ Fáze G.
- **Plan mode** — Claudeův workflow: analýza → vygenerování plánu do `ucaf_workspace/plans/` → čekání na `plan_approve` → implementace. Fáze I.
- **Skill** — modulární doménový kontext v `UCAF_Skills/{name}/skill.md`; Claude načítá selektivně per task. Fáze J.
- **Asset generation** — generování textur nebo zvuků z textového promptu přes external API (DALL-E, ElevenLabs) a uložení jako Unity asset. Fáze H.

---

## 11. Stav v4.2 → v4.3

| Oblast | v4.2 | v4.3 plán |
|--------|------|-----------|
| Unity UI automatizace | ❌ (out of scope) | ❌ Fáze G — UI Toolkit API + Computer Use fallback |
| Generování textur | ❌ | ❌ Fáze H — DALL-E 3 / SD API |
| Generování zvuků | ❌ | ❌ Fáze H — ElevenLabs / Suno API |
| Generování animací | ❌ | ❌ Out of scope (trh nezralý) |
| Generování 3D objektů | ❌ | ❌ Out of scope (kvalita nedostatečná) |
| Plan mode | ❌ | ❌ Fáze I — plán před implementací |
| Figma → Unity UI | ❌ | ❌ Fáze I — Figma JSON → Canvas prefab |
| Scene z obrázku | ❌ | ❌ Fáze I — vision-driven placement |
| Modulární Skills | ❌ | ❌ Fáze J — UCAF_Skills/ architektura |
| Unity AI parrita | Chybí: assety, GUI, plan | Po G+H+I: parrita v autonomii, UCAF vede v testování |

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

## 12.5 Backlog — příkazy k implementaci v budoucí verzi

| Příkaz | Popis | Priorita |
|--------|-------|----------|
| `instantiate_prefab` | Vytvoří instanci prefabu ve scéně za běhu — ekvivalent `Instantiate(prefab, position, rotation)`. Obecně použitelné napříč projekty. | střední |

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
