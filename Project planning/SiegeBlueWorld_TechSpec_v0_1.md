**SIEGE OF THE BLUE WORLD**

TECH SPEC — Technická specifikace mechanik

Verze: 0.1 — V1 Vertical Slice

*Living document — balancing tabulky se iterují podle playtestů.*

---

## ÚVOD

Tento dokument obsahuje **konkrétní čísla, mechaniky a balancing tabulky** pro implementaci. Slouží primárně programátorům a level designerům.

### Vztah k ostatním dokumentům

- **PRD** = CO hra je (vize, designová rozhodnutí)
- **Game Bible** = JAK svět funguje (pravidla)
- **Tech Spec** (tento dokument) = **JAKÉ JSOU MECHANIKY** (čísla, tabulky, parametry)
- **Workflow Checklist** = JAK ji udělat (fáze, kroky)

### Konvence pojmenování

Pro Claude Code a programátorský kód jsou klíčové termíny v **angličtině** (následují Unity konvence). Vysvětlení v češtině.

---

# 0. KAMERA — TPS SETUP

> Tato sekce dokumentuje kompletní nastavení TPS kamery tak, aby bylo možné ji přesně reprodukovat při přechodu z Alien controlleru na Hero controller nebo při jakémkoliv přenastavování.

## 0.1 Architektura — přehled

```
[Scene root]
├── Main Camera          ← CinemachineBrain + AudioListener (žádná Camera logika)
├── CM_TPS_vcam          ← CinemachineVirtualCamera (Follow + Body pouze)
│
└── Alien (nebo Hero)    ← CharacterController + PlayerController script
    └── CmAimTarget      ← CmAimController script (pitch/vertikální rotace)
```

**Klíčová pravidla architektury:**
- `Main Camera` je **vždy root-level** GO — nikdy child character prefabu
- Všechny `Camera` komponenty uvnitř character prefabu jsou **vypnuté** (Camera.enabled = false)
- `CM_TPS_vcam.Follow` → `CmAimTarget` (ne root postava)
- `CM_TPS_vcam.LookAt` → root postava (ne CmAimTarget)
- **Žádný CinemachineComposer** — Composer + Cinemachine3rdPersonFollow si navzájem přebíjejí řízení

## 0.2 Hodnoty — Alien (aktuální, ověřeno v OutdoorsScene)

### CharacterController (Alien.prefab)
| Parametr | Hodnota |
|---|---|
| `height` | 1.8 m |
| `radius` | 0.4 m |
| `center` | (0, 0.9, 0) |
| `slopeLimit` | 45° |
| `stepOffset` | 0.3 m |

### CmAimTarget (child Aliena)
| Parametr | Hodnota |
|---|---|
| `localPosition` | (0, 1.4, 0) — výška ramen/hrudi |
| `localRotation` | identity |

### CmAimController (na CmAimTarget)
| Parametr | Hodnota |
|---|---|
| `sensitivity` | 0.15 |
| `minPitch` | −40° |
| `maxPitch` | +60° |

### AlienController — Mouse Look
| Parametr | Hodnota |
|---|---|
| `mouseSensitivity` | 0.3 (horizontální rotace, Y-osa) |

### CinemachineVirtualCamera (CM_TPS_vcam)
| Parametr | Hodnota |
|---|---|
| `Priority` | 10 |
| `Follow` | `Alien/CmAimTarget` |
| `LookAt` | `Alien` (root) |
| `FieldOfView` | 60° |

### Cinemachine3rdPersonFollow (body CM_TPS_vcam)
| Parametr | Hodnota | Poznámka |
|---|---|---|
| `CameraDistance` | 4.0 m | vzdálenost kamery od Follow targetu |
| `ShoulderOffset` | (0.4, 0, 0) | posun doprava (pravé rameno) |
| `VerticalArmLength` | 0.3 m | výška nad Follow targetem |
| `CameraSide` | 1.0 | pravá strana (1 = plně vpravo) |
| `CameraRadius` | 0.2 m | kolize kamery |
| `Damping` | **(0, 0, 0.3)** | X/Y = 0 (bez lag), Z = 0.3 (přiblížení) |
| `DampingIntoCollision` | 0.5 | zrychlení při nárazu |
| `DampingFromCollision` | 2.5 | pomalé oddálení od kolize |

> ⚠ **Kritické:** Damping X a Y musí být **0**. Hodnoty > 0 způsobují třes/jitter kamery u statické postavy (BUG-0012).

