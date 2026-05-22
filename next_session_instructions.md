# Instrukce pro příští session

> Poslední aktualizace: 2026-05-22 (Phase H A+B v1.3.2 live, 7/7 testů pass)

## Kde jsme skončili

- **Phase H A+B implementována a end-to-end ověřena.** UCAF v1.3.2 commitnut + pushnut na `honzamalak-afk/ucaf` (HEAD = `66f0969`). Manifest + lock v projektu pinnut na full hash `66f0969903f69c5c0fecaee5fca0a76228efa0eb`.
- **3 nové commandy registrované (FR-199, FR-201, FR-221):**
  - `generate_texture` (provider=procedural|local_sd) — ověřeno: 1069ms procedural perlin, 12327ms local_sd
  - `generate_sprite` (provider=local_sd) — ověřeno: floodfill alpha, honest gap "0 alpha pixels" pokud SD nevyrobí white bg
  - `generate_cubemap` (provider=procedural|local_sd) — ověřeno: gradient skybox (694ms) + SD panorama (7150ms)
- **3 procedural generators** v `UCAF_Listener.AssetGen.cs`: perlin_noise, gradient, normal_from_heightmap (Sobel). Voronoi + Cellular skipped per Honza select.
- **PRD v4.5** napsáno (`PRD_Unity_Claude_Framework_v4_5.md`) jako delta nad v4.4 — A+B variant, Forge swap zachycen, FR-199/201/221 ✅ DONE, FR-200/202/203/204/205/222 ❌ DEFERRED.
- **`ucaf_technical_debts.md`** vytvořen (root projektu) jako living log otevřených tech debtů. CLAUDE.md má odkaz. 30+ debtů z 5 sekcí (Phase H, Phase G, cross-cutting, setup, process).
- **Forge setup** beze změny: `C:\Tools\stable-diffusion-webui-forge\`, Honza spouští ručně přes `.\webui-user.bat`, endpoint `http://127.0.0.1:7860`.

## Co je rozpracované / nedokončené

- **Sprite alpha kvalita** — TD-H14. Live test "potion bottle icon" → 0 alpha pixels (SD nevygeneroval white bg). Honest gap dokumentován. Production-quality sprites budou potřebovat post-process (rembg / SAM) nebo agresivnější prompt engineering / Img2Img mask flow.
- **UCAF Poller sharing violation** — TD-X7. Sporadická race condition (~1/10 commandů), retry-on-violation jednoduchý fix (~10 lines).
- **`generate_sfx` (FR-203) a celá audio větev** — žádné A+B equivalent pro audio v této session. Future trigger: rozhodnutí mezi lokálním AudioLDM2/Bark stack (~2-3 sessions) nebo opt-in ElevenLabs.
- **Phase H deferred features** (TD-H1..H6): `generate_texture_set` PBR multi-call, `texture_variation` (img2img), `generate_music`, `generate_sfx_variation`, `generate_terrain_layer`, `provider=auto` fallback chain.
- **`A1111_SETUP.md` v ucaf_workspace/setup je DEPRECATED** (TD-S2) — nebyl smazán, Forge dropuje A1111. Honza může smazat / nahradit `FORGE_SETUP.md` v příští session.

## Co dělat příště

**Priorita 1 — strategická volba další fáze** (Honza). Otevřené:

1. **Fáze I FR-206** — `plan_task` (Markdown plán + state machine). ~1 session, nízká hodnota dokud Claude Code už dělá plánování interně.
2. **Fáze J** — skills architektura, nejlepší jako poslední (refactor nad stabilním kódem). ~2 sessions.
3. **TD-X1 `EnumerateAllCommandTypes` source-generator** — manuální mirror dispatch switch má drift risk. ~0.5 session.
4. **Sprite alpha fix** (TD-H14) — implementovat Img2Img mask flow nebo rembg integraci. ~1 session.
5. **`generate_sfx`** (audio) — pokud Honza chce, decision na lokální vs opt-in API.
6. **TD-S2 FORGE_SETUP.md** — napsat aktuální Forge setup guide nahradit deprecated A1111_SETUP.md. ~0.25 session.

