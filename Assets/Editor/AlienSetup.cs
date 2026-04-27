using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;
using System.IO;

[InitializeOnLoad]
public static class AlienSetup
{
    private const string AnimsPath  = "Assets/Entities/Alien_unequipped/animations";
    private const string TposePath  = "Assets/Entities/Alien_unequipped/Alien_unequipped_T-Pose.fbx";
    private const string CtrlPath   = "Assets/Entities/Alien_unequipped/AlienAnimator.controller";
    private const string PrefabPath = "Assets/Entities/Alien_unequipped/Alien.prefab";

    static AlienSetup()
    {
        EditorApplication.delayCall += () =>
        {
            if (!File.Exists(Path.Combine(Application.dataPath, "..", PrefabPath)))
            {
                Debug.Log("[AlienSetup] Auto-running setup (prefab missing).");
                Run();
            }
        };
    }

    [MenuItem("UCAF/Setup Alien Character")]
    public static void Run()
    {
        Debug.Log("[AlienSetup] Starting...");
        ConfigureFBXImports();
        AssetDatabase.Refresh();
        var ctrl = CreateAnimatorController();
        CreateOrUpdatePrefab(ctrl);
        PlaceInScene();
        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("[AlienSetup] Done.");
    }

    // ── FBX Import ─────────────────────────────────────────────────────────

    private static void ConfigureFBXImports()
    {
        ConfigureFBX(TposePath, false);

        string[] anims = {
            "Alien_unequipped_Breathing_Idle.fbx",
            "Alien_unequipped_Walking.fbx",
            "Alien_unequipped_Walking_Backward.fbx",
            "Alien_unequipped_Left_Strafe_Walking.fbx",
            "Alien_unequipped_Right_Strafe_Walking.fbx",
            "Alien_unequipped_Running.fbx",
            "Alien_unequipped_Sprint.fbx",
            "Alien_unequipped_Jumping.fbx",
            "Alien_unequipped_Forward_Jump.fbx",
            "Alien_unequipped_Running Jump.fbx",
            "Alien_unequipped_Start_Walking.fbx",
        };
        foreach (var f in anims)
        {
            string p = AnimsPath + "/" + f;
            if (File.Exists(Path.Combine(Application.dataPath, "..", p)))
                ConfigureFBX(p, true);
            else
                Debug.LogWarning($"[AlienSetup] File not found: {p}");
        }
    }

    private static void ConfigureFBX(string assetPath, bool animationOnly)
    {
        var imp = AssetImporter.GetAtPath(assetPath) as ModelImporter;
        if (imp == null) { Debug.LogWarning($"[AlienSetup] Importer not found: {assetPath}"); return; }

        // Unity 6: use Human (not Humanoid)
        imp.animationType = ModelImporterAnimationType.Human;
        imp.avatarSetup   = animationOnly
            ? ModelImporterAvatarSetup.CopyFromOther
            : ModelImporterAvatarSetup.CreateFromThisModel;

        imp.importAnimation = true;

        // Unity 6: importMaterials removed → use materialImportMode
        imp.materialImportMode = animationOnly
            ? ModelImporterMaterialImportMode.None
            : ModelImporterMaterialImportMode.ImportViaMaterialDescription;

        if (animationOnly)
        {
            var avatar = AssetDatabase.LoadAssetAtPath<Avatar>(TposePath);
            if (avatar != null) imp.sourceAvatar = avatar;
        }

        imp.SaveAndReimport();
        Debug.Log($"[AlienSetup] Configured: {assetPath}");
    }

    // ── Animator Controller ────────────────────────────────────────────────

    private static AnimatorController CreateAnimatorController()
    {
        if (File.Exists(Path.Combine(Application.dataPath, "..", CtrlPath)))
            AssetDatabase.DeleteAsset(CtrlPath);

        var ctrl = AnimatorController.CreateAnimatorControllerAtPath(CtrlPath);
        ctrl.AddParameter("VelocityX", AnimatorControllerParameterType.Float);
        ctrl.AddParameter("VelocityZ", AnimatorControllerParameterType.Float);
        ctrl.AddParameter("IsGrounded", AnimatorControllerParameterType.Bool);
        ctrl.AddParameter("Jump",       AnimatorControllerParameterType.Trigger);

        var sm = ctrl.layers[0].stateMachine;

        // Clips
        var clipIdle     = LoadClip("Alien_unequipped_Breathing_Idle.fbx");
        var clipWalk     = LoadClip("Alien_unequipped_Walking.fbx");
        var clipWalkBack = LoadClip("Alien_unequipped_Walking_Backward.fbx");
        var clipStrafeL  = LoadClip("Alien_unequipped_Left_Strafe_Walking.fbx");
        var clipStrafeR  = LoadClip("Alien_unequipped_Right_Strafe_Walking.fbx");
        var clipRun      = LoadClip("Alien_unequipped_Running.fbx");
        var clipSprint   = LoadClip("Alien_unequipped_Sprint.fbx");
        var clipJump     = LoadClip("Alien_unequipped_Jumping.fbx");
        var clipRunJump  = LoadClip("Alien_unequipped_Running Jump.fbx");

        // Locomotion blend tree state
        var locoState = sm.AddState("Locomotion");

        var blend = new BlendTree();
        AssetDatabase.AddObjectToAsset(blend, ctrl);
        blend.name              = "LocoBlend";
        blend.blendType         = BlendTreeType.FreeformDirectional2D;
        blend.blendParameter    = "VelocityX";
        blend.blendParameterY   = "VelocityZ";
        blend.useAutomaticThresholds = false;

        AddClip(blend, clipIdle,     0f,  0f);
        AddClip(blend, clipWalk,     0f,  1f);
        AddClip(blend, clipRun,      0f,  2f);
        AddClip(blend, clipSprint,   0f,  3f);
        AddClip(blend, clipWalkBack, 0f, -1f);
        AddClip(blend, clipStrafeL, -1f,  0f);
        AddClip(blend, clipStrafeR,  1f,  0f);

        locoState.motion  = blend;
        sm.defaultState   = locoState;

        // Jump state
        var jumpState = sm.AddState("Jump");
        if (clipJump) jumpState.motion = clipJump;

        var runJumpState = sm.AddState("RunJump");
        if (clipRunJump) runJumpState.motion = clipRunJump;

        // Loco → Jump
        var t1 = locoState.AddTransition(jumpState);
        t1.AddCondition(AnimatorConditionMode.If, 0, "Jump");
        t1.hasExitTime = false;
        t1.duration    = 0.1f;

        // Jump → Loco
        var t2 = jumpState.AddTransition(locoState);
        t2.AddCondition(AnimatorConditionMode.If, 0, "IsGrounded");
        t2.hasExitTime = false;
        t2.duration    = 0.15f;

        // Loco → RunJump (sprint)
        var t3 = locoState.AddTransition(runJumpState);
        t3.AddCondition(AnimatorConditionMode.If,      0,   "Jump");
        t3.AddCondition(AnimatorConditionMode.Greater, 2.5f,"VelocityZ");
        t3.hasExitTime = false;
        t3.duration    = 0.1f;

        // RunJump → Loco
        var t4 = runJumpState.AddTransition(locoState);
        t4.AddCondition(AnimatorConditionMode.If, 0, "IsGrounded");
        t4.hasExitTime = false;
        t4.duration    = 0.15f;

        EditorUtility.SetDirty(ctrl);
        return ctrl;
    }

