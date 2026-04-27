using UnityEditor;
using UnityEngine;

[InitializeOnLoad]
public static class FixAnimLoops
{
    static FixAnimLoops()
    {
        EditorApplication.delayCall += Run;
    }

    [MenuItem("UCAF/Fix Animation Loops")]
    public static void Run()
    {
        string folder = "Assets/Entities/Alien_unequipped/animations";
        string[] guids = AssetDatabase.FindAssets("t:Model", new[] { folder });
        int fixedCount = 0;
        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            string name = System.IO.Path.GetFileNameWithoutExtension(path);
            bool shouldLoop = !name.ToLower().Contains("jump")
                           && !name.ToLower().Contains("start");

            var importer = AssetImporter.GetAtPath(path) as ModelImporter;
            if (importer == null) continue;

            var clips = importer.defaultClipAnimations;
            bool changed = false;
            for (int i = 0; i < clips.Length; i++)
            {
                if (clips[i].loopTime != shouldLoop)
                {
                    clips[i].loopTime = shouldLoop;
                    changed = true;
                }
            }
            if (changed)
            {
                importer.clipAnimations = clips;
                importer.SaveAndReimport();
                fixedCount++;
                Debug.Log($"[FixAnimLoops] {name}: loopTime={shouldLoop}");
            }
        }
        Debug.Log($"[FixAnimLoops] Done. Fixed {fixedCount} clips.");
    }
}
