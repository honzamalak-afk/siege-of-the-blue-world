import json, uuid, datetime, time, os

w = "C:/Projects/Hry/Siege of the Blue World/ucaf_workspace"
def send(t, p={}, to=60):
    cid = f"cmd_{uuid.uuid4().hex[:8]}"
    json.dump({"id":cid,"type":t,"timestamp":datetime.datetime.now().isoformat()+"Z",
               "params_list":[{"key":k,"value":str(v)} for k,v in p.items()]},
              open(f"{w}/commands/pending/{cid}.json","w"))
    for _ in range(to*2):
        for f,l in [(f"{w}/commands/done","OK"),(f"{w}/commands/errors","ERR")]:
            if os.path.exists(f"{f}/{cid}.json"):
                r=json.load(open(f"{f}/{cid}.json"))
                print(f"  [{l}] {t}: {r.get('message','?')}"); return r
        time.sleep(0.5)
    print(f"  [TIMEOUT] {t}"); return None

print("1. Exit play mode..."); send("play_mode", {"enter":"false"})
print("2. Wait 3s..."); time.sleep(3)
print("3. Setup alien..."); send("setup_alien")
print("4. Save scene..."); send("save_scene")
print("Done.")
