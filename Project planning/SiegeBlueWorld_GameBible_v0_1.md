**SIEGE OF THE BLUE WORLD**

GAME BIBLE — Pravidla přežití

Verze: 0.1 — V1 Vertical Slice

*Living document — rozšiřuje se v V2/V3 o nová pravidla.*

---

## ÚVOD

Tento dokument obsahuje **pravidla světa**, podle kterých hra funguje. Pravidla jsou **zdravý rozum** — ne arbitrární game mechaniky. Hráč, který je pochopí a dodržuje, **přežije**. Hráč, který je ignoruje, **zaplatí**.

### Vztah k PRD

PRD definuje **CO hra je** (vize, mechaniky, systémy). Game Bible definuje **JAK svět funguje** (pravidla, která platí pro hráče i AI).

### Princip rules-based designu

Viz **PRD sekce 2.5**. Hra:
- Dává hráči **informace** a **nástroje**
- Hráč si **vybírá strategii**
- **Konzistentní pravidla** napříč celou hrou
- **Žádné výjimky** pro narativní účely
- Pravidla platí pro hráče **i AI**

### Designerova zodpovědnost

Před přidáním obsahu do hry (quest, nepřítel, item) projdi tyto otázky:

1. **Dodržuje toto pravidla?**
2. **Zavádí nová pravidla?**
3. **Pokud ano, nejsou v rozporu se starými?**

Pokud chceš výjimku, **nevytvoř výjimku** — vytvoř **nové pravidlo**, které ji vysvětluje. Pravidlo platí všude, ne jen v jednom questu.

---

# ČÁST 1 — JAK PRAVIDLA UČIT HRÁČE

Hráč nemá v hře tutorial v klasickém smyslu. Pravidla **objevuje** přes:

| Metoda | Příklad |
|---|---|
| **Telegraphing** | Alien má svítící oči — vidíš ho dřív, než tě uvidí |
| **Show, don't tell** | NPC udělá hluk, alien ho zabije — naučíš se hluk = smrt |
| **Soft lessons** | První porušení = zranění (varování). Druhé = smrt (trest) |
| **Environmental storytelling** | Mrtvá těla v určité pozici ukazují, co je zabilo |
| **Dialog NPC** | "Nikdy nechoď do města za soumraku" — NPC řekne pravidlo |
| **Dokumenty / lore** | Deníky, vojenské manuály, alien dešifrování |
| **Soft tutorial v základně** | První hodina = bezpečné prostředí na experimenty |

**Zlaté pravidlo:** Hráč by měl pravidlo **objevit sám**, ne ho **přečíst v menu**. Objev = engagement. Menu = nuda.

### Pacing učení

```
HODINA 1 (les, blízko základny):
- Pasivní pravidla (telegraphing, environmental storytelling)
- Žádná smrt hráče
- Hra ukazuje pravidla na NPC

HODINA 2-3 (les → okraj města):
- První skutečné encountery
- Hráč už zná 60 % pravidel
- První smrti, ale s jasným feedbackem

HODINA 4+ (město, modul):
- Plná lethality
- Hráč zná 90 % pravidel
- Hra ho odměňuje za kreativní použití
```

---

# ČÁST 2 — KATEGORIZACE PRAVIDEL

Pravidla jsou rozdělena do 10 kategorií podle účelu:

1. **Recon a informace** (před akcí)
2. **Engagement decision** (kdy bojovat, kdy ne)
3. **Movement a positioning** (jak se pohybovat)
4. **Combat technique** (jak střílet)
5. **Damage management** (zranění a healing)
6. **Resource management** (jak dlouho vydržet)
7. **Environmental awareness** (svět kolem)
8. **Social a faction rules** (lidé, nejen alieny)
9. **Time management** (kdy co dělat)
10. **Mental discipline** (chování hráče)

Plus speciální kategorie:
- **Alien-specific rules** (vázané na lore světa)
- **Player path rules** (Human / Hybrid / Alien-aligned)

---

