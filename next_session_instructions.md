# Instrukce pro příští session

> Poslední aktualizace: 2026-05-23 (Phase H A+B v1.3.2 live + docs commitnuty na origin/main + gh CLI persistent + .gitattributes added)

## Kde jsme skončili

- **Phase H A+B implementována a end-to-end ověřena.** UCAF v1.3.2 pushed na `honzamalak-afk/ucaf` (HEAD `66f0969`). Unity project repo `honzamalak-afk/siege-of-the-blue-world` HEAD `46ee826` (4 commits dnes).
- **3 nové commandy registrované (FR-199, FR-201, FR-221):**
  - `generate_texture` (provider=procedural|local_sd) — ověřeno: 1069ms procedural perlin, 12327ms local_sd
  - `generate_sprite` (provider=local_sd, floodfill alpha) — honest gap "0 alpha pixels" pokud SD nevyrobí white bg
  - `generate_cubemap` (provider=procedural|local_sd) — gradient skybox (694ms) + SD panorama (7150ms)
- **3 procedural generators** v `UCAF_Listener.AssetGen.cs`: perlin_noise, gradient, normal_from_heightmap (Sobel). Voronoi + Cellular skipped per Honza select.
- **PRD v4.5** napsáno + finalizováno s ✅ DONE statusy pro FR-199/201/221. Live test results section + surfaced bugs.
- **`ucaf_technical_debts.md`** vytvořen s 33+ debty (5 sekcí). CLAUDE.md má odkaz. **Pushed.**
- **`gh` CLI 2.92.0** nainstalován v `~/.local/bin/gh`, auth uložen v `~/.config/gh/hosts.yml` (token `gho_…`, scopes `repo, read:org, gist`). PATH přidán do `~/.bashrc` — **persistuje napříč sessions** per memory `reference_environment`. Žádný PAT v env tabu už nepotřeba.
- **`.gitattributes`** přidán (commit `46ee826`) — WSL2 CRLF prevention: LF default + Unity types/binary excludes + `.bat`/`.cmd` CRLF pro cmd.exe.
- **Forge setup** beze změny: `C:\Tools\stable-diffusion-webui-forge\`, Honza spouští ručně přes `.\webui-user.bat`, endpoint `http://127.0.0.1:7860`.

## Co je rozpracované / nedokončené

- **PAT cleanup** — leaked `github_pat_11CCVZX…` token Honza POSLAL v plain textu v této session. **MUSÍ být revoked** v GitHub Settings → Developer settings → Fine-grained tokens. gh CLI ho už nahradil, ale leaked stále funguje až do revoke.
- **Sprite alpha kvalita** — TD-H14. Live test "potion bottle icon" → 0 alpha pixels (SD nevygeneroval white bg). Production-quality sprites budou potřebovat post-process (rembg / SAM) nebo agresivnější prompt engineering / Img2Img mask flow.
- **UCAF Poller sharing violation** — TD-X7. Sporadická race condition (~1/10 commandů), retry-on-violation jednoduchý fix (~10 lines).
- **`generate_sfx` (FR-203) a celá audio větev** — žádné A+B equivalent pro audio v této session. Future trigger: rozhodnutí mezi lokálním AudioLDM2/Bark stack (~2-3 sessions) nebo opt-in ElevenLabs.
- **Phase H deferred features** (TD-H1..H6): `generate_texture_set` PBR multi-call, `texture_variation` (img2img), `generate_music`, `generate_sfx_variation`, `generate_terrain_layer`, `provider=auto` fallback chain.
- **`A1111_SETUP.md` v ucaf_workspace/setup je DEPRECATED** (TD-S2) — nebyl smazán, Forge dropuje A1111. Honza může smazat / nahradit `FORGE_SETUP.md`.
- **Existing files s CRLF** (pre-`.gitattributes`) — repo má některé soubory s mixed line endings (CLAUDE.md byl fixnut, ale jiné mohou mít CRLF). `git add --renormalize .` by udělalo plošný cleanup, ale to je velký diff. Postupný cleanup při normálních editech stačí.

## Co dělat příště

**Priorita 1 — strategická volba další fáze** (Honza musí říct). Otevřené:

1. **Fáze I FR-206** — `plan_task` (Markdown plán + state machine). ~1 session, nízká hodnota dokud Claude Code už dělá plánování interně.
2. **Fáze J** — skills architektura, nejlepší jako poslední (refactor nad stabilním kódem). ~2 sessions.
3. **TD-X1 `EnumerateAllCommandTypes` source-generator** — manuální mirror dispatch switch má drift risk. ~0.5 session.
4. **Sprite alpha fix** (TD-H14) — implementovat Img2Img mask flow nebo rembg integraci. ~1 session.
5. **`generate_sfx`** (audio) — pokud Honza chce, decision na lokální vs opt-in API.
6. **TD-S2 FORGE_SETUP.md** — napsat aktuální Forge setup guide nahradit deprecated A1111_SETUP.md. ~0.25 session.
7. **TD-X7 UCAF Poller retry** — sharing violation race fix. ~0.25 session.

**Priorita 2 — Honza akce:**
- **Revoke leaked PAT** v GitHub Settings (jednorázová akce, ~1 min).
- **Stale PackageCache** — když Unity nereaguje na nový hash, smaž `Library/PackageCache/com.ucaf@<old_hash>/` ručně. V dnešní session 3× nový hash, Unity to zvládl bez manuálního zásahu.

