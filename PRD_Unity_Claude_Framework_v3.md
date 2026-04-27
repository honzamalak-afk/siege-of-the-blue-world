# PRD – Unity + Claude Code AI-Assisted Development Framework
**Version:** 3.0
**Author:** Jan Malák
**Date:** 2026-04-25
**Status:** OBSOLETE — nahrazeno PRD_Unity_Claude_Framework_v4.md (v4.0)
**Předchozí verze:** PRD_Unity_Claude_Framework.md (v2.0, OBSOLETE)

---

## 1. Overview

### 1.1 Project Name
**UCAF** – Unity Claude Assisted Framework

### 1.2 Purpose
UCAF spojuje Claude Code s Unity Editorem na Windows 11 tak, aby se Claude choval jako **autonomní junior Unity developer**. Dostane zadání, udělá ho, vidí výsledky vlastní práce a iteruje — bez nutnosti manuálního zásahu Jana v Editoru při běžných implementačních úkolech.

V3.0 rozšiřuje v2.0 o klíčové schopnosti: Claude nově **vidí scénu** (screenshot na příkaz), **hledá objekty** (query místo průchodu celé hierarchie), **čte existující kód** (nejen píše nový), a provádí sekvence operací jako atomické celky (batch). Tím se feedback loop uzavírá na straně Clauda, ne Jana.

### 1.3 Role split (nezměněno)
- **Claude = junior developer**: implementuje podle zadání, verifikuje, reportuje, ptá se
- **Jan = senior + art director + game designer**: zadává, rozhoduje o designu, drží vizuální nástroje

### 1.4 Vision Statement
*"Řekni juniorovi co má udělat. Udělá to, podívá se na výsledek, opraví co nefunguje, a teprve když narazí na strop, přijde za tebou."*

### 1.5 Co UCAF není
- Není autopilot ani game designer
- Nenahrazuje Shader Graph, VFX Graph, Timeline, Animator, Cinemachine — vizuální nástroje zůstávají u Jana
- Negaruje game feel ani vizuální polish

---

## 2. Goals and Non-Goals

### 2.1 Goals v3.0
- **Vizuální feedback smyčka:** Claude pořídí screenshot na příkaz a vidí výsledek své práce
- **Autonomní orientace ve scéně:** query objektů podle komponent/tagů/layerů bez stahování celé hierarchie
- **Práce s existujícím kódem:** číst, opravovat a mazat soubory v Assets, nejen vytvářet nové
- **Batch operace:** sekvence commandů jako jeden krok — rychlejší, atomičtější, méně čekání
- **Scene View kontrola:** Claude vybere a zaměří objekt před screenshotem — screenshot ukazuje to, co Claude chce vidět
- **Generalizované asset discovery:** hledat prefaby, textury, meshe, audia — nejen ScriptableObjects

### 2.2 Non-Goals (v3.0)
- Cinematic produkce (Timeline, Cinemachine, Recorder) — řídí Jan
- Automatický download assetů — Jan vybírá a importuje
- Autorství shaderů, VFX, animací — vizuální nástroje u Jana
- HDRP post-process / lighting finální tuning — Claude nastaví výchozí stav, Jan ladí look
- Game feel tuning — Claude změní hodnotu na příkaz, nehodnotí zda se to dobře hraje
- Multiplayer networking, jiné enginy

---

## 3. Users and Use Cases

### 3.1 Primary User
**Jan Malák** — kreativní vedoucí projektu, zadává práci česky. Claude je junior: dostane zadání, udělá, ukáže výsledek, přijme korekci.

### 3.2 Rozdělení odpovědnosti

| Oblast | Claude (junior) | Jan (senior/art/design) |
|---|---|---|
| Gameplay C# skripty | ✅ píše, čte, opravuje | dává zadání a review |
| Scene setup | ✅ dle popisu | finální kompozice |
| Inspector wiring | ✅ dle specifikace | rozhoduje co kam patří |
| ScriptableObject data | ✅ CRUD | designuje schéma |
| Asset discovery | ✅ hledá co tam je | vybírá co tam patří |
| Screenshot pro ověření | ✅ pořídí sám | není potřeba zasahovat |
| Lighting / post-process | základní setup | finální look |
| Materiály / textury | přiřazení hotových | autorství |
| Shadery, VFX, animace | ❌ | ✅ |
| Game feel, balancing | upraví hodnoty na požádání | rozhoduje hodnoty |
| Asset výběr / import | ❌ | ✅ |

### 3.3 Core Use Cases

#### UC-01: Implementace gameplay systému
> "Udělej resource systém — hráč má dřevo, kámen a zlato, ukládá se to v ScriptableObjectu, UI v rohu obrazovky to zobrazuje."