# ČÁST 3 — V1 CORE PRAVIDLA (15 pravidel)

Pro V1 vertical slice cílíme na **15 fundamentálních pravidel**. V V2/V3 se rozšíří.

## 3.1 DETEKCE (4 pravidla)

### Pravidlo 1: Line-of-sight detection
**Definice:** Alien tě vidí, pokud nejsi za krytem, ve tmě, nebo mimo jeho vizuální kuželu.

**Detail:**
- Alien má vizuální range **~50 m** za optimálních podmínek
- Tma redukuje na **~20 m**
- Vegetace (les) ho ruší na **~30 m**
- Plný kryt = neviditelný

**Jak hráče naučit:**
- První hodinu: vidíš NPC schovat se za zeď, alien ho přehlédne
- Vizuální feedback: alien má vision cone (volitelně viditelný v drone view)

**Platí pro AI?** ✅ Ano — alieny také můžeš oklamat krytem.

---

### Pravidlo 2: Hluk přitahuje
**Definice:** Výstřel, sprint, nebo exploze přitahují aliény v okruhu **~50 m**.

**Detail:**
- Pistole / puška = hlasitý výstřel
- Kuše / luk = tichý
- Sprint po betonu = středně hlasitý
- Sprint po trávě = tichý
- Exploze = velmi hlasitá (~100 m)

**Jak hráče naučit:**
- První hodinu: NPC vystřelí, vidíš aliény přijít
- Audio feedback: hlasitější zvuk = výraznější UI ripple

**Platí pro AI?** ✅ Ano — aliené slyší ostatní aliény, mohou si volat posily.

---

### Pravidlo 3: Krev = stopa
**Definice:** Pokud jsi zraněn (critical injury), zanecháváš krvavé stopy, které alieni mohou sledovat.

**Detail:**
- Lehké zranění = minimální stopa
- Critical injury = výrazná stopa (vidět ~10 minut)
- Pokud se ošetříš, krvácení přestane
- Aliené stopu vidí, war dogs ji **cítí** (i delší)

**Jak hráče naučit:**
- První critical injury: hráč vidí krev za sebou
- NPC v dialogu: "Ošetři se rychle, nebo tě najdou."

**Platí pro AI?** Částečně — alieni mohou zanechat alien fluid, ale hráč ho běžně neexplotuje.

---

### Pravidlo 4: Alien Sense (Recon Drone alternative)
**Definice:** Hráč může detekovat alieny **dvěma způsoby**:
1. **Recon Drone** — vidí na ~100m, pasivní scouting
2. **Alien sense** (alien-aligned path) — pasivní zvuk + vizuál v ~30m

Bez ani jednoho je hráč **slepý** vůči alienům za rohem.

**Detail:**
- Drone: human path, vyžaduje aktivní akci
- Alien sense: alien path, pasivní, ale vyžaduje alignment shift
- Hybrid: oboje funguje, ale slabší

**Jak hráče naučit:**
- Hráč najde drona v opuštěné armádní základně (Quest 1 reward)
- Alien sense: pokud vezme alien implantát z mrtvého Puppeta, perk se odemkne

**Platí pro AI?** Aliené mají vlastní detekci skrze línie a alien sense (operátoři).

---

## 3.2 COMBAT (4 pravidla)

### Pravidlo 5: Lethality
**Definice:** Hlavní zásah = smrt. Body shot = critical injury. Limb shot = movement penalty + bleeding.

**Detail:** Viz **PRD sekce 5.1** a **Tech Spec damage tabulky**.

**Klíčové:**
- Hráč i nepřítel jsou křehcí
- Žádné HP-houba combat
- Critical injury **nutí ústup**, není to "nižší HP"

**Jak hráče naučit:**
- První combat: NPC dostane body shot, vidíš, jak se stáhne
- Hráčova první smrt = jasný feedback ("byl to headshot zezadu")

**Platí pro AI?** ✅ Ano — alieni také padnou na headshot. **Žádné výjimky.**

---

