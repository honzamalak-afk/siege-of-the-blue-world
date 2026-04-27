import json, uuid, datetime, time, os

workspace = "C:/Projects/Hry/Siege of the Blue World/ucaf_workspace"
pending   = f"{workspace}/commands/pending"
done      = f"{workspace}/commands/done"
errors    = f"{workspace}/commands/errors"

def send(cmd_type, params, timeout=30):
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
            r = json.load(open(f"{done}/{cmd_id}.json"))
            print(f"  [{cmd_type}] {r.get('message','?')}")
            return r
        if os.path.exists(f"{errors}/{cmd_id}.json"):
            r = json.load(open(f"{errors}/{cmd_id}.json"))
            print(f"  [ERROR {cmd_type}] {r.get('message','?')}")
            return r
        time.sleep(0.5)
    print(f"  [TIMEOUT {cmd_type}]")
    return None

print("=== Fix scene ===")

# 1. ping
r = send("ping", {})
if not r or not r.get("success"):
    print("UCAF not responding. Make sure Unity is open and not in Safe Mode.")
    exit(1)

# 2. Smaž scénovou Main Camera
print("\n1. Deleting scene Main Camera...")
send("delete_object", {"name": "Main Camera"})

# 3. Vytvoř velkou zem (Plane, scale 10x1x10 = 100x100m)
print("\n2. Creating ground plane...")
send("create_object", {
    "name":      "Ground",
    "primitive": "Plane",
    "position":  "0, 0, 0",
    "scale":     "10, 1, 10",
})

# 4. Ulož scénu
print("\n3. Saving scene...")
send("save_scene", {})

print("\nDone.")
