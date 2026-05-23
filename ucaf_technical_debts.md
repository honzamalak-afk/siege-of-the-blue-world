# UCAF Technical Debts

> Living log otevřených tech debtů, deferred features, honest gaps a known gotchas napříč UCAF kódem.
> Aktualizuj při každé session, která přidá / vyřeší tech debt. Před řešením staršího debt: ověř, že je stále relevantní (může být obsolete kvůli pozdějším změnám).
>
> **Konvence statusu:**
> - 🔴 OPEN — blocking nebo bolestivé, plán řešení definovaný
> - 🟡 DEFERRED — vědomě odložené, čeká na trigger / decision
> - 🟢 ACCEPTED — vědomě přijatý kompromis, žádný akční bod
> - ✅ RESOLVED — pro audit, smazat po 2 sessions

---

## Phase H A+B (v1.3.0, 2026-05-22)

### Deferred features

**TD-H1 🟡 `generate_sfx` (FR-203) — no A+B equivalent pro audio**
- Důvod: ElevenLabs vyřazen v session 2026-05-22 (žádné externí API), ale lokální audio gen (AudioLDM2 / Stable Audio / Bark) vyžaduje samostatný Python stack ~10 GB.
- Trigger pro řešení: Honza explicitně chce audio gen + ochota strávit ~2-3 sessions na lokální stack setup, nebo opt-in ElevenLabs subscription pro tento projekt.
- Související: FR-204 (`generate_music`), FR-205 (`generate_sfx_variation`).

**TD-H2 🟡 Voronoi + Cellular procedural generators**
- Vynechány z v1.3.0 per Honzův select (advisor "tři podobné řádky lepší než předčasná abstrakce").
- Trigger: Honza najde real use case pro rock/cellular patterns ve hře.
- Cena: ~0.5 session per generator.

**TD-H3 🟡 `texture_variation` (FR-202) přes img2img**
- Forge má `/sdapi/v1/img2img` endpoint, snadný add.
- Trigger: Honza chce variovat existující texturu (worn version, recolor, damage).
- Cena: ~0.5 session.

**TD-H4 🟡 `generate_texture_set` (FR-200) — PBR multi-call**
- Albedo + normal + roughness + metallic z jednoho promptu, plus HDRP Lit Material wiring.
- Trigger: Honza dělá real PBR assety, ne jen koncepty.
- Cena: ~1 session (orchestrace 4 calls + Material asset graph).

**TD-H5 🟡 `generate_terrain_layer` (FR-222)**
- Depend na TD-H4 (PBR set).
- Cena: ~0.5 session nad TD-H4.

**TD-H6 🟡 `provider=auto` fallback chain**
- Implementační kostra: zkusit procedural (zero dep) → fallback local_sd → fallback "use create_script for custom procedural".
- Trigger: Honza dělá batch generation a chce robust fallback.
- Risk: premature abstraction; teď je explicit volba lepší pro learning.

### Honest gaps (accepted compromises)

**TD-H7 🟢 SD path requires GPU + Forge běž**
- Single-machine dependency. Sandbox / CI bez GPU = SD path nefunkční (vrací `error_code=sd_endpoint_unreachable`).
- Procedural path funguje vždy.
- Accepted, dokumentováno v PRD v4.5 sekce 3.

**TD-H8 🟢 Sprite alpha floodfill — heuristic only**
- Subjects s tenkými průhlednými detaily (peří, vlasy, sklenice) budou misclassified.
- Pro icon-style sprites stačí; pro character portraits → bg-removal AI tool later.
- Trigger pro upgrade: Honza dělá character portraits a kvalita nevyhovuje.

**TD-H9 🟢 Cubemap procedural = 6-face gradient skybox only**
- Žádné mraky, ne hvězdy, jen color blend top → horizon → bottom.
- Real procedural sky (SDFs, clouds) = samostatný command later.

**TD-H10 🟢 Sync HTTP zamrzne Editor 5-60 s**
- `EditorUtility.DisplayProgressBar` mitigation (stejný pattern jako `ui_computer_use_fallback`).
- Async přes `EditorApplication.update` polling = refactor, ne worth pro v1.3.0.

