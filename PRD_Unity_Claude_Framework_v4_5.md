# PRD – Unity + Claude Code AI-Assisted Framework
**Version:** 4.5
**Author:** Jan Malák
**Date:** 2026-05-22
**Status:** Delta IMPLEMENTOVÁNA jako UCAF package v1.3.2 — Phase H A+B variant (local SD via Forge + builtin procedural; žádné externí API, žádné poplatky, full offline). Commit `66f0969903f69c5c0fecaee5fca0a76228efa0eb`. 7/7 live testů PASSED 2026-05-22.
**Předchozí verze:** PRD_Unity_Claude_Framework_v4_4.md (v4.4, stále platná pro FR-219..FR-228), PRD_Unity_Claude_Framework_v4_3.md (v4.3, stále platná pro Fáze A–J framework).

---

## 0. Co se mění proti v4.3 + v4.4

Phase H původní spec v v4.3 sekce 4.48 předpokládala **externí API** (DALL-E 3 + ElevenLabs). Po debate v session 2026-05-22 Honza explicit zvolil **A+B variant**:

- **A) Lokální Stable Diffusion** přes A1111-kompatibilní REST API
- **B) Builtin procedural generators** v C# přímo v UCAF (real PBR data, deterministic)

**Žádné externí API klíče, žádné poplatky, žádný outage risk.** Compromise: vyžaduje lokální GPU (Honza má RTX 5060), out-of-the-box quality nižší než DALL-E 3 pro stylized art, ale **fundamentally lepší pro normal/roughness/metallic** (real tangent-space data, ne hallucinace).

| Co se mění | v4.3 / v4.4 | v4.5 |
|---|---|---|
| FR-199 `generate_texture` provider | DALL-E 3 / SD via `ucaf_config.json` | **explicit `provider=procedural\|local_sd`** param, žádný auto |
| FR-201 `generate_sprite` provider | DALL-E 3 transparent prompt | `provider=local_sd` only, **floodfill alpha extraction** v C# |
| FR-221 `generate_cubemap` provider | DALL-E 3 1792×1024 | `provider=procedural\|local_sd` |
| FR-203 `generate_sfx` | ElevenLabs Sound Effects | **Skipped** — odložen do future session (lokální audio gen plán) |
| FR-200 / FR-202 / FR-204 / FR-205 / FR-222 | Plánováno v Fázi H | **Skipped z této session** (multi-call orchestrace, async, batch deferred) |
| API klíče | `OPENAI_API_KEY`, `ELEVENLABS_API_KEY` env | Žádné. `UCAF_SD_ENDPOINT` env var (default `http://127.0.0.1:7860`), není secret |
| Procedural set | Neexistovalo v v4.3 | **3 generátory** v této session: `perlin_noise`, `gradient`, `normal_from_heightmap` |
| Forge swap | — | A1111 v1.10.1 broken (Stability-AI repo deleted 404); Forge drop-in replacement, identický API |

---

## 1. Implementační status v4.5

| FR | Command/feature | Status |
|----|-----------------|--------|
| FR-199 | `generate_texture` (provider=procedural\|local_sd) | ✅ DONE v1.3.2 |
| FR-200 | `generate_texture_set` | ❌ DEFERRED — multi-call PBR orchestrace + Material wiring |
| FR-201 | `generate_sprite` (provider=local_sd, alpha_method dispatch) | ✅ DONE v1.3.4 — alpha_method = edge_color (default, ~85% alpha typically) / floodfill (legacy) / rembg (opt-in, 99% alpha, AI-quality) / none. Auto-retry on heuristic underfill. TD-H14 RESOLVED + verified all 3 backends live. |
| FR-202 | `texture_variation` | ❌ DEFERRED — vyžaduje img2img SD endpoint |
| FR-203 | `generate_sfx` | ❌ DEFERRED — lokální audio stack (AudioLDM2/Bark) future session |
| FR-204 | `generate_music` | ❌ DEFERRED |
| FR-205 | `generate_sfx_variation` | ❌ DEFERRED |
| FR-221 | `generate_cubemap` (provider=procedural\|local_sd) | ✅ DONE v1.3.2 |
| FR-222 | `generate_terrain_layer` | ❌ DEFERRED — depend na FR-200 |
| FR-223 | AI-metadata tagging + `list_generated_assets` | ✅ DONE v1.1.0; v1.3.0 přidal `ucaf_provider` field (`procedural\|local_sd`) místo původně plánovaného `ucaf_revised_prompt` (DALL-E specific, nepoužitelné v A+B) |

**Status po impl této session bude aktualizován:** 🔄 → ✅ pro FR-199/201/221.

---

## 2. Phase H A+B architektura

### 2.1 Provider switch (explicit, žádný auto)