## 0.3 Postup přenastavení kamery pro novou postavu (recept)

Při přidání nové hratelné postavy (Hero nebo jiné) postupuj takto:

```
1. Přidej do character prefabu child GO "CmAimTarget"
   - localPosition: (0, [výška ramen], 0)  → Alien=1.4, Hero=1.1
   - Přidej CmAimController: sensitivity=0.15, minPitch=-40, maxPitch=60

2. Zkontroluj, že žádná Camera komponenta uvnitř prefabu není enabled=true

3. Na CM_TPS_vcam nastav:
   - Follow  → [nová postava]/CmAimTarget
   - LookAt  → [nová postava] (root)

4. Na Cinemachine3rdPersonFollow nastav hodnoty z tabulky 0.3

5. V controller scriptu:
   - Horizontální rotace: transform.Rotate(Vector3.up, delta.x * 0.3)
   - Pitch deleguj na CmAimController (nevytvářej vlastní logiku)

6. Ověř: Play Mode → pohyb myší, bez třesu, pitch clampovaný
```

## 0.5 Diagnostika — časté problémy

| Symptom | Příčina | Fix |
|---|---|---|
| Kamera se třese/kmitá | Damping X nebo Y > 0 | Nastavit na 0 |
| Kamera letí do vzduchu | CinemachineComposer přítomen zároveň s 3rdPersonFollow | Odebrat Composer |
| Pitch nefunguje | Serialized hodnota v prefabu přebila script default | Použít `set_field` na instanci, ne jen kód |
| Animace posouvá mesh mimo záběr | `applyRootMotion = true` na Mixamo FBX | Nastavit false v Inspectoru i v kódu (Awake) |
| Kamera je uvnitř postavy | `CameraDistance` příliš malé nebo `VerticalArmLength` záporné | Zvýšit CameraDistance ≥ 2.5 |
| Obě kamery aktivní najednou | Stará Camera GO zůstala v scéně bez CinemachineBrain | Smazat nebo deaktivovat |

---

# 1. DAMAGE MODEL

## 1.1 Body parts hitbox systém

Každá postava (hráč, alien, Puppet, war dog) má **multi-part hitbox**:

| Hitbox | Český popis | Damage multiplier |
|---|---|---|
| `Head` | Hlava | 4.0× |
| `Torso` | Trup | 1.0× (base) |
| `LimbUpper` | Ruka horní část | 0.6× |
| `LimbLower` | Ruka dolní část | 0.4× |
| `Leg` | Noha | 0.5× |

## 1.2 Weapon damage tabulka (V1)

| Zbraň | Base damage | Headshot | Body | Limb |
|---|---|---|---|---|
| **Pistol** | 50 | 200 (kill) | 50 (critical) | 30 (injury) |
| **Crossbow** | 80 | 320 (kill) | 80 (kill?) | 48 (critical) |
| **Hunting Rifle** | 120 | 480 (kill) | 120 (kill) | 72 (critical) |
| **Alien Energy Pistol** | 70 | 280 (kill) | 70 (critical) | 42 (injury) |
| **Melee (Knife)** | 40 | 160 (critical) | 40 (injury) | 24 (minor) |
| **Melee (Backstab)** | 999 | 999 (kill) | 999 (kill) | — |
| **Frag Grenade** | 200 (radius) | — | — | — |

**HP base values:**
- Hráč: 100 HP (s critical injury threshold ~30 HP)
- Alien: 100 HP
- War Dog: 80 HP
- Puppet: 60 HP (slabší kvůli lidskému tělu)
- Puppeteer: 100 HP (ale schovává se)
- Hostile Human: 100 HP

## 1.3 Critical Injury State

Když HP padne pod **30%** (nebo bezprostředně po body shot s vysokým damage):

| Efekt | Hodnota |
|---|---|
| Movement speed | -50% |
| Aim drift | +30% deviation |
| Bleeding tick | -2 HP/s (pokud neošetřený) |
| Time to death | ~60 s bez ošetření |
| Vision blur | mírná, eskaluje s časem |

**Recovery:**
- Medkit application: ~5 s animation
- Stops bleeding immediately
- Heals to 50% HP
- Druhý medkit po 30 s pro plné HP

## 1.4 Headshot mechanika