### Pravidlo 6: Kryt = pravidlo č. 1 v boji
**Definice:** Pokud nejsi za krytem, jsi mrtvý. Kryt blokuje line-of-sight a zastavuje projektily.

**Detail:**
- Pevný kryt (zeď, beton) = plná ochrana
- Lehký kryt (auto, dřevo) = částečná, brzy se zničí (destrukce prostředí)
- Kryt nad hlavou = chrání před útoky shora
- Kryt nikdy nevyrve hráče z combat — musí aktivně pokračovat

**Jak hráče naučit:**
- První combat: hráč bez krytu = critical injury rychle
- Hráč za krytem = může střílet bezpečně

**Platí pro AI?** ✅ Ano — aliené také používají kryt (V1+ AI vrstva).

---

### Pravidlo 7: Headshot priority
**Definice:** Mířit na horní část těla — hlava = instant kill. Body = critical (nepřítel se může bránit).

**Detail:**
- Headshot = vyřazení nepřítele (nikdo nevystřelí dále)
- Body shot = nepřítel je critical, ale **může pokračovat ve střelbě** ještě 5-10 sekund
- Brnění alien postav pokrývá hlavu — body shot je často efektivnější (záleží na typu)

**Jak hráče naučit:**
- První kill = headshot, hráč vidí ragdoll
- Body shot = nepřítel se sváží, ale stále nebezpečný

**Platí pro AI?** ✅ Ano — alieni také míří na hlavu hráče.

---

### Pravidlo 8: Reload v krytu
**Definice:** Reload mimo kryt = pravděpodobná smrt. Reload v krytu = bezpečné.

**Detail:**
- Reload animace trvá ~2-3 sekundy (zranitelný)
- Crossbow / pistole = rychlejší
- Puška = pomalejší
- Hráč nesmí reloadovat v open prostoru

**Jak hráče naučit:**
- První pokus o reload v open = critical injury
- Soft tutorial v základně: "Ucvakni do krytu, pak reload"

**Platí pro AI?** Částečně — alieni neresetují klasicky, ale mají cooldown na alien zbraně.

---

## 3.3 MOVEMENT (2 pravidla)

### Pravidlo 9: Pohybuj se mezi kryty, ne přes otevřené prostory
**Definice:** Plánuj trasu z krytu do krytu. Nikdy nesprintuj přímo k cíli přes open.

**Detail:**
- Open prostor mezi krytí = riziko
- Klikatá trasa s pauzami za kryty = bezpečí
- Vysoké pozice (střecha, kopec) = výhoda + kryt současně
- Vyhni se siluetě proti obloze

**Jak hráče naučit:**
- Vidíš NPC sprintovat přes open = umírá
- Heatmap mapy ukazuje "open" oblasti vizuálně

**Platí pro AI?** ✅ Ano — alieni také plánují cesty přes kryty.

---

### Pravidlo 10: Zvuk kroků závisí na povrchu
**Definice:** Tráva = ticho. Beton = středně. Kov = hlasitě. Voda = nejhlasitější.

**Detail:**
- Detection range podle povrchu se mění
- Krčení redukuje hluk o ~50% per povrch
- Sprint zdvojnásobuje hluk

**Jak hráče naučit:**
- Audio feedback při různých površích
- Drone scouting: vidíš, jak alieni reagují na zvuk

**Platí pro AI?** ✅ Ano — pohyb alienů také generuje hluk.

---

## 3.4 DAMAGE MANAGEMENT (2 pravidla)

### Pravidlo 11: Zraněný = stáhni se
**Definice:** Critical injury = okamžitý ústup. Quest počká, život ne.

**Detail:**
- Critical injury = ~60s do smrti bez ošetření
- Pokus o pokračování v boji = pravděpodobná smrt
- Návrat do krytu, ošetření, pak rozhodnutí (pokračovat / vrátit)

**Jak hráče naučit:**
- První critical: UI varování "OŠETŘI SE NEBO ZEMŘEŠ"
- NPC v lore: deník vojáka, který zemřel pokračováním v boji se zraněním