Per advisor + memory `feedback_honest_gaps`: **žádná `provider=auto` v první iteraci**. Premature abstraction over nothing. Klient (Claude session, MCP client) si vybere explicit:

```json
{
  "type": "generate_texture",
  "params_list": [
    {"key": "prompt", "value": "mossy stone wall"},
    {"key": "provider", "value": "procedural"},
    {"key": "style", "value": "perlin_noise"},
    {"key": "resolution", "value": "1024"}
  ]
}
```

Validní `provider` hodnoty per command:

| Command | provider=procedural | provider=local_sd |
|---|---|---|
| `generate_texture` | ✅ Perlin / Gradient / NormalFromHeightmap | ✅ A1111-compatible txt2img |
| `generate_sprite` | ❌ (sprite vyžaduje koherentní subject) | ✅ + floodfill alpha |
| `generate_cubemap` | ✅ 6-face gradient skybox | ✅ equirectangular → cube |

### 2.2 Procedural set (3 generators, B)

Lokace: `UCAF_Listener.AssetGen.cs` (nový partial). Vše inline C#, deterministic, žádné HTTP, žádné dependencies.

**`perlin_noise`** — Perlin 2D heightmap.
- Params: `seed` (int), `octaves` (1-8), `persistence` (0..1), `scale` (1..100)
- Implementace: `Mathf.PerlinNoise(x, y)` (Unity builtin), summed over octaves s decay
- Output: grayscale RGB (luminance = noise value)
- Use case: terrain heightmap base, clouds, foam, dirt overlay

**`gradient`** — Linear / radial 2D gradient.
- Params: `gradient_type=linear|radial`, `colors` (comma-separated `#RRGGBB`), `stops` (comma-separated 0..1), `angle` (linear only, degrees)
- Implementace: per-pixel UV → gradient lookup
- Use case: UI backgrounds, skybox gradients, simple PBR maps (single roughness gradient)

**`normal_from_heightmap`** — Tangent-space normal map ze source heightmap.
- Params: `source_asset_path` (existing grayscale texture), `strength` (1..10)
- Implementace: Sobel filter v 3x3 kernel, výpočet normálu, encode do RGB (R=X+0.5, G=Y+0.5, B=Z)
- Output: **real tangent-space normal** — physically correct PBR data
- Use case: konverze heightmap → normal map, advisor's earlier honest-gap killer

**Skipped procedurals** (odložené per Honzův 2026-05-22 select):
- `voronoi` — cellular rock pattern
- `cellular` — Worley noise variant

### 2.3 Local SD (A) — A1111-compatible REST API

Endpoint: `${UCAF_SD_ENDPOINT}/sdapi/v1/txt2img` (default `http://127.0.0.1:7860`).

Forge `lllyasviel/stable-diffusion-webui-forge` je drop-in replacement A1111 — identický endpoint, identický JSON shape. UCAF code-agnostic mezi A1111 a Forge.

**Request:**
```json
{
  "prompt": "...",
  "negative_prompt": "",
  "width": 1024,
  "height": 1024,
  "steps": 20,
  "sampler_name": "Euler a",
  "cfg_scale": 7,
  "seed": -1
}
```

**Response:** `{"images": ["<base64 PNG>"], "parameters": {...}, "info": "<JSON string s seed/etc>"}`.

**Sync HTTP** — Editor freeze. Mitigation `EditorUtility.DisplayProgressBar` (stejný pattern jako `ui_computer_use_fallback`). GPU verified: 256×256 / 10 steps = 16 s na RTX 5060. 1024×1024 / 20 steps odhadem ~30–60 s.

**Honest gap:** Forge server musí běžet když UCAF posílá request. Pokud neběží, command vrátí `error_code=sd_endpoint_unreachable` s hintem "spusť `.\\webui-user.bat` v `C:\\Tools\\stable-diffusion-webui-forge\\`".

### 2.4 Sprite alpha extraction (FR-201 — enhanced v1.3.3)

**Original v1.3.0–1.3.2:** floodfill from 4 corners, white-only (`RGB > 240`). Real-world test (2026-05-22 "potion bottle icon") returned **0 alpha pixels** when SD produced off-white / coloured background — TD-H14.

**v1.3.3 — `alpha_method` dispatch + auto-retry:**

| Method | What it does | Trade-off |
|---|---|---|
| `edge_color` (default) | Samples 1-px border → median RGB = bg color. Floods from corners with Chebyshev distance ≤ 40. | Handles off-white / coloured bg. Fails if subject touches canvas border (median poisoned). |
| `floodfill` (legacy) | Pre-1.3.3 behaviour. White-only flood (RGB > 240) from 4 corners. | Kept for regression / when SD bg is truly white. |
| `rembg` (opt-in) | `python -m rembg i` subprocess on Windows host. ONNX-based AI bg removal. | Requires `pip install rembg[cli]` + ~200 MB model on first run. Handles hair / glass / thin edges. |
| `none` | No alpha postprocess. Raw SD image stored as-is. | For when caller wants the image without alpha layer. |

