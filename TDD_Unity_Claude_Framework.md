# TDD – Technical Design Document
## Unity Claude Assisted Framework (UCAF)
**Version:** 1.0  
**Author:** Jan Malák  
**Date:** 2026-04-18  
**Unity Version:** 6.4 (6000.4.3f1) | **OS:** Windows 11 | **Interface:** Claude Code CLI

---

## 1. Architecture Overview

UCAF consists of three layers that work together:

```
┌─────────────────────────────────────────────────────┐
│                  LAYER 1: USER                       │
│         Jan types natural language command           │
│              in Claude Code terminal                 │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│               LAYER 2: BRIDGE                        │
│         Python MCP Server (ucaf_bridge.py)           │
│   - Screenshot capture (pyautogui / mss)             │
│   - Sends image to Claude                            │
│   - Writes/executes Unity Editor scripts             │
│   - Manages asset downloads                          │
└────────────────────┬────────────────────────────────┘
                     │
┌────────────────────▼────────────────────────────────┐
│               LAYER 3: UNITY                         │
│         Unity Editor + C# Editor Scripts             │
│   - Executes scene operations                        │
│   - Compiles and runs game logic                     │
│   - Renders video via Unity Recorder                 │
└─────────────────────────────────────────────────────┘
```

---

## 2. Component Specifications

### 2.1 Screenshot Capture Module (`ucaf_screenshot.py`)

**Purpose:** Captures what is currently visible in the Unity Editor window.

**Technology:** `mss` (fast screen capture) + `Pillow` (image processing)

**Behavior:**
- Finds the Unity Editor window by title (`"Unity"`) using `pygetwindow`
- Captures the full Unity Editor window region
- Saves to `ucaf_workspace/screenshots/latest.png`
- Also saves timestamped archive copy

**Key functions:**
```
capture_unity_window()       → saves screenshot, returns file path
find_unity_window()          → returns window bounds (x, y, width, height)
annotate_screenshot(regions) → optionally highlights Scene/Hierarchy/Inspector
```

**Screenshot naming convention:**
```
ucaf_workspace/screenshots/
  latest.png              ← always current
  archive/
    2026-04-18_14-32-01.png
```

---

### 2.2 Unity Bridge Module (`ucaf_bridge.py`)

**Purpose:** Main controller. Receives intent from Claude Code, translates into Unity operations, executes them, and returns results.

**Technology:** Python 3.11+, subprocess, file-based communication with Unity

**Communication protocol with Unity:**

Since Unity Editor does not expose a REST API by default, communication uses a **file-based command queue**:

```
ucaf_workspace/
  commands/
    pending/       ← Claude writes .json command files here
    done/          ← Unity moves completed commands here
    errors/        ← Unity writes error details here
  scripts/
    generated/     ← Claude writes C# Editor scripts here
  screenshots/
    latest.png
  assets/
    index.json     ← local asset library index
    downloads/     ← downloaded asset packages
```

**Command file format (`command_001.json`):**
```json
{
  "id": "cmd_001",
  "type": "create_object",
  "timestamp": "2026-04-18T14:32:01Z",
  "params": {
    "primitive": "Cube",
    "name": "Castle_Wall",
    "position": [0, 0, 0],
    "scale": [10, 5, 1]
  }
}
```

**Supported command types:**

| Command Type | Description |
|---|---|
| `screenshot` | Capture current Unity view |
| `create_scene` | Create new Unity scene |
| `open_scene` | Open existing scene by name |
| `save_scene` | Save current scene |
| `create_object` | Add primitive or prefab to scene |
| `modify_object` | Change position/rotation/scale/material |
| `delete_object` | Remove object from scene |
| `create_script` | Write C# script to Assets/Scripts/ |
| `attach_script` | Attach script component to GameObject |
| `set_lighting` | Configure scene lighting |
| `add_postfx` | Add post-processing effect |
| `create_timeline` | Create Unity Timeline sequence |
| `record_video` | Trigger Unity Recorder |
| `import_asset` | Import .unitypackage into project |
| `compile_check` | Force recompile and return errors |
| `play_mode` | Enter/exit Play Mode |

