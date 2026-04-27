# PRD – Unity + Claude Code AI-Assisted Development Framework
**Version:** 2.0
**Author:** Jan Malák
**Date:** 2026-04-24
**Status:** OBSOLETE — nahrazeno PRD_Unity_Claude_Framework_v3.md

---

## 1. Overview

### 1.1 Project Name
**UCAF** – Unity Claude Assisted Framework

### 1.2 Purpose
UCAF spojuje Claude Code s Unity Editorem na Windows 11 tak, aby se Claude choval jako **junior Unity developer** sedící vedle tebe. Dělá implementační práci podle zadání a feedback dostává přes screenshoty a Console logy. Nenahrazuje senior developera ani art directora — ty zůstáváš ty.

Claude v roli juniora:
- Píše, upravuje a přikládá C# skripty
- Skládá scény podle popisu (objekty, transformy, prefaby, reference v Inspectoru)
- Vidí hierarchii scény a rozumí, co tam je
- Dostává zpátky chyby z kompilace a runtimu (Console)
- Po každé změně ověří výsledek screenshotem a popíše, co vidí
- Když narazí na něco, co nemá v rukou (visual tools, tuning), **řekne to a počká**

### 1.3 Vision Statement
*"Řekni juniorovi co má udělat. Udělá to, ukáže výsledek, a když narazí na strop, zeptá se."*

### 1.4 Co UCAF **není**
- Není autopilot. Není art director. Není senior architekt.
- Není náhrada za Shader Graph, VFX Graph, Timeline, Animator, Cinemachine — to jsou vizuální nástroje, které junior nepoužívá samostatně.
- Negaruje „game feel“ ani vizuální polish — ten dělá člověk.

---

## 2. Goals and Non-Goals

### 2.1 Goals
- Dát Claudovi **základní junior dev toolkit**: kód, scéna, Inspector, kompilace, runtime feedback
- **Zavřít feedback loop**: Claude vidí nejen svůj vlastní výsledek, ale i Unity Console (errors, warnings, Debug.Log)
- **Scene introspection**: Claude umí vyjmenovat hierarchii a číst stav komponent — nemusí hádat
- **Inspector bridge**: Claude umí nastavit serializovaná pole, přiřadit reference, editovat ScriptableObjects
- Podporovat iterativní vývoj v cyklu: zadání → implementace → verifikace → korekce
- Umožnit vývoj běžných gameplay systémů (AI, ekonomika, vstupy, UI logika) téměř bez ručního zásahu v editoru

### 2.2 Non-Goals (v2.0)
- Cinematic video produkce (Timeline, Cinemachine, Recorder) — řídí člověk, Claude pouze pomáhá s boilerplate skripty
- Automatický download z Asset Store / itch.io / Kenney — bezpečnostně rizikové, člověk asset vybere a importuje
- Autorství shaderů, VFX, animací — vizuální nástroje zůstávají u člověka
- HDRP post-process / lighting tuning — Claude umí nastavit výchozí stav, ale finální look ladí člověk
- Game feel tuning (hodnoty postav, křivky, feedback) — Claude upraví hodnoty, ale nehodnotí, zda se to „dobře hraje“
- Voice input, jiné enginy než Unity
- Multiplayer networking

---

## 3. Users and Use Cases

### 3.1 Primary User
**Jan Malák** — kreativní vedoucí projektu. Zadává práci v přirozené řeči (česky). Claude je junior: dostane zadání, udělá, ukáže výsledek, přijme korekci. Jan drží roli **senior developera + art directora + game designera** — rozhoduje *co* a *jak má vypadat*.

### 3.2 Rozdělení odpovědnosti

| Oblast | Claude (junior) | Jan (senior/art/design) |
|---|---|---|
| Gameplay C# skripty | ✅ píše a laděni | dává zadání a review |
| Scene setup (rozmístění objektů) | ✅ dle popisu | finální kompozice |
| Inspector wiring (reference, hodnoty) | ✅ dle specifikace | rozhoduje, co kam patří |
| ScriptableObject data | ✅ CRUD | designuje schéma |
| Lighting / post-process | základní setup | finální look |
| Materiály / textury | přiřazení hotových | autorství |
| Shadery, VFX, animace | ❌ | ✅ |
| Game feel, balancing | upraví hodnoty na požádání | rozhoduje hodnoty |
| Asset výběr / import | ❌ | ✅ |

### 3.3 Core Use Cases

#### UC-01: Implementace gameplay systému
> "Udělej resource systém — hráč má dřevo, kámen a zlato, ukládá se to v ScriptableObjectu, UI v rohu obrazovky to zobrazuje."

Claude:
1. Vytvoří `ResourceData` ScriptableObject a jeho instanci
2. Napíše `ResourceManager` MonoBehaviour
3. Napíše UI skript, attachne na Canvas
4. Propojí reference v Inspectoru přes UCAF
5. Zkompiluje, přečte Console, reportne stav