**TD-H11 🟢 `style=normal_map` na SD path generuje fake normal**
- SD nevygeneruje real tangent-space normal data (RGB ≠ XYZ vector). HDRP lighting nebude fyzikálně korektní.
- Procedural `normal_from_heightmap` je correct alternativa.
- Documented v PRD v4.5, ne hard error.

**TD-H12 🟢 SD `resolution=512` request = downscale 1024 SD output**
- Contract: "I want N px asset" (via `maxTextureSize`), ne "I want SD to generate at N px".
- Pokud Honza chce native 512×512 SD output → změnit kód aby `width/height` v request bodu odpovídalo `resolution`. Současný code hardcode `width=resolution, height=resolution`. **Wait — tohle už dělá.** Re-check: ano, request body `width:resolution, height:resolution` → SD generuje native 512. `maxTextureSize` v importeru = stejná hodnota → no downscale. Note: smazat tento debt po verifikaci v live testu. (Advisor's flag byl false alarm při code review.)

### Migration / format risks

**TD-H13 🟡 Existing assety taggované `ucaf_version=1.1.0` mají old marker format**
- Bez `ucaf_provider` field. JsonUtility default-fillne `""` (žádný crash), ale `list_generated_assets` zobrazí prázdný provider.
- Trigger pro migrace: pokud bude existovat významný počet starých taggovaných assetů. Aktuálně k 2026-05-22 = 0 (Phase H nebyla nikdy live), debt teoretický.

---

## Phase G (v1.2.0, 2026-05-21) — carryover

**TD-G1 🟡 `ui_set_field` typed dispatch unverified empirically**
- Smoke test z předchozí session šel přes reflection-default branch (label match na Foldout místo Toggle).
- Explicit `case TextField / IntegerField / FloatField / EnumField / ObjectField / Vector*Field / ColorField / Slider` v switch nebyl exercised.
- Konstrukčně OK (compile pass), ale první real use case může najít per-type bug.
- Doporučená cesta: před produkčním use case napsat smoke test sequence proti Project Settings (TextField + IntegerField), Inspector na primitive (ObjectField + Vector3Field).

**TD-G2 🟡 Ambiguous label resolution v `UI_FindElement`**
- Vrací FIRST match přes Query walk. Pokud má víc elementů shodný label (Foldout + Toggle "Sources"), n-tý match není přístupný.
- Workaround: použít exact `name` místo label.
- Future enhancement: `match_index` nebo `match_type` filter param. ~0.5 session.

**TD-G3 🟡 FR-298 `dry_run=false` (Computer Use multi-turn execute) odložen**
- Plný flow vyžaduje multi-turn Anthropic Messages API + screenshot loop + mouse event injection + async state machine.
- Honest odhad: ~2-3 sessions s nejistotou ohledně end-to-end test (real ANTHROPIC_API_KEY v Unity env).
- Trigger: Honza potřebuje fallback pro IMGUI okna nebo Scene View interakce které Phase G UI Toolkit cesta neuvládne.

**TD-G4 🟢 IMGUI windows (Lighting, NavMesh v Unity 6.4) — `ui_click/set` neuspěje**
- `has_root_visual_element=false`.
- Workaround dokumentován v PRD 4.47: použít specifický UCAF command (`navmesh_bake`, `lightmap_bake`) nebo `execute_menu_item`.

**TD-G5 🟢 Drag mimo ObjectField (Project tab, Scene View drop) — žádný API pattern**
- Plný FR-294 by vyžadoval `Event.DragPerformEvent` simulaci přes IMGUI vrstvu nebo Computer Use fallback.
- Současný `ui_drag_asset` umí jen ObjectField value-set semantics.

---

## Cross-cutting (architektura)

**TD-X1 🔴 `EnumerateAllCommandTypes` je manuální mirror dispatcher switch**
- Hardcoded list v `UCAF_Listener.MCPServer.cs:629` musí být kept in sync s switch v `UCAF_Listener.cs:240`.
- Drift risk: nový command přidaný do dispatch, zapomenutý v enumeration → MCP `tools/list` ho nezná, ale dispatch funguje.
- Future fix: source-generator nebo reflection-based discovery. ~0.5 session.

**TD-X2 🟡 UCAF_Types.cs vs UCAF_Types.v44.cs vs UCAF_Types.PhaseH.cs — 3 separate files**
- Per-delta strategy "kept separate for clean review". Při 5+ phases bude chaos.
- Future fix: konsolidace do jednoho `UCAF_Types.cs` po stabilizaci Phase H/I/J. ~0.25 session.

**TD-X3 🟡 PRD chain v4.3 → v4.4 → v4.5 = delta architecture**
- Risk: future change loses context across multiple PRD files. Future Honza může číst jen v4.5, miss kontextu z v4.3.
- Future fix: konsolidace do `PRD_Unity_Claude_Framework_v5.md` jako single source of truth. ~1 session.

**TD-X4 🟢 `.meta` soubory pro nové C# v packagi POVINNÉ**
- Unity Package Manager považuje git packages za immutable folder; bez `.meta` ignoruje `.cs` soubory.
- Pattern: `printf 'fileFormatVersion: 2\nguid: <32hex>\n...' > X.cs.meta` se random GUID.
- Mitigated by knowledge — žádný structural fix possible (Unity behavior).

**TD-X5 🟢 Console buffer "clear before compile_and_wait" konvence**
- UCAF log buffer drží 1000 záznamů + persists do `ucaf_workspace/logs/buffer.ndjson`. Po compile error fix vždy `clear_console` před dalším `compile_and_wait` — jinak vrátí stale errors.
- Ne enforced v kódu, je to workflow konvence.

**TD-X6 🟡 Stale Library/PackageCache po `add_package` s novým hashem**
- Občas Unity nereaguje na nový package hash — drží cached starou verzi.
- Workaround: smazat `Library/PackageCache/com.ucaf@<old_hash>/` ručně.
- Triggers: ne každý session, nejasný pattern. Sledovat četnost.

---

## Setup / Environment

**TD-S1 🟡 `test_sd_endpoint.py` unicode bug**
- `print(f"... → ...")` na Windows cp1250 CZ codepage = `UnicodeEncodeError`.
- Fix: nahradit `→` ASCII šipkou `->`, nebo set `PYTHONIOENCODING=utf-8`.
- Location: `ucaf_workspace/setup/test_sd_endpoint.py`.

**TD-S2 🔴 `A1111_SETUP.md` je DEPRECATED (A1111 broken proti 2026 stavu)**
- AUTOMATIC1111 v1.10.1 hardcoded `Stability-AI/stablediffusion` repo deleted = install fail.
- Forge je drop-in replacement (`lllyasviel/stable-diffusion-webui-forge`).
- Action: smazat starý `A1111_SETUP.md`, napsat `FORGE_SETUP.md` s working postupem.
- Cena: ~0.25 session.

**TD-S3 🟢 Bash background spawn ZLOMÍ A1111/Forge launchers**
- Quoting/CWD artefakty.
- Workaround: A1111/Forge spustit Honza ručně v PS okně, ne přes Claude bash. Endpoint pak ověřit zvenku curlem.

**TD-S4 🟢 PyTorch nightly cu128 vyžadováno pro RTX 5060 Blackwell**
- Stable cu121/124/126 nemá sm_120 kernels.
- URL: `https://download.pytorch.org/whl/nightly/cu128`.
- Pin: pravděpodobně potřebuje update po stable cu128 release.

**TD-S5 🟢 NumPy 2.x rozbije skimage v Forge stack**
- Pin `numpy>=1.26,<2`.

**TD-S6 🟢 pip 26.1.1 + Python 3.10 + A1111/Forge stack = broken**
- Build isolation env nemá `pkg_resources`, CLIP build fail.
- Fix: downgrade `pip==23.2.1 setuptools==69.5.1 wheel`.

---

## Process / Workflow

**TD-P1 🟡 Honza PAT nutný pro každý push do `honzamalak-afk/ucaf`**
- Sandbox nemá persistent push credentials. Honza musí dodat přes `! export TOK='ghp_…'` před každým push.
- Memory `feedback_secret_handling` zakazuje echo. Length-check pattern.
- Future fix: setup deploy key nebo gh CLI auth in sandbox. ~0.5 session.

**TD-P2 🟡 PRD update vs impl pořadí — konvence**
- CLAUDE.md říká "PRD řídí vývoj". V této session Honza zvolil paralelní (PRD a kód souběžně, mark DONE po impl).
- Risk: PRD a kód se rozejdou pokud session selže mid-flight.
- Mitigation: vždy v commitu reference PRD verze + finalize task po passing live test.

**TD-P3 🟢 Domain reload po `add_package` nestačí**
- Pattern: `add_package` → `clear_console` → triviální `Assets/Scripts/_UCAFRecompileTrigger.cs` přes `create_script` → `compile_and_wait` (15+ s = real recompile s domain reload).
- Po ověření smaž trigger.
- Workflow konvence, ne enforced.

---

## Surfaced 2026-05-22 live test

**TD-H14 ✅ RESOLVED 2026-05-23 (v1.3.3 + v1.3.4 patch)** — Sprite alpha via `alpha_method` dispatch
- Live test "potion bottle icon" (regression, 3 backends all on same SD seed/prompt):
  - edge_color = **85.5 % alpha** (224 243 / 262 144 px, 1 attempt, 7054 ms)
  - rembg     = **99.6 % alpha** (260 994 / 262 144 px, 1 attempt, 62 251 ms first run incl. U2Net download)
  - floodfill = **0 % alpha**, 2 attempts (auto-retry verified, honest-gap message accurate)
- Implementation in v1.3.3 commit `d37cbd1` + v1.3.4 commit `ef544f6`: `alpha_method` param (default `edge_color`) + auto-retry on heuristic underfill + stronger sprite-mode prompt augmentation + opt-in `rembg` Python subprocess backend (async-safe stdout/stderr to avoid pipe-fill deadlock).
- **v1.3.4 fix**: rembg invocation switched from `python -m rembg` (rembg 2.x has no `__main__`) to `python -c "from rembg import remove; …"`. Also: better stderr-pattern hint matrix (recognises "No onnxruntime backend found" → `pip install rembg[cpu]`).
- Remaining honest gap: full-frame subjects touching canvas border can poison median edge color → use `alpha_method=rembg`. Tracked in PRD v4.5 section 2.4 + per-method honest_gap text.

**TD-X7 🟡 Sharing violation race condition v UCAF Poller**
- Sporadické `System.IO.IOException: Sharing violation on path ...commands\pending\cmd_*.json` při ReadAllText v `UCAF_Listener.Poll()` UCAF_Listener.cs:162.
- Race: Python helper píše file, Unity Poller čte současně, nebo write nedokončen.
- Mitigation: retry-on-sharing-violation v Poller (3 retries s 50ms backoff). ~10 řádků code.
- Bez akce: ~1 z 10 commandů náhodně failne, retry uspěje.

**TD-H15 🟢 Procedural assets default isReadable=false**
- Unity TextureImporter default. Auto-fix v `AssetGenNormalFromHeightmap` toggles flag on demand (v1.3.2).
- Side effect: source texture zůstane readable=true after auto-fix (persistent memory cost).
- Pro user-facing description: prozatím akceptováno; future option set `isReadable=true` univerzálně pro procedural outputs (známo, ne nutné fixovat).



---

## Aktualizace

| Datum | Session | Změna |
|---|---|---|
| 2026-05-22 | Phase H A+B v1.3.0 | Initial creation. Zachyceny TD-H1..H13, TD-G1..G5, TD-X1..X6, TD-S1..S6, TD-P1..P3. |
| 2026-05-22 | Phase H live test v1.3.2 | Surfaced TD-H14 (sprite alpha real-world fail), TD-X7 (UCAF Poller race), TD-H15 (procedural isReadable). TD-H12 RESOLVED (resolution semantic verified, false alarm). |
| 2026-05-23 | TD-H14 fix v1.3.3 | `alpha_method` dispatch + edge_color default + auto-retry + opt-in rembg subprocess. TD-H14 RESOLVED (live regression: 85.5 % vs 0 % alpha on same prompt). TD-X7 hit live during deploy — still open. |
| 2026-05-23 | TD-H14 rembg patch v1.3.4 | rembg invocation `python -m rembg` → `python -c "from rembg import remove; …"` (rembg 2.x missing `__main__`). Honza installed `rembg[cpu]` on Python 3.14.4 (onnxruntime 1.26 cp314 wheel). Live test: rembg = 99.6 % alpha, 62 s first run. All 3 backends verified. |