---

### 2.3 Unity Editor Listener (`UCAF_Listener.cs`)

**Purpose:** C# Editor script that runs inside Unity, monitors the command queue, and executes operations.

**Location in Unity project:** `Assets/Editor/UCAF_Listener.cs`

**Technology:** Unity Editor scripting, `EditorApplication.update`, `FileSystemWatcher`

**Behavior:**
- Registered as `[InitializeOnLoad]` — starts automatically when Unity opens
- Polls `ucaf_workspace/commands/pending/` every 500ms
- Reads command JSON, executes corresponding Unity API calls
- Moves command file to `done/` on success or `errors/` on failure
- Writes result JSON with outcome, any error messages, and screenshot path

**Key Unity APIs used:**

```csharp
// Scene management
EditorSceneManager.NewScene()
EditorSceneManager.OpenScene()
EditorSceneManager.SaveScene()

// Object creation
GameObject.CreatePrimitive()
PrefabUtility.InstantiatePrefab()
AssetDatabase.LoadAssetAtPath<T>()

// Transform
obj.transform.position = new Vector3(x, y, z);
obj.transform.localScale = new Vector3(x, y, z);

// Asset import
AssetDatabase.ImportPackage()
AssetDatabase.Refresh()

// Lighting
RenderSettings.fog = true;
RenderSettings.ambientLight = Color;

// Compilation
CompilationPipeline.RequestScriptCompilation()

// Play mode
EditorApplication.isPlaying = true/false;
```

---

### 2.4 Asset Management Module (`ucaf_assets.py`)

**Purpose:** Maintains a local index of available assets and handles downloads from approved sources.

**Asset Library Index (`assets/index.json`):**
```json
{
  "last_updated": "2026-04-18T14:00:00Z",
  "packages": [
    {
      "name": "Medieval Castle Pack",
      "source": "kenney.nl",
      "path": "assets/downloads/medieval_castle.unitypackage",
      "tags": ["medieval", "castle", "architecture", "stone"],
      "imported": true
    }
  ]
}
```

**Approved free asset sources:**

| Source | URL | Format | Notes |
|--------|-----|--------|-------|
| Kenney.nl | kenney.nl/assets | .zip (FBX/OBJ) | Best free 3D assets |
| OpenGameArt | opengameart.org | various | CC0 license |
| Unity Asset Store (free) | assetstore.unity.com | .unitypackage | Requires Unity login |
| Sketchfab (free CC0) | sketchfab.com | .glb/.fbx | High quality |
| itch.io (free assets) | itch.io/game-assets/free | various | Curated by Claude |

**Asset search strategy:**
1. Check local index first (fastest)
2. If not found locally → search approved sources
3. Download + convert to `.unitypackage` if needed
4. Import into Unity via `AssetDatabase.ImportPackage()`
5. Update local index

---

### 2.5 Video Production Module (`ucaf_video.py` + `UCAF_Recorder.cs`)

**Purpose:** Handles creation of cinematic sequences and video export.

**Pipeline:**

```
Natural language description
         ↓
Claude generates Timeline script
         ↓
UCAF_Listener.cs creates Timeline asset
         ↓
Cinemachine virtual cameras placed along path
         ↓
Post-processing (color grading, DOF, bloom)
         ↓
Unity Recorder configured (MP4, 1080p, 30fps)
         ↓
Play Mode triggered → video recorded
         ↓
Output: ucaf_workspace/output/video_YYYYMMDD_HHMMSS.mp4
```

**Unity packages required:**
- `com.unity.cinemachine` (camera system)
- `com.unity.recorder` (video export)
- `com.unity.render-pipelines.universal` (URP, for post-processing)

---

## 3. Project Folder Structure