**Platí pro AI?** Částečně — aliené ustupují při nízkém HP (flee state V1).

---

### Pravidlo 12: Ošetření v krytu, ne na otevřeném prostoru
**Definice:** Medkit animace trvá ~5 sekund. Během ní jsi nehybný a viditelný.

**Detail:**
- 5s zranitelnost
- Bez krytu = pravděpodobná smrt během ošetření
- Pokud máš víc medkitů, postupné ošetřování (každý zlepšuje stav)
- Vážné zranění může vyžadovat **návrat na základnu** (V2)

**Jak hráče naučit:**
- První pokus o ošetření v open = útok, smrt
- Soft lesson: "Najdi kryt, pak medkit"

**Platí pro AI?** Aliené nemají medkit, ale aliený War Dog může ošetřit jiného aliena (V2/V3).

---

## 3.5 RESOURCE MANAGEMENT (1 pravidlo)

### Pravidlo 13: Munice je vzácná, šetři ji
**Definice:** Žádné varovné výstřely. 1 výstřel = 1 zabití.

**Detail:**
- Lidská munice: relativně dostupná, ale ne nekonečná
- Alien munice: vzácná, vyžaduje crafting
- Crossbow: šípy lze sbírat zpět (re-usable)
- Granáty: extrémně omezené

**Jak hráče naučit:**
- První loot encounter: málo munice
- NPC obchodník: drahé ceny

**Platí pro AI?** Aliené mají energetické zbraně s rechargováním, ne munici.

---

## 3.6 SPECIFIC RULES (2 pravidla)

### Pravidlo 14: Puppeteer = klíčový cíl, ne Puppety
**Definice:** Pokud zabiješ Puppeteera, všichni jeho Puppety okamžitě umírají. Útok na Puppety bez vyřazení Puppeteera = nekonečný boj.

**Detail:**
- Puppeteer stojí ~30m za scénou
- Vizuálně odlišný (ovládací zařízení, antény)
- Headshot zezadu = stealth eliminace celé skupiny
- Puppeteer mizí, jakmile spatří hráče (utíká)

**Jak hráče naučit:**
- První Puppet encounter: vidíš, jak Puppet kolabuje, když Puppeteer zemře
- Lore deník: "Puppet bez Puppeteera je jen tělo."

**Platí pro AI?** Aliené chrání Puppeteery prioritně.

---

### Pravidlo 15: EMP zóny deaktivují elektroniku
**Definice:** Modul lodi a alien struktury vysílají EMP pulzy v okolí ~30-50m. Hráčova elektronika (drone, gadgety, hackovací zařízení) v této zóně **selhává**.

**Detail:**
- Drone v EMP zóně = sestřelí se
- Mapa v EMP zóně = nelze updatovat
- Granáty fungují (mechanické)
- Crossbow funguje (mechanická)

**Jak hráče naučit:**
- První pokus o drone v EMP = ztráta dronu
- Vizuální indikátor: EMP zóna na mapě (permanentní červená)

**Platí pro AI?** Alien tech v EMP zóně funguje normálně (jiná frekvence).

---

# ČÁST 4 — ALIEN-SPECIFIC RULES

Tato pravidla vycházejí z fikce a nejsou v jiných hrách:

### A1: Alien sense funguje na biometriku
- Detekuje **lidskou biometriku** (tep, dech, mozková aktivita)
- **Puppety** mají signál maskovaný (alien implantát)
- **Alien-aligned hráč** s implantátem je hůř detekovatelný

### A2: Alieni komunikují telepaticky
- Pokud zabiješ aliena tak, že nemá čas vykřiknout (stealth headshot), ostatní se nedozví
- Pokud aliena zraníš, vyšle alarm signál okolním
- War dogs komunikují primitivně (zvuk)

### A3: Modul lodi je centrum
- Modul vysílá EMP pulzy
- Modul kontroluje Puppeteery v okolí
- Zničení modulu = osvobození všech Puppetů v regionu

