import json, uuid, datetime, time, os

workspace = "C:/Projects/Hry/Siege of the Blue World/ucaf_workspace"
pending   = f"{workspace}/commands/pending"
done      = f"{workspace}/commands/done"
errors    = f"{workspace}/commands/errors"

def send(cmd_type, params={}, timeout=60):
    cmd_id = f"cmd_{uuid.uuid4().hex[:8]}"
    cmd = {
        "id": cmd_id, "type": cmd_type,
        "timestamp": datetime.datetime.now().isoformat() + "Z",
        "params_list": [{"key": k, "value": str(v)} for k, v in params.items()]
    }
    with open(f"{pending}/{cmd_id}.json", "w") as f:
        json.dump(cmd, f, indent=2)
    for _ in range(timeout * 2):
        for folder, label in [(done, "OK"), (errors, "ERR")]:
            p = f"{folder}/{cmd_id}.json"
            if os.path.exists(p):
                r = json.load(open(p))
                print(f"  [{label}] {cmd_type}: {r.get('message','?')}")
                return r
        time.sleep(0.5)
    print(f"  [TIMEOUT] {cmd_type}")
    return None

print("=== Re-running AlienSetup ===")
send("asset_refresh")
time.sleep(3)
send("setup_alien")
send("save_scene")
print("Done.")