    private static void AddClip(BlendTree tree, AnimationClip clip, float x, float y)
    {
        if (clip == null) return;
        tree.AddChild(clip, new Vector2(x, y));
    }

    private static AnimationClip LoadClip(string fbxFile)
    {
        string path = AnimsPath + "/" + fbxFile;
        foreach (var o in AssetDatabase.LoadAllAssetsAtPath(path))
            if (o is AnimationClip c && !c.name.StartsWith("__preview__"))
                return c;
        Debug.LogWarning($"[AlienSetup] Clip not found in: {path}");
        return null;
    }

    // ── Prefab ─────────────────────────────────────────────────────────────

    private static void CreateOrUpdatePrefab(AnimatorController ctrl)
    {
        var meshAsset = AssetDatabase.LoadAssetAtPath<GameObject>(TposePath);
        if (meshAsset == null) { Debug.LogError($"[AlienSetup] T-Pose not found: {TposePath}"); return; }

        var root = new GameObject("Alien");

        // CharacterController — capsule fits ~1.8m humanoid
        var cc    = root.AddComponent<CharacterController>();
        cc.height = 1.8f;
        cc.radius = 0.4f;
        cc.center = new Vector3(0, 0.9f, 0);

        // Mesh child (the visible alien body)
        var mesh = Object.Instantiate(meshAsset, root.transform);
        mesh.name = "AlienMesh";
        mesh.transform.localPosition = Vector3.zero;
        mesh.transform.localRotation = Quaternion.identity;

        // FBX brings its own Animator — destroy it so it doesn't fight the root Animator
        foreach (var a in mesh.GetComponentsInChildren<Animator>(true))
            Object.DestroyImmediate(a);

        // Animator on root
        var anim = root.AddComponent<Animator>();
        anim.runtimeAnimatorController = ctrl;
        anim.applyRootMotion           = false;
        anim.avatar = AssetDatabase.LoadAssetAtPath<Avatar>(TposePath);

        // Camera rig pivot — third-person: pitch pivots here
        // CameraRig at shoulder height, camera pulled back behind alien
        var camRig = new GameObject("CameraRig");
        camRig.transform.SetParent(root.transform);
        camRig.transform.localPosition = new Vector3(0, 1.6f, 0);

        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        camGO.transform.SetParent(camRig.transform);
        camGO.transform.localPosition = new Vector3(0, 0.5f, -1.5f); // 1.5 units behind
        camGO.transform.localRotation = Quaternion.identity;
        camGO.AddComponent<Camera>();
        camGO.AddComponent<AudioListener>();

        // AlienController script
        root.AddComponent<AlienController>();

        // Save prefab
        string dir = Path.GetDirectoryName(Path.Combine(Application.dataPath, "..", PrefabPath));
        Directory.CreateDirectory(dir);
        PrefabUtility.SaveAsPrefabAsset(root, PrefabPath);
        Object.DestroyImmediate(root);
        Debug.Log($"[AlienSetup] Prefab saved: {PrefabPath}");
    }

    // ── Scene placement ────────────────────────────────────────────────────

    private static void PlaceInScene()
    {
        var existing = GameObject.Find("Alien");
        if (existing != null) Object.DestroyImmediate(existing);

        var prefab = AssetDatabase.LoadAssetAtPath<GameObject>(PrefabPath);
        if (prefab == null) { Debug.LogError("[AlienSetup] Prefab missing, cannot place."); return; }

        var inst = (GameObject)PrefabUtility.InstantiatePrefab(prefab);
        inst.transform.position = new Vector3(0, 0.05f, 0);

        UnityEditor.SceneManagement.EditorSceneManager.MarkSceneDirty(
            UnityEditor.SceneManagement.EditorSceneManager.GetActiveScene());
        Debug.Log("[AlienSetup] Alien placed in scene.");
    }
}