### A4: Alien tělo má jiné slabé body
- Hlava ne nutně hlavní cíl (záleží na typu)
- War Dog: zranitelný na hrudi
- Alien voják: zranitelný v kloubech brnění
- Hráč se naučí experimentem nebo lore

---

# ČÁST 5 — PLAYER PATH RULES

### Human path
- Plný přístup k drone, gadgetům, hackování
- Slabší fyzická síla, ale **informační převaha**
- Lepší vztahy s NPC odporu
- Alien sense **nedostupný** (potřebuje implantát)

### Alien path
- Plný přístup k augmentacím (síla, rychlost, regenerace)
- Alien sense funguje od začátku
- Alien tech (zbraně, gadgety) využitelné
- NPC odporu **nedůvěřuje** — žádné lidské questy
- Některé encountery s alieny **neútočí** (tolerance)

### Hybrid path
- Slabší obě stránky, ale flexibilní
- Žádné z extrému, ale širší možnosti
- Některé questy zamčené pro oba extrémy
- "Nikdo ti nedůvěřuje plně"

---

# ČÁST 6 — ROZŠÍŘENÉ KATEGORIE (V2/V3 expansion)

Pro V1 nejsou součástí, ale připravujeme se na ně:

### Recon a informace (V2)
- Skautuj před útokem (drone)
- Naslouchej (audio detection)
- Mapuj stopy
- Pamatuj si patroly

### Engagement decision (V2)
- Číselná převaha = vyhni se
- Útok zezadu nebo z výšky
- Pokud nejsi 100% jistý zabitím, neútoč
- Vždy únikovou cestu

### Environmental awareness (V2/V3)
- Day vs. night
- Počasí (déšť, mlha)
- Vodní toky
- Vítr nese zvuk
- Oheň = signál

### Social rules (V2/V3)
- Civilisté = riziko
- Důvěra se buduje pomalu
- Kolaboranti tě sledují
- NPC dialog = informace

### Time management (V2/V3)
- Invasion roste samovolně
- Nezbytečné úkoly = ztráta času
- Den/noc cyklus
- Spánek = obnovení vs. ztráta času

### Mental discipline (přesahy)
- Nepanikař
- Quest není prio, život ano
- Pokud něco nedává smysl, je to past

---

# ČÁST 7 — VALIDACE PRAVIDEL

## 7.1 Kontrolní otázky pro každé pravidlo

Před přidáním pravidla do hry:

✅ **Funguje v lese?**
✅ **Funguje ve městě?**
✅ **Funguje v alien modulu?**
✅ **Funguje ve dne i v noci?** (V2+)
✅ **Funguje proti všem typům nepřátel?**
✅ **Platí pro AI stejně jako pro hráče?**
✅ **Vidí ho hráč v akci?** (telegraphing)
✅ **Je důsledek porušení čitelný?**
✅ **Není v rozporu se starými pravidly?**

## 7.2 Greybox testovací protokol

Detail v **Workflow Checklist Fáze 1.5**.

---

# ČÁST 8 — ŽIVÝ DOKUMENT

Tento dokument je **živý**. Při každé iteraci hry:

- Přidávej nová pravidla, která vznikla
- Aktualizuj existující při změně mechaniky
- Zaznamenávej, která pravidla **nefungovala** v testu (a proč)
- Verzování: GameBible v0.1 → v0.2 → ...

## 8.1 Princip: pravidla jsou dohoda mezi designérem a hráčem

Pokud designér pravidlo poruší (skript, výjimka, "magic"), **rozbije celý systém**. Hráč ztratí důvěru. Pak hra už **není rules-based** — je to **lhář v sci-fi obalu**.

> **Nejdůležitější princip:** **Pravidla platí. Vždy. Bez výjimek. Pro hráče i AI.**

---

Game Bible v0.1 — Siege of the Blue World

*Aktualizovat při každém testu, který odhalí nové nuance.*

*Související dokumenty: PRD (specifikace), Tech Spec (mechaniky), Workflow Checklist (postup).*