- **Headshot zezadu** = vždy instant kill (i s pistole)
- **Headshot frontálně** = instant kill, pokud zbraň má dostatečný damage (Crossbow+, Rifle, Alien Pistol)
- **Pistol headshot frontálně** = critical injury, ne instant kill (slabá zbraň)
- **Brnění alien postav** může pokrývat hlavu — body shot je často efektivnější

## 1.5 Healing system

| Item | Healing | Animation time | Vulnerability |
|---|---|---|---|
| **Bandage** | Stops bleeding only | 3 s | Animovaný |
| **Medkit (light)** | +50 HP, stops bleeding | 5 s | Animovaný, slow movement |
| **Medkit (heavy)** | +100 HP, stops bleeding | 8 s | Animovaný, immobile |
| **Alien Healing Stim** | +100 HP instant | 2 s | Animovaný, +alignment alien |

**Pravidla:**
- Healing v boji možný, ale **5-8 s zranitelnost**
- Bez krytu = pravděpodobná smrt
- Některá zranění mohou vyžadovat návrat do hub (V2)

---

# 2. RECON DRONE SPECIFICATION

## 2.1 Tier 1 — Scout Drone (V1 funkční)

| Atribut | Hodnota |
|---|---|
| `MaxRange` | 100 m od hráče |
| `MaxBatteryTime` | 60 s aktivního letu |
| `MaxSpeed` | 8 m/s |
| `Acceleration` | 4 m/s² |
| `RotationSpeed` | 90°/s |
| `MaxAltitude` | 30 m nad terénem |
| `NoiseLevel` | Tichý (~10m detection range pro aliény) |
| `HP` | 50 (dva výstřely = sestřelený) |
| `InventorySlot` | 1 slot |
| `CameraFOV` | 60° |
| `DetectionUploadRate` | 1× za sekundu (update mapy) |

## 2.2 Drone control mechanika

**Aktivace:**
- Inventory → drone → klik nebo klávesa F
- Animace: hráč vytáhne drona, ~2s setup

**Ovládání:**
- Hráč fyzicky **nehybný a krčí se** (vulnerability)
- Kamera switch na drone view
- WASD = pohyb drona
- Mouse = rotace
- Q/E = výška
- ESC = ukončit ovládání, vrátit kameru

**Battery management:**
- 60 s aktivního letu
- Pokud baterie dojde, drone klesne / spadne
- Pokud je drone příliš daleko (>100m) = ztráta signálu, drone se vrátí auto

**Detection:**
- Drone vidí aliény podle `LineOfSight` raycast
- Přidává body do `MapDataSystem` (sekce 3)

## 2.3 EMP zóna logika

```
if (drone.Position.IsInsideEMPZone) {
    drone.SystemFailure();
    drone.Drop();  // Padá k zemi
    drone.MarkAsLost();  // Hráč ztrácí drona
}
```

EMP zóny jsou definovány jako sférické trigger zóny kolem alien tech struktur.

## 2.4 Drone loss mechanika

Pokud je drone:
- Sestřelený (HP = 0)
- V EMP zóně (system failure)
- Mimo range bez návratu (signal lost)

→ **Hráč ztrácí drona**. Pozice ztraceného dronu se zaznamená na mapu. Hráč ho může:
- Najít a sebrat (částečné funkční / na opravu)
- Craftovat náhradního z dílů

## 2.5 Tier 2 / Tier 3 — Architektura připravena

Pro V1 implementujeme **rozšiřitelnou architekturu**:

```csharp
public abstract class ReconDroneBase : MonoBehaviour {
    public float MaxRange;
    public float MaxBatteryTime;
    public float MaxSpeed;
    // Detection capabilities (overridable)
    public virtual bool DetectThroughCover() => false;
    public virtual bool HasAudioSensor() => false;
    public virtual bool CanCarryPayload() => false;
}

public class Tier1ScoutDrone : ReconDroneBase {
    // Defaults
}

public class Tier2ReconDrone : ReconDroneBase {
    public override bool DetectThroughCover() => true;  // Biometrika
    public override bool HasAudioSensor() => true;
}

public class Tier3TacticalDrone : ReconDroneBase {
    public override bool CanCarryPayload() => true;
}
```

V1 implementuje **Tier 1**, ale architektura umožňuje rozšíření na Tier 2/3 bez refactoringu.

**⚠ TBD:** Které features Tier 2 jsou ve V1 (biometrika? audio sensor?). Zatím architektura připravena.

---

# 3. GPS / TACTICAL MAP SYSTEM

## 3.1 Map data structure