Claude:
1. Zkontroluje, zda podobný systém v projektu už neexistuje (`find_objects`, `list_assets`)
2. Vytvoří `ResourceData` ScriptableObject a instanci; napíše `ResourceManager` a UI skript
3. Propojí reference v Inspectoru přes UCAF
4. Zkompiluje, přečte Console
5. Pořídí screenshot (`take_screenshot`) — vidí výsledek sám, bez Jana

#### UC-02: Úprava existujícího skriptu
> "V PlayerController zvyš rychlost na 8 a přidej sprint na Shift."

Claude:
1. **Přečte existující skript** (`read_file`)
2. Upraví hodnotu a přidá input handling, zapíše zpět
3. Zkompiluje, zkontroluje errory
4. Vstoupí do Play Mode, pořídí screenshot, reportne

#### UC-03: Scene setup
> "Připrav testovací scénu s terénem 100×100, hráčem uprostřed, třemi spawn pointy na okrajích."

Claude:
1. Vytvoří scénu, rozmístí objekty s transformy
2. Přikládá existující prefaby (`list_assets type=Prefab`)
3. Zaměří každý klíčový objekt ve Scene View a pořídí screenshot pro kontrolu
4. Upozorní, pokud prefab chybí

#### UC-04: Ladění chyby
> "Když stisknu útok, nic se neděje."

Claude:
1. `find_objects component_type=PlayerCombat` — najde relevantní objekt bez průchodu celé hierarchie
2. Přečte skript, přečte Console logy
3. Identifikuje problém, opraví; pokud vyžaduje vizuální nástroj, řekne to
4. Ověří screenshotem v Play Mode

#### UC-05: Hromadné wiring v Inspectoru
> "Všech 20 wave configů nastav easy/medium/hard podle pořadí."

Claude:
1. `list_assets type=WaveConfig` — najde všechny assety
2. `batch` operace — upraví všechna pole v jednom volání
3. Reportne kolik bylo změněno, vypíše přehled

#### UC-06: Orientace v existujícím projektu
> "Co máme za systémy? Co je ve scéně?"

Claude:
1. `list_scene max_depth=2` — přehled hierarchie bez zahlcení
2. `list_assets` pro různé typy — co je v projektu
3. `find_objects` pro klíčové komponenty
4. Shrne stav bez nutnosti, aby Jan cokoli vysvětloval

---

## 4. Functional Requirements

### 4.1 Scene control (nezměněno z v2.0)
- **FR-01:** Vytvořit, otevřít, uložit scénu
- **FR-02:** Vytvořit GameObject (primitivum, prefab, prázdný), nastavit transform
- **FR-03:** Najít objekt podle cesty v hierarchii
- **FR-04:** Modifikovat transform, name, tag, layer, active state
- **FR-05:** Reparenting, duplikace, smazání
- **FR-06:** Scene enumeration — hierarchie s komponenty, volitelná hloubka

### 4.2 Object query (NEW v3.0)
- **FR-07:** `find_objects` — hledat objekty podle: `component_type`, `tag`, `layer`, `name_contains`, `include_inactive`; vrátit seznam path+name bez nutnosti stáhnout celou hierarchii
- **FR-08:** `get_object_info` — rychlý souhrn jednoho objektu: transform + seznam komponent + prefab status; ušetří 2–3 roundtrips při práci s existujícím objektem

### 4.3 Inspector bridge (rozšíření z v2.0)
- **FR-09:** Číst hodnotu libovolného serializovaného pole (`get_field`)
- **FR-10:** Zapisovat hodnotu — primitiva, Vector, Color, enum, asset ref, scene ref (`set_field`)
- **FR-11:** Přidat / odebrat komponentu; vypsat seznam komponent
- **FR-12:** CRUD nad ScriptableObject assety
- **FR-13:** Materiál: vytvořit, přiřadit, nastavit shader property
- **FR-14:** Array element manipulation — nastavit jednotlivé prvky pole/listu, přidat prvek (`append_array_element`)

### 4.4 Scripting (rozšíření z v2.0)
- **FR-15:** Zapsat C# skript do Assets (i podsložek)
- **FR-16:** **Přečíst existující soubor** z Assets — vrátí obsah jako string (`read_file`)
- **FR-17:** **Smazat soubor** z Assets + refresh (`delete_file`)
- **FR-18:** Attachnout MonoBehaviour na objekt
- **FR-19:** Kompilace s čekáním na dokončení + strukturovaný výsledek s errory/warningy
- **FR-20:** AssetDatabase refresh

### 4.5 Console & runtime feedback (nezměněno)
- **FR-21:** Číst Console — errors, warnings, Debug.Log — od daného timestampu
- **FR-22:** Filtrovat podle severity
- **FR-23:** Vyčistit Console