#### UC-02: Úprava existujícího skriptu
> "V PlayerController zvyš rychlost na 8 a přidej sprint na Shift."

Claude:
1. Přečte skript
2. Upraví hodnotu a přidá input handling
3. Zkompiluje, zkontroluje errory
4. Vstoupí do Play Mode, reportne (Console + screenshot)

#### UC-03: Základní scene setup
> "Připrav testovací scénu s terénem 100×100, hráčem uprostřed, třemi nepřátelskými spawn pointy na okrajích."

Claude:
1. Vytvoří scénu, rozmístí prázdné GameObjecty s transformy
2. Přikládá existující prefaby pokud v projektu jsou
3. Vyjmenuje hierarchii pro kontrolu
4. Upozorní, pokud prefab chybí — Jan ho doplní

#### UC-04: Ladění chyby
> "Když stisknu útok, nic se neděje."

Claude:
1. Enumeruje relevantní objekty a komponenty
2. Přečte Console logy z posledního Play Mode
3. Identifikuje problém (chybějící reference, null, input action není bound)
4. Navrhne nebo přímo opraví; pokud oprava vyžaduje vizuální nástroj (např. Input Action asset), řekne to

#### UC-05: Inspector wiring dávkově
> "Všech 20 wave configů v Assets/Data/Waves nastav tak, aby první tři byly easy (10 enemies), další tři medium (20), zbytek hard (40)."

Claude:
1. Načte všechny `WaveConfig` assety
2. Upraví serializovaná pole přes SerializedObject
3. Uloží, reportne kolik bylo změněno

---

## 4. Functional Requirements

### 4.1 Scene control
- **FR-01:** Vytvořit, otevřít, uložit scénu
- **FR-02:** Vytvořit GameObject (primitivum nebo instanci prefabu), nastavit transform
- **FR-03:** Najít objekt podle cesty v hierarchii (ne jen podle jména rootu)
- **FR-04:** Modifikovat transform, name, tag, layer, active state
- **FR-05:** Parent/child reparenting, duplikace, smazání
- **FR-06:** **Scene enumeration** — vrátit strom hierarchie s jmény, tagy, seznamem komponent (pro orientaci bez hádání)

### 4.2 Inspector bridge (NEW — klíčové pro junior-level)
- **FR-07:** Číst hodnotu libovolného serializovaného pole komponenty (`SerializedObject` / `SerializedProperty`)
- **FR-08:** Zapisovat hodnotu libovolného serializovaného pole (primitiva, Vector, Color, enum, reference na asset, reference na scene object)
- **FR-09:** Přidat / odebrat komponentu; vypsat seznam komponent na objektu
- **FR-10:** CRUD nad ScriptableObject assety (create, read, modify, list)
- **FR-11:** Materiál: vytvořit, přiřadit na renderer, nastavit shader property

### 4.3 Scripting
- **FR-12:** Zapsat C# skript do `Assets/Scripts/` (i podsložek)
- **FR-13:** Attachnout MonoBehaviour na objekt podle názvu třídy
- **FR-14:** Vyvolat kompilaci a **počkat na její skutečné dokončení**, ne jen vystřelit request
- **FR-15:** **Vrátit compile errors/warnings** jako strukturovaný výsledek (ne jen „žádost odeslána“)

### 4.4 Console & runtime feedback (NEW — klíčové)
- **FR-16:** Číst Unity Console — errors, warnings, `Debug.Log` výstup — od daného timestampu
- **FR-17:** Filtrovat Console podle severity a zdroje
- **FR-18:** Vyčistit Console před operací pro čistý kontext

### 4.5 Play Mode
- **FR-19:** Vstoupit/opustit Play Mode
- **FR-20:** Po ukončení Play Mode vrátit Console výstup z runtime
- **FR-21:** Screenshot Game View i Scene View zvlášť

### 4.6 Lighting (minimal, HDRP-aware)
- **FR-22:** Základní setup directional lightu (barva, intenzita v lumenech pro HDRP, rotace)
- **FR-23:** Point/Spot light CRUD
- **FR-24:** HDRP Volume profile — přečíst a nastavit základní overrides (exposure, fog) **pouze pokud Jan explicitně zadá hodnoty**; polish nechat na Janovi

### 4.7 Assets
- **FR-25:** Import `.unitypackage` souboru, který Jan umístí do workspace (ne automatický download)
- **FR-26:** AssetDatabase refresh s čekáním na dokončení
- **FR-27:** Listing assetů v projektu podle typu/cesty
- **FR-28:** Import settings FBX/textur — `AssetImporter` nastavení (rig, animation, normal map) — **pouze pokud Jan dá konkrétní hodnoty**

### 4.8 Prefab workflow
- **FR-29:** Vytvořit prefab ze scénového objektu
- **FR-30:** Instance prefabu do scény
- **FR-31:** Apply / revert overrides