```csharp
public class MapDataPoint {
    public Vector3 WorldPosition;
    public DataPointType Type;  // Alien, AlienTech, Patrol, EMPZone, Loot, ...
    public float TimestampSeconds;
    public float DecayRate;  // 0 for static, >0 for dynamic
    public bool IsStatic;
}

public enum DataPointType {
    AlienSpotted,
    AlienTechStatic,    // Modul, věž — permanentní
    EMPZone,            // Permanentní
    PatrolRoute,        // Linka mezi body, decay
    PuppetSpotted,
    PuppeteerSpotted,
    LootContainer,
    PlayerNote          // V2 personal markers
}
```

## 3.2 Heatmap colors mapping

| Stav | Barva | RGB | Trvání |
|---|---|---|---|
| Fresh clear (< 5 min scout) | 🟢 Sytá zelená | (0, 200, 0) | Auto-fade |
| Aging clear (5-10 min) | 🟡 Žluto-zelená | (180, 220, 0) | Auto-fade |
| Stale (> 10 min) | ⚪ Šedá | (150, 150, 150) | Mizí ze zobrazení |
| Recent alien (< 5 min) | 🔴 Červená | (220, 0, 0) | Pulsing |
| Aging alien (5-10 min) | 🟠 Oranžová | (255, 140, 0) | Auto-fade |
| Permanent alien tech | ⚫ Tmavě červená | (120, 0, 0) | Static, nestárne |
| EMP zone | 🟣 Fialová | (140, 0, 200) | Static |

## 3.3 Decay timing (V1 návrh — TBD finální)

```csharp
public static class MapDecayConfig {
    // Clear zone (no enemies seen)
    public const float ClearFresh_Duration = 5f * 60f;       // 5 min
    public const float ClearAging_Duration = 10f * 60f;      // 10 min
    
    // Alien sighting
    public const float AlienRecent_Duration = 5f * 60f;      // 5 min
    public const float AlienAging_Duration = 10f * 60f;      // 10 min
    public const float AlienStale_Duration = 30f * 60f;      // 30 min total
    
    // Beyond stale = removed from map
}
```

**⚠ TBD:** Tyto časy budou iterovány na základě playtestů.

## 3.4 Map update logic

**Kdy se mapa updatuje:**
- ✅ Když hráč ovládá drona (real-time během drone fly)
- ✅ Když hráč fyzicky vidí aliéna (LineOfSight z hráčovy pozice)
- ✅ Při alien sense aktivaci (alien-aligned path)
- ❌ Real-time bez akce (žádný "god view")
- ❌ NPC info bez explicitního dialogu (V2/V3)

**Update frekvence:**
- Při drone fly: 1× za sekundu
- Pasivně: jen při hráčově detekci

## 3.5 GPS šipka (objective navigation)

```csharp
public class GPSArrow : MonoBehaviour {
    public Transform CurrentTarget;
    public Color FriendlyColor = Color.blue;
    
    void Update() {
        // Šipka ukazuje směr k aktuálnímu quest objective
        // NEDĚLÁ pathfinding přes alien zóny
        // Hráč si vybírá fyzickou trasu sám
    }
}
```

**Klíčový princip:** Šipka ukazuje **přímý směr** ke cíli, ne **doporučenou trasu**. Hráč si vybírá, jak fyzicky dorazit (přes les, kolem hor, atd.).

## 3.6 Mini-map UI

| Element | Pozice | Velikost |
|---|---|---|
| Mini-map | Top-right | 200×200 px |
| Player icon | Center | 16×16 px |
| GPS arrow | Center, rotující | 24×24 px |
| Heatmap overlay | Below player | 180×180 px |
| Compass | Top of mini-map | 200×20 px |

## 3.7 Full map (Tab/M)

| Element | Vlastnosti |
|---|---|
| **View** | Top-down ortho |
| **Zoom levels** | 1×, 2×, 4× |
| **Layers toggleable** | Hrozby, Tech, Heatmap, Patroly, Visibility, Terén |
| **Personal markers** | V2 (přidávat ručně) |
| **Photo log** | V2 (drone snímky) |
| **Time scrubber** | V2 (vidět minulost) |

---

# 4. ALIEN SENSE (Alien path mechanika)

## 4.1 Specifikace

