**SIEGE OF THE BLUE WORLD**

Rozšířený seznam assetů pro Unity — V1 Vertical Slice

🆕 = nová položka přidaná v rozšíření oproti původnímu dokumentu

## LEGENDA PRIORIT

| **Kritické** | Bez toho nefunguje V1 |
|---|---|
| **Vysoká** | Potřeba před playtestem |
| **Střední** | Pro final pass |
| **🆕 Kritické / Vysoká / Střední** | Nová položka přidaná v rozšíření |

# ČÁST 3 — ROZŠÍŘENÝ SEZNAM ASSETS PRO UNITY

Tento seznam rozšiřuje původní dokument o chybějící kategorie. Položky označené 🆕 jsou nové.

## UNITY PACKAGE MANAGER (nové — instalovat přes Package Manager)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 Unity Package** | ✅ HDRP (High Definition RP) | Nainstalovat jako první — základ celého projektu | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ Input System (com.unity.inputsystem) | Vyžaduje UCAF fáze B — simulace vstupu hráče | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ Cinemachine | TPS kamera, follow, aim assist — zdarma od Unity | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ Visual Effect Graph (VFX Graph) | HDRP particles — výstřely, exploze, alien efekty | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ TextMeshPro | Veškerý text ve hře — povinnost pro UI | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ ProBuilder | 3D blockout přímo v Unity — les, město, modul lodi | **🆕 Vysoká** |
| **🆕 Unity Package** | ✅ NavMesh Components (AI Navigation) | NavMesh pro 4 typy nepřátel — různé rychlosti | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ Unity UI (uGUI) | HUD, inventory, quest log, crafting menu | **🆕 Kritické** |
| **🆕 Unity Package** | ✅ Addressables | Streaming assetů pro open world — additive scenes | **🆕 Vysoká** |
| **🆕 Unity Package** | VFX Graph Samples (official) | 30+ hotových HDRP efektů — výstřely, oheň, energie | **🆕 Vysoká** |

## 3D — PROSTŘEDÍ

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| 3D — Prostředí | ✅ Les: stromy, keře, kameny, tráva, cesta | Blockout postačí pro F1–F3 | **Kritické** |
| 3D — Prostředí | ✅ Město: budovy (fasády + interiér klíčových), ulice, chodník, auta | Modulární sada pro opakování | **Kritické** |
| 3D — Prostředí | ✅ Modul vesmírné lodi: exteriér + vstup | Unikátní asset, klíčový cíl V1 | **Kritické** |
| 3D — Prostředí | Destruktivní objekty: zeď, plot, bedna, auto | Potřeba pro destrukci prostředí | **Vysoká** |
| 3D — Prostředí | POI dekorace: stany, barikády, vraky, mimozemský harampádí | Věrohodnost světa | **Střední** |
| 3D — Prostředí | Fotbalový stadion (exteriér + tribuna) | Klíčový pro humorous event | **Vysoká** |

## 3D — PROSTŘEDÍ (rozšíření)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 3D — Prostředí** | HDRP materiály: tráva, asfalt, beton, kov, půda | Základní PBR materiály pro terén a město — HDRP specifické | **🆕 Kritické** |
| **🆕 3D — Prostředí** | Decals: stopy, trhliny, krev, alien fluid na površích | HDRP Decal Projector — detail bez extra geometrie | **🆕 Vysoká** |
| **🆕 3D — Prostředí** | Interiér modulu lodi: chodby, panely, terminály | Pro quest uvnitř modulu — nelze jen exteriér | **🆕 Vysoká** |
| **🆕 3D — Prostředí** | ✅ Zbraně: pistole, puška, alien energetická zbraň (3D model) | Viditelné v ruce hráče v TPS pohledu | **🆕 Kritické** |
| **🆕 3D — Prostředí** | Loot předměty: krystaly, součástky, lékárničky (3D model) | Viditelné ve světě před sebráním | **🆕 Střední** |

## 3D — POSTAVY

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| 3D — Postava | Hrdina: plný 3D model s rigem | Viditelný v TPS kameře (ruce, tělo) | **Kritické** |
| 3D — Postava | Mimozemšťan: plný model s rigem | 1 typ vizuálu, variace sílou/technologií | **Kritické** |
| 3D — Postava | Mimozemský War Dog: model s rigem | Čtyřnohý, agresivní tvar | **Kritické** |
| 3D — Postava | Dron (zotročený člověk): model s mimozemskými modifikacemi | Viditelné implantáty/tech na těle | **Kritické** |
| 3D — Postava | ✅ NPC civilista: 2–3 varianty modelu | Placeholder pro F1–F3 | **Vysoká** |
| 3D — Postava | Operátor dronu: mimozemšťan s ovládacím zařízením | Vizuálně odlišitelný od standardního | **Vysoká** |