## Blokátory / otevřené otázky

- **Volba další fáze** — Honza musí říct (I / J / TD priority / SFX path / sprite alpha).
- **PAT revoke status** — Honza by měl ověřit / udělat.

## Důležitý kontext / pozor na

- **UCAF v1.3.2 HEAD** `66f0969903f69c5c0fecaee5fca0a76228efa0eb`. Pro další update: commit, push (přes `gh` CLI nebo `git push origin main` — již nakonfigurováno), manifest pin v `Packages/manifest.json` + `packages-lock.json`.
- **`gh` CLI je persistent** — `gh auth status` ověří. Push bez PAT v env. Memory `feedback_secret_handling` stále platí pro budoucí secrets.
- **Forge musí běžet** pro live test SD path (Honza: `cd C:\Tools\stable-diffusion-webui-forge && .\webui-user.bat`).
- **Procedural path = zero dep** — testovatelný bez Forge. Diagnostic ordering: procedural first, SD second.
- **Domain reload pattern** stále platí: `clear_console` → `create_script _UCAFRecompileTrigger.cs` → `compile_and_wait` (15+s = recompile). V dnešní session použito 3× (1.3.0 → 1.3.1 → 1.3.2).
- **`.meta` files pro nové C#** v UCAF packagi povinné. Pattern: `printf 'fileFormatVersion: 2\nguid: <32hex>\n' > X.cs.meta`.
- **Auto-fix isReadable** v `AssetGenNormalFromHeightmap` (v1.3.2) → source texture získá `isReadable=true` persistently. Memory side effect.
- **Texture2D `linear=false`** v encode helpers (per advisor). `linear=true` rozbije gradient sRGB encoding přes EncodeToPNG.
- **Inline `VAR=x command` NEFUNGUJE pro `${VAR}` v args** — shell expanduje před assignment. Pattern: `export VAR='...'; cmd ${VAR}; unset VAR`.
- **`.gitattributes` přidán** — Edit z Linux sandboxu už nemá CRLF problem (LF default). `.bat`/`.cmd` zůstanou CRLF.
- **WSL2 mount inheritance** — soubory psané ze sandbox do `/workspace/` mohou mít CRLF, pokud `.gitattributes` ne-překrýt — teď OK, dokud někdo nezruší attribute.
- **Workspace má 2 remotes**: `origin` (siege-of-the-blue-world) hlavní, `greybox` (sieg-of-the-blue-world-gray-box-yaml-1.5) — používat origin.
- **Project path** — `/workspace/` = `C:\Projects\Hry\Siege of the Blue World\` na Windows.

## Files touched v dnešní session (2026-05-22 + 2026-05-23)

UCAF dev repo (`/workspace/_ucaf_dev/`, pushed do `honzamalak-afk/ucaf`):
- `Editor/UCAF_Listener.AssetGen.cs` — NEW (3 procedural generators: perlin / gradient / normal_from_heightmap)
- `Editor/UCAF_Listener.AssetGeneration.cs` — NEW (3 commandy + provider switch + floodfill alpha + SD HTTP)
- `Editor/UCAF_Types.PhaseH.cs` — NEW (response types)
- `Editor/UCAF_Listener.AIMetadata.cs` — modified (ucaf_provider field)
- `Editor/UCAF_Types.v44.cs` — modified (ucaf_provider in entry)
- `Editor/UCAF_Listener.cs` — modified (3 dispatch cases, version 1.2.0 → 1.3.2)
- `Editor/UCAF_Listener.MCPServer.cs` — modified (3 enumeration entries)
- `package.json` — 1.2.0 → 1.3.2
- Commits: `c5a5a64` (DALL-E variant, reverted lokálně, nikdy pushnut) → `f00fed2` (v1.3.0) → `192fd14` (v1.3.1) → `66f0969` (v1.3.2 final)

Unity project repo (`/workspace/`, pushed do `honzamalak-afk/siege-of-the-blue-world`):
- `PRD_Unity_Claude_Framework_v4_5.md` — NEW (Phase H A+B delta)
- `PRD_Unity_Claude_Framework_v4_3.md` — committed první verze (předtím untracked)
- `PRD_Unity_Claude_Framework_v4_4.md` — committed první verze (předtím untracked)
- `ucaf_technical_debts.md` — NEW (33+ debtů, living log)
- `next_session_instructions.md` — UPDATED (this file)
- `CLAUDE.md` — added pointer to `ucaf_technical_debts.md`
- `Packages/manifest.json` — pinned to `66f0969…` full hash
- `Packages/packages-lock.json` — same
- `.gitattributes` — NEW (WSL2 CRLF prevention)
- Commits: `2491ec9` (Phase H docs — PRD + tech debts + instructions) → `3623c42` (pin v1.3.2 + tech debt link) → `129c5b6` (LF fix CLAUDE.md) → `46ee826` (.gitattributes)

Sandbox environment:
- `~/.local/bin/gh` — NEW (gh CLI 2.92.0 precompiled, no sudo)
- `~/.config/gh/hosts.yml` — NEW (auth token, scopes repo + read:org + gist)
- `~/.bashrc` — appended `$HOME/.local/bin` PATH guard

Unity project (`Assets/Generated/`):
- 10 test-generated assetů (textures, sprites, cubemaps, normal maps). Honza může smazat / použít pro inspiraci.
