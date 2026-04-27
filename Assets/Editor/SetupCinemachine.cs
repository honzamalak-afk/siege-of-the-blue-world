using UnityEngine;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine.SceneManagement;
using Cinemachine;

public static class SetupCinemachine
{
    [MenuItem("UCAF/Setup Cinemachine TPS Camera")]
    public static void Run()
    {
        var alien = GameObject.Find("Alien");
        if (alien == null) { Debug.LogError("[CmSetup] Alien not found in scene."); return; }

        // ── 1. Main Camera: root-level GO with CinemachineBrain ─────────────────
        foreach (var cam in alien.GetComponentsInChildren<Camera>(true))
        {
            cam.enabled = false;
            EditorUtility.SetDirty(cam);
        }

        var allCams = Object.FindObjectsByType<Camera>(FindObjectsInactive.Include);
        foreach (var cam in allCams)
            if (cam.transform.parent == null)
                Undo.DestroyObjectImmediate(cam.gameObject);

        var camGO = new GameObject("Main Camera");
        camGO.tag = "MainCamera";
        Undo.RegisterCreatedObjectUndo(camGO, "Create Main Camera");
        camGO.AddComponent<Camera>();
        camGO.AddComponent<CinemachineBrain>();

        foreach (var al in alien.GetComponentsInChildren<AudioListener>(true))
            al.enabled = false;
        if (!camGO.GetComponent<AudioListener>())
            camGO.AddComponent<AudioListener>();

        // ── 2. PitchPivot: reuse CmAimTarget (prefab child) as pitch orbit pivot ─
        // Adding new GameObjects to prefab instances via Undo has serialization quirks,
        // so we reuse the existing CmAimTarget child instead of creating CamPivot.
        Transform pivotT = alien.transform.Find("CmAimTarget");
        if (pivotT == null) pivotT = alien.transform.Find("CamPivot");
        GameObject pivotGO;
        if (pivotT != null)
        {
            pivotGO = pivotT.gameObject;
        }
        else
        {
            pivotGO = new GameObject("CmAimTarget");
            Undo.RegisterCreatedObjectUndo(pivotGO, "Create CmAimTarget");
            Undo.SetTransformParent(pivotGO.transform, alien.transform, "Parent CmAimTarget");
        }
        pivotGO.transform.localPosition = new Vector3(0f, 1.4f, 0f);
        pivotGO.transform.localRotation = Quaternion.identity;

        // ── 3. Virtual Camera ────────────────────────────────────────────────────
        GameObject vcamGO = GameObject.Find("CM_TPS_vcam");
        if (vcamGO == null)
        {
            vcamGO = new GameObject("CM_TPS_vcam");
            Undo.RegisterCreatedObjectUndo(vcamGO, "Create CM_TPS_vcam");
        }

        var vcam = vcamGO.GetComponent<CinemachineVirtualCamera>();
        if (vcam == null)
            vcam = Undo.AddComponent<CinemachineVirtualCamera>(vcamGO);

        vcam.Follow     = pivotGO.transform;
        vcam.LookAt     = alien.transform;
        vcam.m_Priority = 10;
        vcam.m_Lens.FieldOfView = 60f;

        // ── 4. Body: Cinemachine3rdPersonFollow ──────────────────────────────────
        vcam.InvalidateComponentPipeline();
        var body = vcam.GetCinemachineComponent<Cinemachine3rdPersonFollow>();
        if (body == null)
            body = vcam.AddCinemachineComponent<Cinemachine3rdPersonFollow>();

        body.VerticalArmLength    = 0.3f;
        body.CameraDistance       = 4.0f;
        body.ShoulderOffset       = new Vector3(0.4f, 0f, 0f);
        body.Damping              = new Vector3(0.3f, 0.3f, 0.5f);
        body.DampingIntoCollision = 0.5f;
        body.DampingFromCollision = 2.5f;

        // ── 5. Aim: CinemachineComposer — smooth horizontal + vertical tracking ──
        var aim = vcam.GetCinemachineComponent<CinemachineComposer>();
        if (aim == null)
            aim = vcam.AddCinemachineComponent<CinemachineComposer>();

        aim.m_TrackedObjectOffset  = new Vector3(0f, 1.5f, 0f);
        aim.m_LookaheadTime        = 0f;
        aim.m_LookaheadSmoothing   = 0f;
        aim.m_HorizontalDamping    = 0.1f;
        aim.m_VerticalDamping      = 0.1f;
        aim.m_DeadZoneWidth        = 0.1f;
        aim.m_DeadZoneHeight       = 0.1f;
        aim.m_SoftZoneWidth        = 0.5f;
        aim.m_SoftZoneHeight       = 0.5f;

        // ── 6. Pitch controller on pivot ─────────────────────────────────────────
        if (!pivotGO.GetComponent<CmAimController>())
            Undo.AddComponent<CmAimController>(pivotGO);

        EditorUtility.SetDirty(vcamGO);
        EditorUtility.SetDirty(camGO);
        EditorUtility.SetDirty(pivotGO);
        EditorSceneManager.MarkSceneDirty(SceneManager.GetActiveScene());

        Debug.Log("[CmSetup] Done — Composer+3rdPersonFollow TPS. Save scene after.");
    }
}
