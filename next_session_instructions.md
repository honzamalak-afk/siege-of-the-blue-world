# Instrukce pro příští session

> Poslední aktualizace: 2026-05-23 (UCAF v1.3.4 live + TD-H14 RESOLVED, all 3 backends verified)

## Kde jsme skončili

- **UCAF v1.3.4 pushed** na `honzamalak-afk/ucaf` (HEAD `ef544f6`). Unity repo manifest + lock repinned, package reload + recompile verified, listener back online v 1.3.4 protocol_capabilities.
- **TD-H14 RESOLVED, all 3 backends verified live** (same SD prompt "potion bottle icon"):
  - `edge_color` (default) — sample 1px border → median bg color → flood s Chebyshev tolerance. **85.5 % alpha**, 1 attempt, 7 s.
  - `rembg` (opt-in) — `python -c "from rembg import remove; …"` subprocess (v1.3.4 fix: rembg 2.x nemá `__main__`). **99.6 % alpha**, 1 attempt, 62 s first run (U2Net ~200 MB download), <10 s subsequent. Honza nainstaloval `rembg[cpu]` na Python 3.14.4 (`onnxruntime-1.26.0 cp314 wheel`).
  - `floodfill` (legacy) — white-only z corners. **0 % alpha, 2 attempts** (auto-retry kicked in correctly + honest gap accurate).
  - `none` — raw image, no alpha postprocess.
- **Auto-retry** (`auto_retry=true`, heuristic only): bump cfg+3, seed+1, if alpha < 5 %. `attempts` field v response.
- **Stronger sprite-mode prompt augmentation:** positive ("isolated subject, centered, pure white background, plain background, clean edges, studio lighting, no shadow, no scenery") + negative ("complex background, scene, environment, busy background, gradient/coloured/dark background, noise, blur, frame, border, watermark, text").
- **Surfaced bug:** TD-X7 (UCAF Poller sharing violation race) **hit live** během 1.3.3 deploy — confirmed real, not theoretical.
- **Docs synced:** PRD v4.5 sections 2.4 + 3 + 5b updated. `ucaf_technical_debts.md` TD-H14 marked RESOLVED + activity log entry. PackageCache `com.ucaf@66f0969903f6` smazána (per "stale PackageCache" pattern).

## Co je rozpracované / nedokončené

- **TD-X7 UCAF Poller sharing violation** — confirmed live, fix ~10 řádků retry-on-violation loop v `UCAF_Listener.cs:162`. Quick win.
- **PAT cleanup** — leaked `github_pat_11CCVZX…` token z předchozí session musí být **revoked** v GitHub Settings → Developer settings → Fine-grained tokens. `gh` CLI ho už nahradil; leaked token funguje až do revoke.
- **Edge-color median contamination** — full-frame subject touching canvas border poisons median bg color. Documented v `honest_gap` + PRD 2.4. Workaround = `alpha_method=rembg`.
- **rembg subsequent-call benchmark** — first call 62 s (download + first inference), subsequent calls untested. Memory-side claim "~10 s subsequent" je inference-only odhad, ne measured.

## Co dělat příště

**Priorita 1 — Honza strategická volba:**

1. **Phase I FR-206** — `plan_task` (~1 session, nízká hodnota, Claude Code už plánuje interně)
2. **Phase J** — skills architektura (~2 sessions, nejlepší jako poslední refactor)
3. **TD-X7 UCAF Poller retry** — sharing violation fix (~0.25 session, quick win, confirmed real bug)
4. **TD-S2 FORGE_SETUP.md** — napsat aktuální Forge setup, smazat deprecated A1111_SETUP.md (~0.25 session)
5. **TD-X1 source-generator** — manuální mirror dispatch switch drift risk (~0.5 session)
6. **`generate_sfx` audio** — pokud Honza chce, decision na lokální vs ElevenLabs opt-in
7. **rembg path live test** — pokud Honza nainstaluje rembg, ověřit subprocess flow na hair/glass case

**Priorita 2 — Honza akce:**
- Revoke leaked PAT (~1 min)

