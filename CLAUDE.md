# UCAF – Unity Claude Agent Framework v2.0
## Siege of the Blue World

Projekt: Strategická hra Siege of the Blue World, Unity 6.4 HDRP, 3D.
Workspace: `ucaf_workspace/` (relativně od kořene projektu)

**Role split:** Claude = junior dev, Jan = senior + art director + game designer.
Podrobnosti v `PRD_Unity_Claude_Framework.md`.

---

## Command protocol

Commandy se posílají jako JSON do `ucaf_workspace/commands/pending/`.
Výsledky v `ucaf_workspace/commands/done/`, chyby v `ucaf_workspace/commands/errors/`.

### Formát
```json
{
  "id": "cmd_001",
  "type": "list_scene",
  "timestamp": "2026-04-24T12:00:00Z",
  "params_list": [
    {"key": "include_components", "value": "true"},
    {"key": "max_depth", "value": "-1"}
  ]
}
```

### Výsledek
```json
{
  "success": true,
  "message": "...",
  "screenshot_path": "",
  "data_json": "{...}"    // structured payload (JSON string), command-specific
}
```

### Python helper
```python
import json, uuid, datetime, time, os

workspace = "C:/Projects/Hry/Siege of the Blue World/ucaf_workspace"
pending   = f"{workspace}/commands/pending"
done      = f"{workspace}/commands/done"
errors    = f"{workspace}/commands/errors"

def send_command(cmd_type, params: dict, timeout=60):
    cmd_id = f"cmd_{uuid.uuid4().hex[:8]}"
    cmd = {
        "id": cmd_id,
        "type": cmd_type,
        "timestamp": datetime.datetime.utcnow().isoformat() + "Z",
        "params_list": [{"key": k, "value": str(v)} for k, v in params.items()]
    }
    with open(f"{pending}/{cmd_id}.json", "w") as f:
        json.dump(cmd, f, indent=2)

    for _ in range(timeout * 2):
        if os.path.exists(f"{done}/{cmd_id}.json"):
            res = json.load(open(f"{done}/{cmd_id}.json"))
            if res.get("data_json"):
                res["data"] = json.loads(res["data_json"])
            return res
        if os.path.exists(f"{errors}/{cmd_id}.json"):
            return json.load(open(f"{errors}/{cmd_id}.json"))
        time.sleep(0.5)
    return {"success": False, "message": "Timeout"}
```

---

## Command reference

### Scene lifecycle
| Command | Params | Returns |
|---------|--------|---------|
| `ping` | – | `message: "pong"` |
| `create_scene` | `name` | Creates `Assets/Scenes/{name}.unity` |
| `open_scene` | `name` | Opens existing scene |
| `save_scene` | – | Saves active scene |
| `list_scene` | `include_components` (default true), `max_depth` (-1 = unlimited) | `data_json` = `UCAFSceneTree` |

### Object CRUD
Object resolution: specify `path` (hierarchy "Root/Child/Grandchild") or `name` (root-level fallback).

| Command | Params | Notes |
|---------|--------|-------|
| `create_object` | `name`, (`primitive` Cube/Sphere/… OR `prefab_path` OR neither=empty GO), `parent`, `position`, `rotation`, `scale`, `tag`, `layer`, `active` | |
| `modify_object` | `path`/`name`, `new_name`, `position`, `rotation`, `scale`, `tag`, `layer`, `active` | |
| `delete_object` | `path`/`name` | |
| `reparent_object` | `path`/`name`, `new_parent` (empty = root), `keep_world_position` (default true) | |
| `duplicate_object` | `path`/`name`, `new_name`, transform overrides | |

### Inspector bridge — components
| Command | Params | Returns |
|---------|--------|---------|
| `add_component` | object + `component_type` | |
| `remove_component` | object + `component_type`, `index` (default 0) | |
| `list_components` | object | `data_json.items` = list of component type names |

### Inspector bridge — fields
Target: either a component (object + `component_type` + optional `index`) or an asset (`asset_path` to ScriptableObject/Material/etc.).