### 4.6 Screenshot & visual feedback (rozšíření — kritické pro v3.0)
- **FR-24:** `take_screenshot` jako command (ne jen menu item) — Claude ho může volat autonomně
- **FR-25:** Screenshot vrátí cestu k souboru a počká, až soubor skutečně existuje
- **FR-26:** `select_object` — programaticky vybrat objekt v hierarchii (`Selection.activeGameObject`)
- **FR-27:** `focus_scene_view` — zaměřit Scene View na vybraný objekt (`SceneView.FrameSelected`)
- **FR-28:** Workflow: `select_object` → `focus_scene_view` → `take_screenshot` — Claude vidí přesně to co chce

### 4.7 Batch operace (NEW v3.0 — výkonnostní)
- **FR-29:** `batch` command — pole sub-commandů v payloadu, provedeno sekvenčně v jednom poll cyklu
- **FR-30:** `batch` vrátí pole výsledků (jeden per sub-command)
- **FR-31:** Parametr `stop_on_error` — při chybě přeruší nebo pokračuje dál

### 4.8 Asset discovery (rozšíření z v2.0)
- **FR-32:** `list_assets type=Prefab/Texture/AudioClip/Mesh/... folder=...` — generalizovaný asset search přes `AssetDatabase.FindAssets`
- **FR-33:** `list_scriptables` zůstává jako zkratka pro `list_assets type=ScriptableObject`

### 4.9 Generic escape hatch (NEW v3.0)
- **FR-34:** `execute_menu_item menu_path=...` — spustí libovolný Unity menu item přes `EditorApplication.ExecuteMenuItem`

### 4.10 Play Mode & prefab workflow (nezměněno)
- **FR-35:** Vstoupit / opustit Play Mode
- **FR-36:** Vytvořit prefab, instanciovat, apply / revert overrides

### 4.11 Misc (nezměněno)
- **FR-37:** Import `.unitypackage`
- **FR-38:** Základní lighting setup (directional light, fog)

---

## 5. Non-Functional Requirements

| ID | Požadavek | Cíl |
|----|-----------|-----|
| NFR-01 | Screenshot: command → soubor na disku | < 3 s |
| NFR-02 | Jednoduchý command response | < 1 s (FileSystemWatcher), < 1.5 s (polling) |
| NFR-03 | `batch` response (10 sub-commandů) | < 5 s |
| NFR-04 | Kompilace s čekáním | < 45 s |
| NFR-05 | Unity verze | 6.4 (6000.4.3f1), HDRP 17.4.0 |
| NFR-06 | Stabilita | Žádná operace nesmí zanechat scénu v tichém broken stavu |
| NFR-07 | Bezpečnost | Zápis omezen na `Assets/`, `ucaf_workspace/`, `ProjectSettings/` |
| NFR-08 | Idempotence | Opakovaný command nesmí duplikovat stav |
| NFR-09 | Poll interval | ≤ 0.5 s (default); zvážit FileSystemWatcher pro ≤ 0.1 s |

---

## 6. Junior dev protokol (aktualizováno)

### Před změnou
1. `find_objects` nebo `list_scene max_depth=2` — orientace, nehádám
2. `get_object_info` nebo `list_components` na dotčených objektech
3. `read_file` pokud upravuji existující skript — nejdřív čtu, pak píšu

### Po změně
1. `compile_and_wait` — kontrola, že kód stojí
2. `get_console since=<timestamp>` — žádné runtime errory
3. `select_object` + `focus_scene_view` + `take_screenshot` — vidím výsledek sám

### Reportovací formát
```
✅ Co jsem udělal
📁 Změněné soubory / objekty
👁️ Co vidím (screenshot + relevantní části hierarchie + Console)
⚠️ Problémy / chybějící vstupy
➡️ Co navrhuji dál / co potřebuji od tebe
```

### Kdy říct "stop"
- Požadavek na Shader Graph / VFX Graph / Animator / Timeline
- Estetické rozhodnutí bez konkrétní hodnoty (barva, intenzita, feel)
- Chybějící asset — nepouštět se do stahování, popsat co potřebuji

---

## 7. System Architecture

```
[Jan zadává v přirozené řeči]
        ↓
[Claude Code CLI]
        ↓
[Python helper → ucaf_workspace/commands/pending/]
        ↓
[UCAF_Listener.cs — poll / FileSystemWatcher]
     ↓       ↓         ↓         ↓        ↓
[Scene]  [Inspector] [Scripts] [Console] [Screenshot]
  ops      bridge    read+write  reader   + Scene View
        ↓
[Result + screenshot path + console log]
        ↓
[Claude vidí výsledek, iteruje nebo reportuje Janovi]
```

