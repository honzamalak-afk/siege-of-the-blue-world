
using UnityEngine;
using UnityEditor;
using System.IO;

public class AnimPreviewCapture
{
    [MenuItem("UCAF/Capture Animation Previews")]
    public static void CaptureAll()
    {
        string animFolder  = "Assets/Entities/Universal animation YBOT";
        string outputDir   = "C:/Projects/Hry/Siege of the Blue World/ucaf_workspace/screenshots/anim_previews";
        Directory.CreateDirectory(outputDir);

        var alien = GameObject.Find("Alien");
        if (alien == null) { Debug.LogError("[AnimPreview] Alien not found in scene."); return; }

        // Animations target bones under AlienMesh (mixamorig:Hips is direct child of AlienMesh)
        var alienMesh = alien.transform.Find("AlienMesh");
        if (alienMesh == null) { Debug.LogError("[AnimPreview] AlienMesh not found."); return; }

        // Dedicated preview camera — positioned to show full character from front
        var camGO = new GameObject("_PreviewCam");
        var cam   = camGO.AddComponent<Camera>();
        cam.clearFlags       = CameraClearFlags.SolidColor;
        cam.backgroundColor  = new Color(0.18f, 0.18f, 0.22f);
        cam.fieldOfView      = 45f;
        cam.nearClipPlane    = 0.1f;
        cam.farClipPlane     = 50f;

        var rt = new RenderTexture(512, 768, 24);

        var guids = AssetDatabase.FindAssets("t:AnimationClip", new[] { animFolder });
        int count = 0;

        foreach (var guid in guids)
        {
            string path = AssetDatabase.GUIDToAssetPath(guid);
            var clip = AssetDatabase.LoadAssetAtPath<AnimationClip>(path);
            if (clip == null) continue;

            float sampleTime = clip.length * 0.4f;

            AnimationMode.StartAnimationMode();
            // Sample on AlienMesh — its direct children are the mixamorig bones
            AnimationMode.SampleAnimationClip(alienMesh.gameObject, clip, sampleTime);

            // Camera: front view, full body visible
            Vector3 charPos  = alien.transform.position;
            Vector3 camPos   = charPos + new Vector3(0f, 0.9f, 3.2f);
            camGO.transform.position = camPos;
            camGO.transform.LookAt(charPos + Vector3.up * 0.9f);

            cam.targetTexture = rt;
            cam.Render();
            cam.targetTexture = null;

            RenderTexture.active = rt;
            var tex = new Texture2D(512, 768, TextureFormat.RGB24, false);
            tex.ReadPixels(new Rect(0, 0, 512, 768), 0, 0);
            tex.Apply();
            RenderTexture.active = null;

            string safeName = System.IO.Path.GetFileNameWithoutExtension(path).Replace(" ", "_").Replace("(", "").Replace(")", "");
            string outPath = Path.Combine(outputDir, safeName + ".png");
            File.WriteAllBytes(outPath, tex.EncodeToPNG());
            Object.DestroyImmediate(tex);

            AnimationMode.StopAnimationMode();
            count++;
        }

        rt.Release();
        Object.DestroyImmediate(camGO);
        Debug.Log($"[AnimPreview] Done — {count} previews saved to {outputDir}");
    }
}
