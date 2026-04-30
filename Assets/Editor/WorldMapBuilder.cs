using UnityEngine;
using UnityEditor;
using UnityEditor.AI;
using System.IO;

[InitializeOnLoad]
public static class WorldMapBuilder
{
    const string BUILT_KEY = "SiegeBW_WorldMapBuilt_v1";

    static WorldMapBuilder()
    {
        if (!EditorPrefs.GetBool(BUILT_KEY, false))
            EditorApplication.delayCall += Build;
    }

    [MenuItem("Siege/Build World Map (Greybox)")]
    public static void Build()
    {
        // Cleanup legacy objects
        foreach (var n in new[] { "Ground", "TestPlatform", "WorldMap" })
        {
            var old = GameObject.Find(n);
            if (old != null) Object.DestroyImmediate(old);
        }

        var root = new GameObject("WorldMap");

        // ── Zone grounds (top face at Y=0) ──────────────────────────────────
        // Layout: player travels along Z axis
        //   Hub         Z   0–200   100m wide
        //   Forest      Z 200–400   200m wide
        //   ModuleZone  Z 400–600   100m wide
        AddGround(root, "Hub_Ground",
            new Vector3(0f, -0.5f, 100f), new Vector3(100f, 1f, 200f),
            new Color(0.72f, 0.72f, 0.72f));

        AddGround(root, "Forest_Ground",
            new Vector3(0f, -0.5f, 300f), new Vector3(200f, 1f, 200f),
            new Color(0.55f, 0.55f, 0.55f));

        AddGround(root, "Module_Ground",
            new Vector3(0f, -0.5f, 500f), new Vector3(100f, 1f, 200f),
            new Color(0.40f, 0.40f, 0.40f));

        // ── Zone boundary markers (yellow, no collision) ─────────────────────
        AddSlab(root, "Boundary_Hub_Forest",
            new Vector3(0f, 0.02f, 200f), new Vector3(200f, 0.1f, 0.4f),
            new Color(1f, 0.85f, 0.1f));

        AddSlab(root, "Boundary_Forest_Module",
            new Vector3(0f, 0.02f, 400f), new Vector3(200f, 0.1f, 0.4f),
            new Color(1f, 0.85f, 0.1f));

        // ── Cover grids (1×2×1m dark-gray boxes, ~12m spacing) ───────────────
        // 5m buffer inside zone edges to avoid boundary clutter
        AddCoverGrid(root, "Forest_Cover",  -100f, 100f, 205f, 395f, 12f);
        AddCoverGrid(root, "Module_Cover",   -50f,  50f, 405f, 595f, 12f);

        // ── Alien ship module (40×5×10m, black + blue emission) ──────────────
        AddAlienModule(root);

        // ── Reposition player to Hub start ───────────────────────────────────
        var alien = GameObject.Find("Alien");
        if (alien != null)
            alien.transform.position = new Vector3(0f, 0.1f, 30f);

        // ── NavMesh bake ──────────────────────────────────────────────────────
        try
        {
#pragma warning disable 0618
            NavMeshBuilder.BuildNavMesh();
#pragma warning restore 0618
            Debug.Log("[WorldMap] NavMesh baked.");
        }
        catch (System.Exception e)
        {
            Debug.LogWarning("[WorldMap] NavMesh bake skipped: " + e.Message);
        }

        // ── Save ──────────────────────────────────────────────────────────────
        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEngine.SceneManagement.SceneManager.GetActiveScene());
        UnityEditor.SceneManagement.EditorSceneManager.SaveOpenScenes();

        EditorPrefs.SetBool(BUILT_KEY, true);
        Debug.Log("[WorldMap] Done. Hub Z0-200 | Forest Z200-400 | ModuleZone Z400-600.");
    }

    // ── Builders ──────────────────────────────────────────────────────────────

    static void AddGround(GameObject parent, string id, Vector3 pos, Vector3 scale, Color col)
    {
        var go = MakeCube(parent, id, pos, scale, SaveMat(id, col));
        SetStatic(go);
    }

    static void AddSlab(GameObject parent, string id, Vector3 pos, Vector3 scale, Color col)
    {
        var go = MakeCube(parent, id, pos, scale, SaveMat(id, col));
        go.GetComponent<Collider>().enabled = false;
    }

    static void AddCoverGrid(GameObject parent, string id,
        float xMin, float xMax, float zMin, float zMax, float spacing)
    {
        var group = new GameObject(id);
        group.transform.SetParent(parent.transform);
        var mat = SaveMat(id, new Color(0.22f, 0.22f, 0.22f));

        int ci = 0;
        for (float x = xMin + spacing * 0.5f; x < xMax; x += spacing, ci++)
        {
            int ri = 0;
            for (float z = zMin + spacing * 0.5f; z < zMax; z += spacing, ri++)
            {
                if ((ci * 7 + ri * 3) % 11 == 0) continue; // ~9% natural gaps
                var c = MakeCube(group, $"C{ci}_{ri}",
                    new Vector3(x, 1f, z), new Vector3(1f, 2f, 1f), mat);
                SetStatic(c);
            }
        }
    }

    static void AddAlienModule(GameObject parent)
    {
        // Module center of ModuleZone — 40m wide, 5m tall, 10m deep
        var path = "Assets/Materials/Greybox_AlienModule.mat";
        var shader = Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        mat.SetColor("_BaseColor", Color.black);
        mat.SetColor("_EmissiveColor", new Color(0f, 0.35f, 1f) * 3f);
        mat.SetFloat("_UseEmissiveIntensity", 1f);
        mat.SetFloat("_EmissiveIntensity", 3f);
        EnsureMaterialsDir();
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(mat, path);

        var go = MakeCube(parent, "AlienModule",
            new Vector3(0f, 2.5f, 500f), new Vector3(40f, 5f, 10f), mat);
        SetStatic(go);
    }

    // ── Helpers ───────────────────────────────────────────────────────────────

    static GameObject MakeCube(GameObject parent, string name, Vector3 pos, Vector3 scale, Material mat)
    {
        var go = GameObject.CreatePrimitive(PrimitiveType.Cube);
        go.name = name;
        go.transform.SetParent(parent.transform);
        go.transform.position = pos;
        go.transform.localScale = scale;
        go.GetComponent<MeshRenderer>().sharedMaterial = mat;
        return go;
    }

    static Material SaveMat(string id, Color col)
    {
        EnsureMaterialsDir();
        var path = $"Assets/Materials/Greybox_{id}.mat";
        var shader = Shader.Find("HDRP/Lit") ?? Shader.Find("Standard");
        var mat = new Material(shader);
        mat.SetColor("_BaseColor", col);
        AssetDatabase.DeleteAsset(path);
        AssetDatabase.CreateAsset(mat, path);
        return mat;
    }

    static void SetStatic(GameObject go)
    {
#pragma warning disable 0618
        GameObjectUtility.SetStaticEditorFlags(go,
            StaticEditorFlags.NavigationStatic | StaticEditorFlags.ContributeGI);
#pragma warning restore 0618
    }

    static void EnsureMaterialsDir()
    {
        if (!AssetDatabase.IsValidFolder("Assets/Materials"))
            AssetDatabase.CreateFolder("Assets", "Materials");
    }
}