| Atribut | Hodnota |
|---|---|
| `Range` | 30 m (Hybrid) — 60 m (Full alien) |
| `Detection through cover` | Ne (LineOfSight) |
| `Through walls` | Ne |
| `Activation` | Pasivní (vždy aktivní) |
| `Cooldown` | Žádný |
| `Battery` | Ne (alien augmentace) |

## 4.2 Vizuální feedback

- Pulsující červený kruh kolem alien postavy (v hráčově view)
- Audio cue: nízkofrekvenční bzučení, hlasitější s blízkostí
- HUD indicator: count alienů v range

## 4.3 Limitace

- **Puppety** mají signál maskovaný (alien implantát) — alien sense je **nedetekuje**
- **Static alien tech** detekováno, ale ne reálná hrozba
- **War Dogs** detekováni, ale bez směru

## 4.4 Vztah k Recon Drone

| | Drone | Alien Sense |
|---|---|---|
| Range | 100 m | 30-60 m |
| Active akce | Ano | Ne |
| Battery | Ano | Ne |
| Through cover | Ne | Ne |
| Path | Human | Alien |
| Map integration | Ano | Ano |
| Risk | Ztráta dronu | Žádný |
| Stealth | Tichý | Ne — alieni vědí, že je hráč alien-aligned |

**Klíčové:** Hybrid hráč může používat oboje, ale neefektivněji.

---

# 5. PUPPET / PUPPETEER MECHANIKA

## 5.1 Telepatic link

```csharp
public class PuppetController : MonoBehaviour {
    public PuppeteerController ControllingPuppeteer;
    public float MaxLinkDistance = 30f;
    
    void Update() {
        if (ControllingPuppeteer == null || ControllingPuppeteer.IsDead) {
            Collapse();  // Animace shutdown, ragdoll
        }
        else if (Vector3.Distance(transform.position, ControllingPuppeteer.transform.position) > MaxLinkDistance) {
            // Link se přerušil (vzácně, ale možné)
            EnterDormantState();  // Puppet zamrzne, pak Collapse za 5s
        }
    }
    
    void Collapse() {
        // Instant shutdown
        // Ragdoll aktivace
        // Implant deactivation VFX
    }
}
```

## 5.2 Puppeteer chování

```csharp
public class PuppeteerController : MonoBehaviour {
    public List<PuppetController> ControlledPuppets;
    public float DetectionFleeRadius = 40f;
    
    void Update() {
        if (PlayerInDetectionRadius()) {
            // Puppeteer utíká, schovává se za kryt
            // Puppety pokračují v útoku
            FleeBehavior();
        }
    }
}
```

## 5.3 Identifikace Puppeteera

Vizuální markery (musí být **viditelné z dálky**):
- Ovládací zařízení (helmet/headpiece)
- Antény
- Energetické pulzy mezi Puppeteerem a Puppety (lehce viditelné)
- Vyšší alien (pokud lidský), nebo ne-bojový vzhled

## 5.4 Spawn pravidla

V1 encounter:
- 1 Puppeteer = 2-3 Puppety
- Puppeteer **vždy** v rozsahu 30m od svých Puppetů
- Puppeteer prefer kryt, vyšší pozice
- Pokud Puppeteer zemře, Puppety okamžitě umírají

---

# 6. SOUND DETECTION SYSTEM

## 6.1 Sound emission

```csharp
public class SoundEmitter {
    public static void Emit(Vector3 position, float radius, SoundType type) {
        // Volá AlienAI v range, simuluje hluk
    }
}

public enum SoundType {
    Footstep,       // 5-15m podle povrchu
    Sprint,         // 10-25m podle povrchu
    GunshotPistol,  // 50m
    GunshotRifle,   // 80m
    Crossbow,       // 5m (téměř tichý)
    Explosion,      // 100m
    DroneActive,    // 10m
    GlassBreak,     // 30m
}
```

## 6.2 Surface modifier table

| Povrch | Footstep multiplier | Sprint multiplier |
|---|---|---|
| Tráva | 0.3× | 0.7× |
| Beton | 1.0× (base) | 1.5× |
| Kov | 1.5× | 2.0× |
| Voda | 2.0× | 3.0× |
| Listí | 1.2× | 1.8× |
| Krčení | 0.5× per povrch | — (sprint nemožný) |

## 6.3 Alien response k hluku

```csharp
public class AlienAI : MonoBehaviour {
    void OnSoundHeard(Vector3 sourcePosition, SoundType type) {
        if (DistanceTo(sourcePosition) < HearingRange) {
            // Investigate behavior
            // Alert other aliens in range
        }
    }
}
```