### 3.1 Unity Project Structure
```
[Unity Project Root]/
  Assets/
    Editor/
      UCAF_Listener.cs        ← Command queue listener
      UCAF_Tools.cs           ← Helper Editor utilities
    Scripts/
      [Generated game scripts by Claude]
    Scenes/
      [User scenes]
    Prefabs/
      [Imported and custom prefabs]
    Materials/
    Textures/
    Audio/
  Packages/
    manifest.json             ← Must include Cinemachine, Recorder, URP
  ucaf_workspace/             ← Bridge communication folder
    commands/
      pending/
      done/
      errors/
    screenshots/
    scripts/generated/
    assets/
      index.json
      downloads/
    output/
      videos/
      logs/
```

### 3.2 Claude Code / Python Structure
```
ucaf/
  ucaf_bridge.py              ← Main controller
  ucaf_screenshot.py          ← Screenshot capture
  ucaf_assets.py              ← Asset management
  ucaf_video.py               ← Video production helpers
  ucaf_config.json            ← Paths, settings
  requirements.txt            ← Python dependencies
  README.md                   ← Setup instructions
```

---

## 4. Configuration (`ucaf_config.json`)

```json
{
  "unity_project_path": "C:/Users/Jan/Documents/Unity/UCAF_Project",
  "ucaf_workspace": "C:/Users/Jan/Documents/Unity/UCAF_Project/ucaf_workspace",
  "unity_executable": "C:/Program Files/Unity/Hub/Editor/6000.4.3f1/Editor/Unity.exe",
  "screenshot_interval_ms": 500,
  "command_poll_interval_ms": 500,
  "max_command_wait_seconds": 60,
  "video_output_resolution": [1920, 1080],
  "video_output_fps": 30,
  "video_output_format": "MP4",
  "approved_asset_sources": [
    "kenney.nl",
    "opengameart.org",
    "sketchfab.com"
  ],
  "default_language": "cs"
}
```

---

## 5. Dependencies

### Python (requirements.txt)
```
mss>=9.0.1              # Fast screen capture
Pillow>=10.0.0          # Image processing
pygetwindow>=0.0.9      # Find Unity window
pyautogui>=0.9.54       # Fallback screen interaction
requests>=2.31.0        # Asset downloads
watchdog>=4.0.0         # File system monitoring
```

### Unity Packages (manifest.json additions)
```json
"com.unity.cinemachine": "3.1.1",
"com.unity.recorder": "5.1.0",
"com.unity.render-pipelines.universal": "17.0.3",
"com.unity.timeline": "1.8.7"
```

---

## 6. Error Handling

| Error Condition | Detection | Recovery Action |
|---|---|---|
| Unity not running | Window not found | Alert user; do not proceed |
| Command timeout | No response in 60s | Log error; screenshot current state |
| Script compile error | CompilationPipeline callback | Report error to Claude; attempt auto-fix |
| Asset import failure | AssetDatabase exception | Log; try alternative asset source |
| Scene not saved | Unsaved changes detected | Auto-save before operations |
| Play mode crash | Unity process dies | Restart detection; reload last scene |

---

## 7. Security and Safety

- Claude Code only writes to `Assets/Scripts/` and `ucaf_workspace/` — never to Engine files
- Downloaded assets are scanned for known malware signatures before import
- No Unity project settings are modified without explicit command
- All generated C# scripts are saved to disk (reviewable) before execution
- Play Mode is only entered when explicitly requested

---

## 8. Setup Checklist (First Time)

1. Install Unity 6.4 via Unity Hub (manual, one-time)
2. Create new Unity project: **Universal 3D (URP)** template
3. Copy `Assets/Editor/UCAF_Listener.cs` and `Assets/Editor/UCAF_Tools.cs` into project
4. Install Python packages: `pip install -r ucaf/requirements.txt`
5. Edit `ucaf_config.json` with correct paths
6. Open project in Unity — Listener auto-starts
7. Run `python ucaf/ucaf_bridge.py` to verify bridge is active
8. Test: type first command in Claude Code terminal