## ANIMACE

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Animace | Hrdina: idle, chůze, sprint, krčení, střelba, melee, smrt | Priorita: základní pohybový set — Mixamo | **Kritické** |
| Animace | Mimozemšťan: idle, patrol, útok ranged, útok melee, smrt, flee | Ragdoll overlay přes death anim. | **Kritické** |
| Animace | War Dog: idle, sprint, útok, smrt | Čtyřnohý locomotion | **Kritické** |
| Animace | Dron: idle, útok, smrt (instantní při kill operátora) | Kolaps/shutdown animace | **Vysoká** |
| Animace | NPC: idle, strach, útěk | Minimální sada | **Vysoká** |
| Animace | Zničení modulu lodi: destrukční sekvence (finální cíl V1) | Může být částečně VFX | **Kritické** |

## ANIMACE (rozšíření)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 Animace** | Hrdina: reload, aim, cover (krytí) | Doplněk základního setu — TPS nutnost | **🆕 Vysoká** |
| **🆕 Animace** | Hrdina: alien augmentace vizuál (glow ruce, změna postury) | Progression feedback — alien větev | **🆕 Střední** |
| **🆕 Animace** | Animator Controller setup per postava | Blend tree pro locomotion — chůze/sprint plynule | **🆕 Kritické** |

## VFX — VIZUÁLNÍ EFEKTY

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| VFX | Výstřel: nábojnice, záblesk, kouř | Per typ zbraně (lidská vs. alien) | **Kritické** |
| VFX | Zásah: krev/alien fluid, jiskry, střepiny | Dle materiálu (maso vs. kov vs. alien) | **Kritické** |
| VFX | Exploze: obecná + alien varianta | Pro granáty a destrukci | **Vysoká** |
| VFX | Destrukce objektu: prach, debris, zlomení | Napojeno na destroy trigger | **Vysoká** |
| VFX | Invasion vizuál: alien záře, hologramy, energie | Ambient VFX pro modul a výsadek | **Střední** |
| VFX | Crafting UI efekt: animace výroby/upgradu | Malý, ale důležitý feedback | **Střední** |
| VFX | Ragdoll aktivace: žádný speciální VFX, jen fyzika | Řešeno Unity ragdoll systémem | **Kritické** |

## VFX (rozšíření)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 VFX** | HDRP Volume profily: den/noc, invasion mood, interiér lodi | Post-processing per zóna — klíčové pro atmosféru | **🆕 Kritické** |
| **🆕 VFX** | Volumetrická mlha (Fog Volume) | HDRP nativní — les a modul lodi atmosféra | **🆕 Vysoká** |
| **🆕 VFX** | Alien augmentace efekt na hrdinovi: pulsující implantáty | Vizuální feedback progression — alien větev | **🆕 Střední** |
| **🆕 VFX** | Spawn efekt nepřátel: teleport/materialization | Invasion event vizuál — kdy přicházejí mimozemšťané | **🆕 Střední** |

## ZVUK — SFX

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Zvuk — SFX | ✅ Zbraně: výstřel, přebíjení, prázdný zásobník (lidská zbraň) | WAV/OGG, prostorový 3D zvuk | **Kritické** |
| Zvuk — SFX | ✅ Zbraně: alien energetická zbraň (odlišná od lidské) | Sci-fi tón | **Kritické** |
| Zvuk — SFX | ✅ Kroky: dle povrchu (tráva, beton, kov, loď) | Footstep systém | **Vysoká** |
| Zvuk — SFX | ✅ Nepřátelé: zvuky útoku, bolesti, detekce, smrt | Per typ nepřítele | **Vysoká** |
| Zvuk — SFX | ✅ Prostředí: vítr v lese, ruch města, hum modulu lodi | Ambient loop | **Střední** |
| Zvuk — SFX | ✅ UI zvuky: klik, quest splněn, inventory, save | Drobné ale důležité | **Střední** |

## ZVUK (rozšíření)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 Zvuk — SFX** | ✅ Destrukce prostředí: lámání, prach, řinčení kovů | Per typ materiálu (beton, kov, dřevo) | **🆕 Vysoká** |
| **🆕 Zvuk — SFX** | ✅ Ragdoll dopad: tělo padá na různé povrchy | Fyzický feedback — důležité pro TPS | **🆕 Vysoká** |
| **🆕 Zvuk — SFX** | Dialog: hlas hrdiny + NPC (placeholder nebo generovaný) | I placeholder hlas lepší než ticho | **🆕 Střední** |

## ZVUK — HUDBA

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Zvuk — Hudba | Combat hudba: napjatý rytmický loop | Adaptive audio — spouštěno bojem | **Vysoká** |
| Zvuk — Hudba | Ambient hudba: temný, atmosférický ambient | Smyčka pro průzkum | **Střední** |
| Zvuk — Hudba | Victory/fail moment: krátký stinger | Konec V1 sekvence | **Střední** |