---

# 7. INVASION % SYSTEM

## 7.1 Data model

```csharp
public class InvasionState {
    public float GlobalInvasion;          // 0-100
    public Dictionary<string, float> RegionalInvasion;  // "Forest", "City", "Module"
    public float PlayerAlignment;          // -100 (Alien) to +100 (Human)
}
```

## 7.2 Quest impact tabulka

| Akce | Lokální delta | Globální delta |
|---|---|---|
| Zničit Puppeteera | -2% | -0.5% |
| Zničit alien tech | -5% | -1% |
| Quest splněný (minor) | -3% | -0.5% |
| Quest splněný (major) | -10% | -3% |
| Civilista zachráněn | -1% | -0.2% |
| Civilista zabit | +2% | +0.5% |
| Modul lodi zničen | -25% | -8% |

## 7.3 Idle progression

```csharp
const float IDLE_INVASION_GROWTH_PER_HOUR = 0.5f;
// Pasivní růst, pokud hráč nic nedělá
```

**⚠ TBD:** Konkrétní balancing — bude se iterovat.

---

# 8. INVENTORY SYSTEM

## 8.1 Dual limit system

```csharp
public class Inventory {
    public float MaxWeightKg;       // Roste se Strength stat
    public int MaxSlots;             // Fixní (nezávisí na Strength)
    public List<Item> Items;
}

public class Item {
    public float WeightKg;
    public int SlotsOccupied;        // Většinou 1, granáty 1, brnění 2-3
    public ItemCategory Category;
}
```

## 8.2 Item examples

| Item | Weight | Slots |
|---|---|---|
| Pistol + ammo | 1.5 kg | 1 |
| Rifle + ammo | 4.0 kg | 2 |
| Crossbow + arrows | 2.5 kg | 1 |
| Recon Drone Tier 1 | 1.0 kg | 1 |
| Medkit (light) | 0.3 kg | 1 |
| Medkit (heavy) | 0.8 kg | 1 |
| Frag Grenade | 0.5 kg | 1 |
| Alien Crystal | 0.2 kg | 1 |
| Alien Material (scrap) | 1.0 kg | 1 |

## 8.3 Strength → MaxWeight mapping

| Strength Level | Max Weight |
|---|---|
| 1 (start) | 25 kg |
| 2 | 30 kg |
| 3 | 35 kg |
| 4 | 40 kg |
| 5 (cap V1) | 45 kg |

Slots fixní: **15 slotů** (V1 vertical slice).

---

# 9. SAVE/LOAD SYSTEM

## 9.1 Free save mechanika

```csharp
public class SaveManager : MonoBehaviour {
    public void SaveGame(string slotName) {
        var data = new SaveData {
            PlayerPosition = Player.transform.position,
            PlayerHP = Player.HP,
            Inventory = Player.Inventory,
            QuestProgress = QuestSystem.GetState(),
            InvasionState = WorldState.Invasion,
            MapDataPoints = MapSystem.GetAllDataPoints(),
            EnemyStates = EnemyManager.SerializeAll(),
            // ...
        };
        SerializeToJSON(data, slotName);
    }
}
```

## 9.2 Co se ukládá

✅ Hráčova pozice, HP, alignment, alien implant status
✅ Inventář (items + weight)
✅ Quest progress (active, completed, branches)
✅ Invasion %, lokální invasion per region
✅ Mapa data (drone scout, decay timestamps)
✅ Stav nepřátel (kdo zemřel, kdo patroluje kde)
✅ NPC dialogue history, faction relationships

## 9.3 Load behavior

- Plná restore z JSON
- Mapa decay continues od save timestampu
- Quest stavy obnoveny
- Invasion % obnoven

---

# 10. PERFORMANCE TARGETS

## 10.1 V1 minimální spec

**⚠ TBD finální:**
- CPU: i5-8400 nebo lepší
- GPU: GTX 1660 nebo RTX 2060 (HDRP náročnost)
- RAM: 16 GB
- Storage: 30 GB SSD

## 10.2 V1 cíle (in-game)

| Metrika | Cíl |
|---|---|
| FPS (1080p, medium) | 60+ |
| FPS (1440p, high) | 60+ |
| Loading time | <30 s |
| Map streaming | seamless (additive scenes) |

## 10.3 Optimalizace