**Priorita 2 — průběžně:**
- **PAT** — Honzův fine-grained token použit 3× v této session (3.5× v minulých, 5× v ještě dřívější). PŘI této session leak v plain chat → **token MUSÍ být revoked** v GitHub Settings. Generovat nový pro další session.
- **Stale PackageCache** — když Unity nereaguje na nový hash, smaž `Library/PackageCache/com.ucaf@<old_hash>/` ručně. V této session 3× nový hash, Unity to zvládl bez manuálního zásahu.

## Blokátory / otevřené otázky

- **Volba další fáze** — Honza musí říct (I / J / TD priority / SFX path).
- **PAT revoke status** — Honza by měl ověřit, že použitý token je revoked.

## Důležitý kontext / pozor na

- **UCAF v1.3.2 HEAD** `66f0969903f69c5c0fecaee5fca0a76228efa0eb`. Pro další update commit, push, manifest pin v `Packages/manifest.json` + `packages-lock.json`.
- **Forge musí běžet** pro live test SD path (Honza: `cd C:\Tools\stable-diffusion-webui-forge && .\webui-user.bat`).
- **Procedural path = zero dep** — testovatelný bez Forge. Diagnostic ordering: procedural first, SD second.
- **Domain reload pattern** stále platí: `clear_console` → `create_script _UCAFRecompileTrigger.cs` → `compile_and_wait` (15+s = recompile). V této session použito 3× (1.3.0 → 1.3.1 → 1.3.2).
- **`.meta` files pro nové C#** v packagi povinné. Pattern: `printf 'fileFormatVersion: 2\nguid: <32hex>\n' > X.cs.meta`.
- **Auto-fix isReadable** v `AssetGenNormalFromHeightmap` (v1.3.2) → source texture get `isReadable=true` set persistently. Memory side effect.
- **Texture2D `linear=false`** v encode helpers (per advisor). `linear=true` rozbije gradient sRGB encoding přes EncodeToPNG.
- **Inline `VAR=x command` NEFUNGUJE pro `${VAR}` v args** — shell expanduje před assignment. Pattern: `export VAR='...'; cmd ${VAR}; unset VAR`.
- **Memory `feedback_secret_handling`** — PAT nikdy neecho v shell output, nikdy persist do file/memory/env. Per-session jen.
- **Project path** — `/workspace/` = `C:\Projects\Hry\Siege of the Blue World\` na Windows.

## Files touched v této session (2026-05-22)

UCAF dev repo (`/workspace/_ucaf_dev/`, pushed):
- `Editor/UCAF_Listener.AssetGen.cs` — NEW (3 procedural generators)
- `Editor/UCAF_Listener.AssetGeneration.cs` — NEW (3 commandy + provider switch + floodfill alpha + SD HTTP)
- `Editor/UCAF_Types.PhaseH.cs` — NEW (response types)
- `Editor/UCAF_Listener.AIMetadata.cs` — modified (ucaf_provider field)
- `Editor/UCAF_Types.v44.cs` — modified (ucaf_provider in entry)
- `Editor/UCAF_Listener.cs` — modified (3 dispatch cases, version 1.2.0 → 1.3.2)
- `Editor/UCAF_Listener.MCPServer.cs` — modified (3 enumeration entries)
- `package.json` — 1.2.0 → 1.3.2

Project root (`/workspace/`):
- `Packages/manifest.json` — pinned to `66f0969…` full hash
- `Packages/packages-lock.json` — same
- `PRD_Unity_Claude_Framework_v4_5.md` — NEW (Phase H A+B delta)
- `PRD_Unity_Claude_Framework_v4_3.md` — reverted (DALL-E status edity smazány)
- `PRD_Unity_Claude_Framework_v4_4.md` — reverted (DALL-E status edity smazány)
- `ucaf_technical_debts.md` — NEW (30+ debtů, living log)
- `CLAUDE.md` — added pointer to ucaf_technical_debts.md
- `next_session_instructions.md` — UPDATED (this file)

Unity project (`Assets/Generated/`):
- 10 test-generated assetů (textures, sprites, cubemaps, normal maps). Honza může smazat / použít pro inspirate.
