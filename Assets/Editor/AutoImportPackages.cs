using UnityEditor;
using UnityEngine;
using System.IO;

[InitializeOnLoad]
public static class AutoImportPackages
{
    static readonly string[] Packages =
    {
        @"C:/Users/honza/AppData/Roaming/Unity/Asset Store-5.x/Render Knight/Textures MaterialsSkies/Fantasy Skybox FREE.unitypackage",
        @"C:/Users/honza/AppData/Roaming/Unity/Asset Store-5.x/Jason Booth/Editor ExtensionsTerrain/MicroSplat.unitypackage",
        @"C:/Users/honza/AppData/Roaming/Unity/Asset Store-5.x/Kevin Iglesias/Animation/Human Basic Motions FREE.unitypackage",
        @"C:/Users/honza/AppData/Roaming/Unity/Asset Store-5.x/Staggart Creations/Editor ExtensionsTerrain/Procedural Terrain Painter FREE - Automatic Terrain Texturing.unitypackage",
        @"C:/Users/honza/AppData/Roaming/Unity/Asset Store-5.x/Rowlan/Editor ExtensionsTerrain/StampIT Collection - FREE Heightmaps for Unity 6 MicroVerse Gaia Terrain.unitypackage",
    };

    const string DoneKey = "AutoImportPackages_Done";

    static AutoImportPackages()
    {
        if (EditorPrefs.GetBool(DoneKey, false)) return;
        EditorApplication.delayCall += Run;
    }

    static void Run()
    {
        EditorPrefs.SetBool(DoneKey, true);
        foreach (var path in Packages)
        {
            if (!File.Exists(path))
            {
                Debug.LogWarning($"[AutoImport] Package not found: {path}");
                continue;
            }
            Debug.Log($"[AutoImport] Importing: {Path.GetFileName(path)}");
            AssetDatabase.ImportPackage(path, false);
        }
        Debug.Log("[AutoImport] All packages queued. Script can be deleted.");
    }
}