## Důležitý kontext / pozor na

- **UCAF v1.3.4 HEAD** `ef544f6f4a0bf015877f31cd6a0014acc705f80b`. Listener answering, 160 commands enumerated, package_version reported correctly.
- **rembg invocation:** musí být `python -c "from rembg import remove; …"`, NIKDY `python -m rembg` (rembg 2.x nemá `__main__.py`). Honza má `rembg[cpu] 2.0.75` + `onnxruntime 1.26` na Python 3.14.4.
- **Stale PackageCache pattern works:** delete `Library/PackageCache/com.ucaf@<old_hash>/` → asset_refresh → Unity re-clones nový hash → long domain reload (cca 120-180 s pro fresh git pull + compile). Pak listener obživne.
- **Sharing violation race** — `UCAF_Listener.cs:162` `File.ReadAllText` v Poll(). Real, hit during this session. Retry-on-IOException w/ 50ms backoff fix je trivial.
- **Process subprocess (rembg)**: async event-based readers (`OutputDataReceived`/`ErrorDataReceived` + `BeginOutputReadLine`/`BeginErrorReadLine`) — NIKDY sync `ReadToEnd()` na obou streamech sequentially (pipe-fill deadlock při tqdm/onnxruntime chatter).
- **Sandbox = Docker container**, ne WSL2 — Windows localhost dostupný přes `host.docker.internal:7860`, ne `127.0.0.1`. `cmd.exe` interop NEdostupný — Python check na Windows musí ručně.
- **`gh` CLI persistent** v `~/.local/bin/gh`, auth v `~/.config/gh/hosts.yml`. Push bez PAT v env.
- **Forge musí běžet** pro live SD test: `cd C:\Tools\stable-diffusion-webui-forge && .\webui-user.bat`.
- **Domain reload pattern** stále platí pro běžné code edits: `clear_console` → `create_script _UCAFRecompileTrigger.cs` → `compile_and_wait`.
- **`.meta` files** pro nové C# v UCAF packagi povinné. V této session žádné nové soubory přidány — jen editace existujících, takže meta files unchanged.
- **Workspace 2 remotes:** `origin` (siege-of-the-blue-world), `greybox` (sieg-of-the-blue-world-gray-box). Pro běžnou práci `origin`.
- **Project path:** `/workspace/` = `C:\Projects\Hry\Siege of the Blue World\` na Windows.

## Files touched v této session (2026-05-23 TD-H14 fix v1.3.3 + v1.3.4)

UCAF dev repo (`/workspace/_ucaf_dev/`, pushed jako `d37cbd1` then `ef544f6`):
- `Editor/UCAF_Listener.AssetGeneration.cs` — major refactor (`CmdGenerateSprite` + new helpers: `FloodfillAlphaEdgeColor`, `ChebyshevColorDist`, `MedianByte`, `DecodePng`, `ApplyRembgAlpha`, local `ParseBool`)
- `Editor/UCAF_Listener.cs` — `package_version` bump na "1.3.3"
- `Editor/UCAF_Types.PhaseH.cs` — `UCAFGenerateSpriteResult` + `alpha_method`, `attempts`
- `package.json` — version + description bump na 1.3.3

Unity project repo (`/workspace/`, pending commit):
- `Packages/manifest.json` — pin na `d37cbd1b2f8ccba11bbc1c9a9795f66dffc7e077`
- `Packages/packages-lock.json` — same hash
- `PRD_Unity_Claude_Framework_v4_5.md` — section 2.4 rewrite, status row update, honest gap update, new section 5b live test
- `ucaf_technical_debts.md` — TD-H14 marked RESOLVED + activity log entry
- `next_session_instructions.md` — UPDATED (this file)
- `Assets/Generated/Sprites/test_td_h14_edge_color_potion.png` — NEW (live test artifact)
- `Assets/Generated/Sprites/test_td_h14_floodfill_potion.png` — NEW (regression artifact)
- `Library/PackageCache/com.ucaf@66f0969903f6/` — DELETED (stale PackageCache pattern)