| Command | Params | Returns |
|---------|--------|---------|
| `list_fields` | target | `data_json.fields` = [{name, display_name, type}] |
| `get_field` | target + `field` (property path) | `data_json` = `{field_name, type, value}` |
| `set_field` | target + `field` + `value` (encoded string) | |

**Value encoding:**
- Numeric: `"42"`, `"3.14"` (InvariantCulture)
- Boolean: `"true"` / `"false"`
- Enum: enum value name (e.g. `"Directional"`) or index
- Vector2/3/4/Quat: comma-separated `"1,2,3"`
- Color: `"#RRGGBBAA"`
- Object reference (asset): asset path `"Assets/Data/config.asset"`
- Object reference (scene): `"scene:Root/Child"` or `"scene:Root/Child#ComponentType"`

### ScriptableObjects
| Command | Params | Returns |
|---------|--------|---------|
| `create_scriptable` | `class_name`, `asset_path` | Creates `.asset` file |
| `list_scriptables` | `class_name` (optional filter), `folder` (default "Assets") | `data_json.items` = list of asset paths |

### Materials
| Command | Params | Notes |
|---------|--------|-------|
| `create_material` | `asset_path`, `shader` (default "HDRP/Lit") | |
| `assign_material` | object + `material_path`, `slot` (default 0) | |
| `set_material_prop` | `material_path`, `property` (e.g. "_BaseColor"), `kind` (color/float/vector/texture), `value` | |

### Scripting
| Command | Params | Notes |
|---------|--------|-------|
| `create_script` | `class_name`, `content`, `folder` (default "Scripts") | Writes to `Assets/{folder}/{class_name}.cs` + Refresh |
| `attach_script` | object + `class_name` | Requires compiled first |
| `compile_check` | – | Fire-and-forget (legacy) |
| `compile_and_wait` | `timeout` (seconds, default 45) | **Async.** Returns `UCAFCompileResult` with errors/warnings after compile + domain reload complete |
| `asset_refresh` | – | AssetDatabase.Refresh |

### Console
| Command | Params | Returns |
|---------|--------|---------|
| `get_console` | `since` (ISO timestamp), `severity` (error/warning/log/empty=all), `max` (default 200) | `data_json.entries` = [{timestamp, type, message, stack_trace}] |
| `clear_console` | – | Clears both UCAF log buffer and Unity's built-in console |

### Prefab workflow
| Command | Params | Notes |
|---------|--------|-------|
| `create_prefab` | object + `asset_path`, `connect` (default true) | SaveAsPrefabAsset(AndConnect) |
| `apply_prefab` | object (must be prefab instance) | Apply overrides |
| `revert_prefab` | object (must be prefab instance) | Revert overrides |

### Runtime / misc
| Command | Params | Notes |
|---------|--------|-------|
| `play_mode` | `enter` (true/false, default true) | |
| `import_asset` | `package_path`, `interactive` (default false) | `.unitypackage` import |
| `set_lighting` | `fog`, `fog_color`, `fog_density`, `ambient_color`, `light_color`, `light_intensity`, `light_rotation_x`, `light_rotation_y` | Basic only; HDRP Volume polish → use `set_field` on Volume profile |
| `setup_alien` | – | Legacy helper |

---

## Startup protocol

1. Ověř, že `ucaf_workspace/commands/pending/` existuje a je zapisovatelné
2. Pošli `ping` → `pong` — UCAF aktivní
3. Použij `list_scene` pro orientaci ve scéně před jakoukoliv úpravou
4. Po každé změně **kompilace + console**:
   - `compile_and_wait` → zjisti, že nic nevybuchlo
   - `get_console severity=error` → žádné runtime chyby
5. Pokud něco vyžaduje vizuální nástroj (Shader Graph, VFX Graph, Animator, Timeline, Cinemachine) → **stop a zeptej se Jana**

---

## Junior dev protokol

### Před změnou
- `list_scene` — vím, co tam je, nehádám
- `list_components` na dotčených objektech — vím, co je navěšené
- `list_fields` — vím, která serializovaná pole mám k dispozici