## UI / UX

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| UI / UX | HUD: health bar, invasion %, alignment indikátor | Dynamicky mění styl dle alignmentu | **Kritické** |
| UI / UX | Inventory screen: mřížka slotů + hmotnostní bar | Přístupné z pause menu | **Kritické** |
| UI / UX | Quest log: aktivní questy + cíle | Jednoduchý list, bez mapy v V1 | **Kritické** |
| UI / UX | Crafting menu: seznam receptů + podmínky | Zobrazit co chybí / co je splněno | **Vysoká** |
| UI / UX | Skill tree screen: obě větve (lidská / alien) | Vizuálně odlišné větve | **Vysoká** |
| UI / UX | Pause menu: resume, save, load, quit | Standardní, minimalistické | **Kritické** |
| UI / UX | ✅ Ikony: zbraně, loot typy, quest markery | Jednoduchá piktogramová sada | **Vysoká** |
| UI / UX | Font: sci-fi nebo minimalistický bezpatkový | Konzistentní s tónem hry | **Střední** |

## UI / UX (rozšíření)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 UI / UX** | Dialog UI: text box, jméno mluvčího, portrait | NPC interakce — quest dialogy | **🆕 Kritické** |
| **🆕 UI / UX** | Minimap nebo compass | Orientace v regionu — quest markery | **🆕 Vysoká** |
| **🆕 UI / UX** | Damage number popup (floating text) | Feedback zásahu — čitelnost boje | **🆕 Střední** |
| **🆕 UI / UX** | Notifikace / toast system | Quest update, loot pickup, invasion % change | **🆕 Střední** |
| **🆕 UI / UX** | Crosshair system: různý dle zbraně a aiming módu | TPS shooter nutnost | **🆕 Vysoká** |
| **🆕 UI / UX** | Loading screen / scene transition | Přechod mezi zónami při additive scene streamingu | **🆕 Vysoká** |

## UNITY SCRIPT / SYSTÉM

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| Unity Script / Systém | Scriptable Objects: nepřátelé, loot, questy, recepty | Data-driven základ celé hry | **Kritické** |
| Unity Script / Systém | NavMesh Agent konfigurace per typ nepřítele | Různé rychlosti, poloměry | **Kritické** |
| Unity Script / Systém | Ragdoll prefab setup (rigidbody + collider na každé kosti) | Per model postavy | **Kritické** |
| Unity Script / Systém | Destruction prefab: verze před a po zničení + debris | Per destruktivní objekt | **Vysoká** |
| Unity Script / Systém | Adaptive music manager: detekce combat stavu | Jednoduchý state machine | **Střední** |
| Unity Script / Systém | Invasion event trigger: spawnování událostí dle invasion % | Např. fotbalový stadion event | **Vysoká** |

## UNITY SCRIPT / SYSTÉM (rozšíření)

| **Kategorie** | **Asset / Položka** | **Poznámka** | **Priorita** |
|---|---|---|---|
| **🆕 Unity Script / Systém** | Object Pool Manager | Spawn/despawn nepřátel a projektilů bez GC spike | **🆕 Kritické** |
| **🆕 Unity Script / Systém** | Footstep Manager: materiál pod nohama → zvuk | Raycast pod hráče, swap audio clip dle surface tag | **🆕 Vysoká** |
| **🆕 Unity Script / Systém** | Dialog systém: trigger → NPC linka → response volby | I jednoduchý systém stačí pro V1 questy | **🆕 Kritické** |
| **🆕 Unity Script / Systém** | Audio Mixer setup: Master / SFX / Music / Voice | Hlasitost per kategorie — settings menu | **🆕 Vysoká** |
| **🆕 Unity Script / Systém** | LOD Group konfigurace per asset | Výkon na dálku — kritické pro open world | **🆕 Kritické** |
| **🆕 Unity Script / Systém** | Scene streaming controller (Additive Scenes) | Load/unload regionů při pohybu hráče | **🆕 Vysoká** |
| **🆕 Unity Script / Systém** | Camera shake systém | Exploze, zásah hráče — feedback boje | **🆕 Střední** |
| **🆕 Unity Script / Systém** | Interakční systém (E klávesa): loot, NPC, terminál | Proximity trigger + UI prompt | **🆕 Kritické** |
| **🆕 Unity Script / Systém** | Alignment systém (Human/Hybrid/Alien tracker) | Backend pro invasion system + UI transformaci | **🆕 Kritické** |
| **🆕 Unity Script / Systém** | Game Event Bus (ScriptableObject events) | Decoupled komunikace — invasion %, quest update, death | **🆕 Vysoká** |

---

Siege of the Blue World — Interní dokument týmu | V1 Vertical Slice | Aktualizovat při každé změně scope.