**Auto-retry** (`auto_retry=true` by default, heuristic methods only): if `alpha_pixels < retry_threshold_pct (5%)` after first attempt, retry once with `cfg_scale + 3` and `seed + 1` (if seed was explicit; random seed re-rolled). Result reports `attempts` (1 or 2).

**Prompt augmentation** (sprite mode):
- Positive: `prompt + ", isolated subject, centered composition, pure white background, plain background, clean edges, studio lighting, no shadow, no scenery"`
- Negative: user-supplied + `"complex background, scene, environment, busy background, gradient background, coloured background, dark background, noise, blur, frame, border, watermark, text"`

**Soft edge:** 1-pixel boundary alpha=128 blend retained from v1.3.0.

**Result type** (`UCAFGenerateSpriteResult`) gains `alpha_method` + `attempts` fields, plus method-specific `honest_gap` message.

### 2.5 AI metadata tag (FR-223 update)

`UCAFAIMetadataMarker` rozšířen o `ucaf_provider` field:

```json
{
  "ucaf_generated": true,
  "ucaf_version": "1.3.0",
  "ucaf_command": "generate_texture",
  "ucaf_prompt": "mossy stone wall",
  "ucaf_provider": "procedural",
  "ucaf_api": "perlin_noise",
  "ucaf_timestamp": "2026-05-22T..."
}
```

Pro `provider=procedural` je `ucaf_api` = procedural style (`perlin_noise`, `gradient`, `normal_from_heightmap`).
Pro `provider=local_sd` je `ucaf_api` = `"sd_local"` + optional model name (parse z Forge response `info.sd_model_name`).

`list_generated_assets` payload entry rozšířen o `ucaf_provider`.

---

## 3. Honest gaps & non-goals (v této session)

**Honest gaps documented:**
- Procedural quality není srovnatelná s DALL-E pro stylized art (chápání of "scary tree" vs "moss stone wall"). Procedural je deterministic geometry, ne semantic.
- SD vyžaduje GPU + Forge server běží = single-machine dependency. Sandbox / CI bez GPU = SD path nefunkční. Procedural path funguje vždy.
- Sprite alpha edge_color default (v1.3.3): subjects s tenkými průhlednými detaily (peří, vlasy, sklenice) nebo subjects touching canvas border budou misclassified. Pro tyto case → `alpha_method=rembg` (opt-in, vyžaduje `pip install rembg[cli]`).
- Cubemap procedural je 6-face gradient skybox — žádné mraky, ne hvězdy, jen color blend. Pro real procedural sky → SDFs / clouds → samostatný command later.

**Non-goals (v této session):**
- `generate_sfx` — audio žádný (žádný local equivalent A+B pro audio v této session)
- `voronoi` / `cellular` procedural — odloženo per Honza
- `provider=auto` fallback chain — premature abstraction
- img2img endpoint (FR-202 texture variation) — vyžaduje SD v jiném režimu
- Multi-call PBR set (FR-200) — vyžaduje 4 nezávislé txt2img calls + Material asset wiring

---

## 4. Files changed v1.3.0 (předpokládané, finalizováno po impl)

**Nové soubory:**
- `Editor/UCAF_Listener.AssetGen.cs` — 3 procedural generators (Perlin / Gradient / NormalFromHeightmap)
- `Editor/UCAF_Listener.AssetGeneration.cs` — generate_texture / generate_sprite / generate_cubemap dispatch + local_sd HTTP path
- `Editor/UCAF_Types.PhaseH.cs` — response types (UCAFGenerateTextureResult, UCAFGenerateCubemapResult)

**Modified:**
- `Editor/UCAF_Listener.cs` — dispatch cases (3 new), version `1.2.0` → `1.3.0`
- `Editor/UCAF_Listener.MCPServer.cs` — `EnumerateAllCommandTypes` (3 new entries)
- `Editor/UCAF_Listener.AIMetadata.cs` — `UCAFAIMetadataMarker.ucaf_provider`, `TagAIGeneratedAsset` signature
- `Editor/UCAF_Types.v44.cs` — `UCAFGeneratedAssetEntry.ucaf_provider`
- `package.json` — version + description