- LOD groups per asset
- Object pooling pro projektily, particles, ragdolly
- Occlusion culling
- HDRP Volume profily per zóna

---

# 11. UNITY ARCHITECTURE

## 11.1 Folder structure

```
Assets/
├── _Project/
│   ├── Scripts/
│   │   ├── Combat/
│   │   ├── AI/
│   │   ├── Drone/
│   │   ├── Map/
│   │   ├── Inventory/
│   │   ├── Quest/
│   │   └── World/
│   ├── Prefabs/
│   │   ├── Enemies/
│   │   ├── Player/
│   │   ├── Drone/
│   │   └── Items/
│   ├── Materials/
│   ├── Scenes/
│   ├── ScriptableObjects/
│   │   ├── EnemyData/
│   │   ├── WeaponData/
│   │   ├── QuestData/
│   │   └── DroneData/
│   └── UI/
├── ThirdParty/
│   ├── HDRP/
│   ├── ProBuilder/
│   ├── DreamTree2/
│   └── ...
└── Plugins/
```

## 11.2 ScriptableObject pattern

Data-driven design pro:
- `EnemyData` (per typ nepřítele)
- `WeaponData` (per zbraň)
- `QuestData` (per quest)
- `DroneData` (per drone tier)
- `RuleData` (V2 — pro game bible)

## 11.3 Event Bus (decoupled communication)

```csharp
public class GameEventBus : ScriptableObject {
    public UnityEvent<float> OnInvasionChanged;
    public UnityEvent<int> OnAlignmentChanged;
    public UnityEvent<QuestData> OnQuestCompleted;
    public UnityEvent<EnemyType> OnEnemyKilled;
    // ...
}
```

---

# 12. BALANCING TABULKY (V1)

## 12.1 Encounter difficulty progression

| Region | Avg alien count | Avg patrol density | Loot quality |
|---|---|---|---|
| Forest (start) | 1-2 | Low | Basic |
| Forest edge | 2-3 | Medium | Mixed |
| City outskirts | 3-4 | High | Mixed |
| City center | 4-6 | Very high | Advanced |
| Module zone | 6-8 + Puppeteer | Extreme | Alien tech |

## 12.2 Player power curve

| Hodina hry | Strength | Skills | Gear |
|---|---|---|---|
| 1 | 1 | 0 | Pistol, basic |
| 2 | 1-2 | 1-2 | + Crossbow, drone |
| 3 | 2 | 2-3 | + Better weapons |
| 4 | 2-3 | 3-4 | + Crafting unlocked |
| 5+ | 3 | 4-5 | + Alien tech (path dependent) |

## 12.3 Resource scarcity

| Resource | Frekvence dropu | V1 quantity |
|---|---|---|
| Pistol ammo | Frequent | Plenty |
| Rifle ammo | Less frequent | Limited |
| Crossbow arrows | Recoverable | 5-10 max |
| Medkit (light) | Common | 2-3 in inventory |
| Medkit (heavy) | Rare | 1 in inventory |
| Alien crystal | Rare (drop from elite) | For crafting |
| Alien scrap | Common (Puppets, drones) | For crafting |

---

# 13. ITERAČNÍ PLÁN

## 13.1 V1 → V2 expanze

| Systém | V1 | V2 |
|---|---|---|
| Drone | Tier 1 | + Tier 2 features |
| Map | Mini + Full | + Personal markers, Photo log |
| AI | Patrol/Attack/Flee/Cover | + Sound investigation, coordinated |
| Healing | Medkit basic | + Trauma states, station heal |
| Time | Static | + Day/Night cycle |
| Weather | Static | + Rain/Fog effects |
| Crafting | Knowledge gate basic | + Workbench progression |

## 13.2 Balancing iterace

Po každém greybox playtestu:
1. Zaznamenej všechny smrti hráče (důvod, lokace, čas)
2. Zaznamenej completion time per encounter
3. Subjective rating (frustrace 1-10, fun 1-10)
4. Iteruj damage tabulky, decay timery, alien spawns

**Cílové metriky V1:**
- Smrti per quest: 1-3 (lethality, ne frustrace)
- Quest completion time: 30-45 min
- Drone usage rate: >70%
- Stealth vs combat: 40-60% / 60-40%

---

Tech Spec v0.1 — Siege of the Blue World

*Aktualizovat po každé větší designové změně nebo po playtestu.*

*Související dokumenty: PRD (specifikace), Game Bible (pravidla), Workflow Checklist (postup).*