### 4.9 Screenshot & workspace
- **FR-32:** Screenshot Game View nebo Scene View na vyžádání, uložení do `ucaf_workspace/screenshots/`
- **FR-33:** Všechny commandy přes file-based bridge v `ucaf_workspace/commands/{pending,done,errors}`

---

## 5. Non-Functional Requirements

| ID | Požadavek | Cíl |
|----|-----------|-----|
| NFR-01 | Screenshot latence | < 3 s |
| NFR-02 | Command response time (jednoduché) | < 5 s |
| NFR-03 | Kompilace s čekáním | < 30 s |
| NFR-04 | Unity verze | 6.4 (6000.4.3f1), HDRP |
| NFR-05 | Stabilita | Po každé operaci ověřit, Unity nesmí zůstat v rozbitém stavu |
| NFR-06 | Jazyk | Čeština a angličtina |
| NFR-07 | Bezpečnost | Žádné operace mimo `Assets/`, `ucaf_workspace/`, `ProjectSettings/` |
| NFR-08 | Idempotence | Opakované spuštění stejného commandu nesmí duplikovat stav |

---

## 6. Junior dev protokol

Claude se musí chovat jako odpovědný junior:

1. **Zadání nejdřív pochopit** — pokud je nejednoznačné, zeptat se, neimprovizovat
2. **Před větší změnou** — enumerovat relevantní část scény/projektu, potvrdit výchozí stav
3. **Po změně** — vždy zkompilovat, přečíst Console, udělat screenshot
4. **Reportovat strukturovaně** — co udělal, co vidí, co je v Console, co dál
5. **Přiznat strop** — pokud požadavek vyžaduje vizuální nástroj (Shader Graph, Animator, VFX Graph) nebo estetické rozhodnutí, řekne to a **čeká na Jana**
6. **Neimprovizovat art** — když Jan neřekne barvu/intenzitu/hodnotu, zeptá se, nevybírá sám

### Reportovací formát
```
✅ Co jsem udělal
📁 Změněné soubory / objekty
👁️ Co vidím (screenshot + hierarchie + Console)
⚠️ Problémy / chybějící vstupy
➡️ Co navrhuji dál / co potřebuji od tebe
```

---

## 7. System Boundaries

```
[Jan zadává v přirozené řeči]
        ↓
[Claude Code CLI]
        ↓
[File-based bridge: ucaf_workspace/commands/]
        ↓
[UCAF_Listener.cs v Unity Editoru]
     ↓       ↓         ↓         ↓
[Scene]  [Inspector] [Scripts] [Console]
  ops      bridge     compile   reader
        ↓
[Result + screenshot + console log]
        ↓
[Claude analyzuje a reportuje Janovi]
```

---

## 8. Success Criteria

- Jan zadá implementaci gameplay systému (UC-01 rozsah) a Claude ho dokončí **bez ručního zásahu v editoru** v >80 % případů
- Claude správně identifikuje a opraví runtime chybu na základě Console logu v >70 % případů
- Claude se nepokouší dělat věci mimo junior scope — když narazí, **explicitně řekne** místo toho, aby zkoušel obejít
- Jan nikdy neskončí ve stavu, kdy musí hádat, co Claude udělal — report je vždy kompletní
- Žádná operace nezanechá scénu v rozbitém stavu, který se tiše neobjeví až po restartu

---

## 9. Roadmap

### v2.0 (aktuální)
- Inspector bridge (FR-07 až FR-11)
- Scene enumeration (FR-06)
- Console reader (FR-16 až FR-18)
- Compilation wait + error reporting (FR-14, FR-15)
- Prefab workflow (FR-29 až FR-31)

### v2.1
- Material / shader property editing v plném rozsahu
- HDRP Volume bridge (read/write overrides)
- Play Mode runtime state introspection (proměnné instancí)

### v2.2
- Test Runner integrace (EditMode + PlayMode testy)
- NavMesh bake trigger
- Lightmap bake trigger

### Mimo scope (zůstává u Jana)
- Timeline / Cinemachine / Unity Recorder
- Shader Graph / VFX Graph autorství
- Animator Controller autorství
- Asset Store integrace
- AI image generation

---

## 10. Open Questions

| Téma | Poznámka |
|------|----------|
| Inspector bridge — jak řešit `[SerializeReference]` a custom property drawery | Zatím omezit na standardní `SerializedProperty` typy; exotika nechat na Janovi |
| Console reader — jak číst logy bez reflexe interní Unity API | LogEntries reflexe je cesta, rizikem je breaking change mezi Unity verzemi |
| Čekání na kompilaci — domain reload vs. script reload | Potřeba test, zda `CompilationPipeline.compilationFinished` stačí, nebo je nutný `AssemblyReloadEvents` |
| Bezpečnost ScriptableObject editace | Před hromadnou editací automaticky udělat backup do `ucaf_workspace/backups/` |