### Po změně
- `compile_and_wait` — kontrola, že kód stojí
- `get_console` (since = timestamp před operací) — žádné errors
- `UCAF/Take Screenshot` menu nebo screenshot přes Python → vidím výsledek
- **`log_bug`** — po každém vyřešeném bugu zapsat do BugLedgeru (viz sekce níže)

### Bug tracking (povinné)

Po každém nalezeném a opraveném bugu **vždy** zapsat `log_bug` command do `ucaf_workspace/commands/pending/`. Bez výjimek — i pro jednořádkové opravy.

Povinné parametry: `title`, `symptom`, `root_cause`, `fix`
Doporučené: `scope_files`, `scope_components`, `scope_scenes`, `tags`, `lessons`

```json
{
  "id": "logbug-<hash>",
  "type": "log_bug",
  "timestamp": "...",
  "params_list": [
    {"key": "title",       "value": "Krátký název bugu"},
    {"key": "symptom",     "value": "Co uživatel vidí / co selhává"},
    {"key": "root_cause",  "value": "Proč to nastalo"},
    {"key": "fix",         "value": "Co jsem změnil a kde"},
    {"key": "scope_files", "value": "Assets/Scripts/Foo.cs,Assets/Prefabs/Bar.prefab"},
    {"key": "tags",        "value": "camera,prefab,serialization"},
    {"key": "lessons",     "value": "Co si z toho odnést do budoucna"}
  ]
}
```

Před hlášením nového bugu nejdřív zkontroluj `find_similar_bugs symptom=...` — možná jsme to řešili.

### Reportovací formát
```
✅ Co jsem udělal
📁 Změněné soubory / objekty
👁️ Co vidím (screenshot + relevantní části hierarchie + console)
⚠️ Problémy / chybějící vstupy
➡️ Co navrhuji dál / co potřebuji od Jana
```

### Kdy říct „stop"
- Požadavek na shader / VFX Graph / animaci
- Požadavek na estetické rozhodnutí (barva, intenzita, feel) bez konkrétní hodnoty
- Chybějící asset — nestahovat sám, popsat co potřebuji a počkat

---

## Projekt struktura

```
Assets/
  Editor/
    UCAF_Types.cs                       shared types
    UCAF_Listener.cs                    core polling + dispatch + log capture
    UCAF_Listener.Scene.cs              scene/object ops, hierarchy enum, helpers
    UCAF_Listener.Inspector.cs          SerializedObject bridge, SO, materials
    UCAF_Listener.Scripting.cs          scripts + compile_and_wait (async)
    UCAF_Listener.Console.cs            get_console / clear_console
    UCAF_Listener.Misc.cs               prefab, playmode, import, lighting
    UCAF_Tools.cs                       UCAF/ menu items
    AlienSetup.cs                       legacy alien helper
  Scripts/                              generated by Claude
  Scenes/
  Entities/Alien_unequipped/
  Settings/                             HDRP pipeline settings

ucaf_workspace/
  commands/{pending,done,errors}/       file-based bridge
  logs/buffer.ndjson                    persistent log buffer (survives domain reload)
  screenshots/{latest.png,archive/}
  assets/
  output/
```

---

## Technické poznámky

- Unity 6.4 (6000.4.3f1), **HDRP** 17.4.0, 3D
- New Input System aktivní
- Timeline 1.8.12 (není ovládáno přes UCAF — řídí Jan)
- **Compile-and-wait** používá `SessionState` pro přežití domain reloadu + diff persistentního log souboru od offsetu
- **Console buffer** je in-memory (cap 1000 záznamů) + persistentní `logs/buffer.ndjson` pro reload
- Inspector bridge pracuje přes `SerializedObject` / `SerializedProperty` — podporuje standardní typy + ObjectReference. Exotické `[SerializeReference]` a custom drawery nemusí fungovat.
- Security: zápis omezen na `Assets/`, `ucaf_workspace/`, `ProjectSettings/`

---

## Menu items v Unity
- `UCAF/Take Screenshot`
- `UCAF/Show Workspace Status`
- `UCAF/Clear Done Commands`
- `UCAF/Open Workspace Folder`