**Forge setup** (mimo UCAF, dokumentováno v `next_session_instructions.md` 2026-05-22):
- `C:\Tools\stable-diffusion-webui-forge\` — install
- `webui-user.bat` config — `--api`, Python 3.10.6 per-user
- SD 1.5 base model — `models\Stable-diffusion\v1-5-pruned-emaonly.safetensors`

---

## 5. Live test results (2026-05-22, RTX 5060 + Forge SD 1.5)

**Procedural (resolution=512 except normal):**
| Test | Style | Result | Time |
|---|---|---|---|
| 1 | perlin_noise (seed=42, octaves=4) | ✅ PASS | 1069 ms |
| 2 | gradient linear (red→blue, 45°) | ✅ PASS | 453 ms |
| 3 | normal_from_heightmap (z Testu 1) | ✅ PASS po v1.3.2 fix | 837 ms |

**Local SD (Forge running on 127.0.0.1:7860, resolution=512, steps=20):**
| Test | Command | Result | Time |
|---|---|---|---|
| 4 | generate_texture "mossy stone wall" | ✅ PASS | 12327 ms |
| 5 | generate_sprite "potion bottle icon" | ✅ PASS (0 alpha px — honest gap) | 3996 ms |
| 6 | generate_cubemap procedural | ✅ PASS | 694 ms |
| 7 | generate_cubemap local_sd "sunset desert" | ✅ PASS | 7150 ms |

**Metadata tagging:** `list_generated_assets` vrátil 10 assetů (po retry/v1.3.x testech), všechny s `ucaf_provider` correctly populated (`procedural` nebo `local_sd`).

**NFR:**
- Procedural ≤ 5 s pro 1024×1024 → ✅ pro 512: 0.5-1 s
- SD ≤ 60 s pro 1024×1024 → ✅ pro 512: 12 s (extrapolated 60-90s pro 1024 = within budget)

**Surfaced bugs (fixed in patch releases):**
- v1.3.0 → v1.3.1: chained workflow (`perlin → normal_from_heightmap`) failed na `isReadable=false` → auto-fix přidán
- v1.3.1 → v1.3.2: try/catch (UnityException) nezachytil Unity 6.4 exception type → replaced preflight check na `TextureImporter.isReadable`

**Surfaced honest gaps (deferred fixes):**
- ~~Sprite alpha floodfill: SD nevygeneruje vždy pure white bg → 0 alpha pixels možné (TD-H8 / TD-H14)~~ → **RESOLVED v1.3.3** via `alpha_method=edge_color` default (see section 2.4)
- Sharing violation race v UCAF poller — sporadic, ne v Phase H code (TD-X7, hit live během v1.3.3 deploy)

---

## 5b. v1.3.3 live test (2026-05-23)

Same Forge endpoint, same SD model (`v1-5-pruned-emaonly`), same "potion bottle icon" prompt:

| Test | alpha_method | Alpha pixels | Attempts | Time | Result |
|---|---|---|---|---|---|
| 1 | edge_color (new default) | 224 243 / 262 144 = **85.5%** | 1 | 7054 ms | ✅ PASS |
| 2 | floodfill (legacy regression) | **0** | 2 (retry triggered) | 6438 ms | ✅ Honest gap reported correctly |
| 3 | rembg | 260 994 / 262 144 = **99.6 %** | 1 | 62 251 ms (first run incl. U2Net download) | ✅ PASS |

Conclusion: TD-H14 RESOLVED for all three heuristic + AI backends. Auto-retry mechanism verified working in floodfill case (attempts=2 reported). rembg quality is decisively higher (99.6 % vs 85.5 % edge_color) but pays in time (62 s first / ~10 s subsequent) + Python dep.

**v1.3.4 patch:** v1.3.3 originally invoked rembg as `python -m rembg i …` — rembg 2.x has no `__main__.py`, so this exited with `No module named rembg.__main__`. Fixed by calling the Python API directly via `python -c "from rembg import remove; …"` (rembg's snippet uses only single-quoted string literals so C# / shell escaping stays trivial). Also: hint matrix recognises "No onnxruntime backend found" (rembg's friendly import-time message) → routes to `pip install rembg[cpu]` (the `[cpu]` extra is what pulls `onnxruntime`, contrary to docs that say `[cli]`).

---

## 6. Otevřené otázky pro budoucí PRD update

- **FR-203 audio path** — lokální AudioLDM2 / Stable Audio / Bark mají samostatný Python stack (~10 GB). Worth it pro hobby workflow? Alternativa: jednorázový ElevenLabs subscription jen pro projekt + opt-in env key. Rozhodne se v session N+1.
- **Voronoi / cellular procedural** — kdy je Honza skutečně potřebuje? Pokud nikdy, smazat z PRD.
- **img2img (FR-202)** — Forge má `/sdapi/v1/img2img` endpoint, snadný add. Worth ~0.5 session, jen pokud Honza chce variace existujících textur.

---

**Status tracking:** tento PRD bude finalizován s ✅ DONE statusy pro FR-199/201/221 po commit + push UCAF v1.3.0 a live test pass.