### Klíčový rozdíl oproti v2.0
V2.0: Claude provede → čeká na Jana, aby ověřil → Jan dá zpětnou vazbu  
V3.0: Claude provede → Claude vidí výsledek sám → iteruje → teprve hotové ukáže Janovi

---

## 8. Command Reference (v3.0 delta)

Kompletní reference zůstává v `CLAUDE.md`. Zde jsou nové/rozšířené commandy v3.0:

| Command | Nový/Rozšířen | Klíčové params | Returns |
|---------|---------------|----------------|---------|
| `take_screenshot` | 🆕 | – | `screenshot_path` |
| `select_object` | 🆕 | `path`/`name` | – |
| `focus_scene_view` | 🆕 | – | – |
| `find_objects` | 🆕 | `component_type`, `tag`, `layer`, `name_contains`, `include_inactive` | list of `{path, name}` |
| `get_object_info` | 🆕 | `path`/`name` | transform + components + prefab status |
| `read_file` | 🆕 | `asset_path` | `data_json.content` |
| `delete_file` | 🆕 | `asset_path` | – |
| `batch` | 🆕 | `commands` (JSON array), `stop_on_error` | array of results |
| `append_array_element` | 🆕 | object/asset + `component_type` + `field` | `data_json.new_index` |
| `list_assets` | ✏️ rozšíření | `type`, `folder` | list of asset paths |
| `execute_menu_item` | 🆕 | `menu_path` | – |

---

## 9. Success Criteria

- Jan zadá implementaci gameplay systému (UC-01 rozsah) a Claude ho dokončí **bez ručního zásahu v editoru** v >80 % případů
- Claude **vidí výsledek vlastní práce** a iteruje bez Jana v >90 % vizuálních kontrol (screenshot workflow)
- Claude správně přečte a opraví existující skript v >80 % případů (UC-02)
- Claude se nepokouší dělat věci mimo junior scope — při naražení na strop **explicitně řekne** místo obcházení
- Jan nikdy neskončí ve stavu, kdy musí hádat, co Claude udělal — report je vždy kompletní
- `batch` snižuje čekací dobu sekvencí o >60 % oproti sekvenčním commandům

---

## 10. Roadmap

### v2.0 (dokončeno)
- Inspector bridge, scene enumeration, console reader, compile_and_wait, prefab workflow

### v3.0 (tato verze — k implementaci)
- `take_screenshot` command + `select_object` + `focus_scene_view` (FR-24 až FR-28)
- `find_objects` + `get_object_info` (FR-07, FR-08)
- `read_file` + `delete_file` (FR-16, FR-17)
- `batch` command (FR-29 až FR-31)
- `list_assets` generalizovaný (FR-32)
- `append_array_element` (FR-14)
- `execute_menu_item` (FR-34)

### v3.1 (plánováno)
- FileSystemWatcher místo pollingu — latence < 0.1 s per command
- HDRP Volume bridge — read/write profile overrides přes SerializedObject
- `get_prefab_overrides` — seznam overridů na prefab instanci
- Play Mode runtime introspection — číst hodnoty instancí za běhu

### v3.2 (výhled)
- Test Runner integrace (EditMode + PlayMode testy)
- NavMesh bake trigger
- Lightmap bake trigger
- `import_settings` — AssetImporter nastavení (FBX rig, animation, normal map)

### Mimo scope (zůstává u Jana)
- Timeline / Cinemachine / Unity Recorder
- Shader Graph / VFX Graph autorství
- Animator Controller autorství
- Asset Store / automatický download
- AI image generation

---

## 11. Open Questions

| Téma | Stav | Poznámka |
|------|------|----------|
| `take_screenshot` async | 🔴 open | `ScreenCapture.CaptureScreenshot` je async — listener musí čekat na existenci souboru, ne jen volat API |
| FileSystemWatcher vs. polling | 🟡 open | FSW by snížil latenci na ~0ms; risk: Unity Editor může FSW na Windows blokovat; potřeba test |
| `batch` transakčnost | 🔴 open | Pokud batch selže v půlce, scéna je v partial state. `stop_on_error=true` pomůže; revert není implementován |
| `[SerializeReference]` a custom drawery | 🟡 open | Standardní `SerializedProperty` typy pokryje většinu případů; exotika nechat na Janovi |
| Array element manipulation — property path | 🟡 open | `myList.Array.data[2]` přes `SerializedProperty` by mělo fungovat; potřeba ověřit s `List<T>` vs. `T[]` |
| `read_file` bezpečnost | 🟢 OK | Omezit na `Assets/` a `ucaf_workspace/` — konzistentní s write sandbox |
