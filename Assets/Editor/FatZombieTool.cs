using UnityEngine;
using UnityEditor;
using UnityEngine.AI;
using Photon.Pun;
using System.IO;

public class FatZombieTool : EditorWindow
{
    [MenuItem("Tools/Create Fat Zombie")]
    public static void CreateFatZombie()
    {
        // 1. Define paths
        string sourcePath = "Assets/Asset Packs/FatZombie/Prefab/FatZombie.prefab";
        string targetFolder = "Assets/Resources";
        string targetPath = Path.Combine(targetFolder, "FatZombie.prefab");

        // 2. Load Source Prefab
        GameObject sourcePrefab = AssetDatabase.LoadAssetAtPath<GameObject>(sourcePath);
        if (sourcePrefab == null)
        {
            Debug.LogError($"Could not find source prefab at {sourcePath}. Please check if the asset is imported correctly.");
            return;
        }

        // 3. Create Target Directory if needed
        if (!Directory.Exists(targetFolder))
        {
            Directory.CreateDirectory(targetFolder);
        }

        // 4. Instantiate temporarily to modify
        GameObject instance = (GameObject)PrefabUtility.InstantiatePrefab(sourcePrefab);
        instance.name = "FatZombie";

        try
        {
            // 5. Add Components
            
            // --- Navigation ---
            NavMeshAgent agent = instance.GetComponent<NavMeshAgent>();
            if (agent == null) agent = instance.AddComponent<NavMeshAgent>();
            agent.speed = 1.5f; // Slower than normal zombie
            agent.stoppingDistance = 2f;
            agent.radius = 0.6f; // Fatter
            agent.height = 2.0f;

            // --- Physics ---
            CapsuleCollider collider = instance.GetComponent<CapsuleCollider>();
            if (collider == null) collider = instance.AddComponent<CapsuleCollider>();
            collider.center = new Vector3(0, 1.0f, 0);
            collider.radius = 0.5f;
            collider.height = 2.0f;

            // --- Networking (Photon) ---
            PhotonView pv = instance.GetComponent<PhotonView>();
            if (pv == null) pv = instance.AddComponent<PhotonView>();
            
            PhotonTransformView ptv = instance.GetComponent<PhotonTransformView>();
            if (ptv == null) ptv = instance.AddComponent<PhotonTransformView>();
            ptv.m_SynchronizePosition = true;
            ptv.m_SynchronizeRotation = true;

            PhotonAnimatorView pav = instance.GetComponent<PhotonAnimatorView>();
            if (pav == null) pav = instance.AddComponent<PhotonAnimatorView>();
            // Note: PhotonAnimatorView setup via code is tricky, usually requires serializedObject modification
            // We will leave it default or user might need to check layers manually if complex.
            // But basic trigger syncing usually works if "SetTrigger" is used via RPC or custom sync. 
            // In this project, EnemyAI uses generic Animator, and PhotonAnimatorView usually syncs layers/params automatically if configured.
            // Let's try to set basic sync parameters for common animation states if possible, or rely on defaults.
            pav.SetParameterSynchronized("Attack", PhotonAnimatorView.ParameterType.Bool, PhotonAnimatorView.SynchronizeType.Discrete);
            pav.SetParameterSynchronized("Move", PhotonAnimatorView.ParameterType.Trigger, PhotonAnimatorView.SynchronizeType.Discrete);
            pav.SetParameterSynchronized("Die", PhotonAnimatorView.ParameterType.Trigger, PhotonAnimatorView.SynchronizeType.Discrete);
            pav.SetParameterSynchronized("Idle", PhotonAnimatorView.ParameterType.Trigger, PhotonAnimatorView.SynchronizeType.Discrete);

            // Add Observed Components to PhotonView
            // We need to overwrite the observed components list
            SerializedObject pvObject = new SerializedObject(pv);
            SerializedProperty observedComponents = pvObject.FindProperty("ObservedComponents");
            observedComponents.ClearArray();
            observedComponents.InsertArrayElementAtIndex(0);
            observedComponents.GetArrayElementAtIndex(0).objectReferenceValue = ptv;
            observedComponents.InsertArrayElementAtIndex(1);
            observedComponents.GetArrayElementAtIndex(1).objectReferenceValue = pav;
            pvObject.ApplyModifiedProperties();

            // --- Audio ---
            AudioSource audio = instance.GetComponent<AudioSource>();
            if (audio == null) audio = instance.AddComponent<AudioSource>();
            audio.spatialBlend = 1.0f; // 3D Sound
            audio.minDistance = 2f;
            audio.maxDistance = 20f;

            // --- Game Logic ---
            // Fix Animator Controller FIRST
            Animator animator = instance.GetComponent<Animator>();
            if (animator == null) animator = instance.AddComponent<Animator>();
            
            // Explicitly ensure Avatar is correct (should be from prefab)
            if (animator.avatar == null)
            {
                Debug.LogWarning("FatZombie Animator missing Avatar! Trying to find it in source.");
                // Try to load from FBX if needed, but prefab usually has it.
            }

            // Create Override Controller to separate from Zombie logic
            RuntimeAnimatorController zombieController = AssetDatabase.LoadAssetAtPath<RuntimeAnimatorController>("Assets/Asset Packs/Zombie/Animations/Zombie.controller");
            if (zombieController != null)
            {
                // Create the override controller file
                string overridePath = Path.Combine(targetFolder, "FatZombieController.overrideController");
                AnimatorOverrideController overrideController = new AnimatorOverrideController(zombieController);
                AssetDatabase.CreateAsset(overrideController, overridePath);
                
                animator.runtimeAnimatorController = overrideController;
                Debug.Log("Created FatZombie Override Controller.");
            }
            else
            {
                Debug.LogWarning("Could not find standard Zombie.controller. FatZombie might not animate.");
            }
            
            animator.applyRootMotion = false; // CRITICAL: AI uses NavMesh, disabling root motion prevents scaling/position weirdness
            animator.cullingMode = AnimatorCullingMode.AlwaysAnimate;


            EnemyHealth health = instance.GetComponent<EnemyHealth>();
            if (health == null) health = instance.AddComponent<EnemyHealth>();
            // Set Health Stats - access via SerializedObject since fields might be private/protected
            SerializedObject healthObj = new SerializedObject(health);
            healthObj.FindProperty("hitPoints").floatValue = 200f; // More health for Fat Zombie
            // Assign sounds if possible (hard to find exact GUIDs without search, defaulting to null, user can assign)
            healthObj.ApplyModifiedProperties();

            EnemyAI ai = instance.GetComponent<EnemyAI>();
            if (ai == null) ai = instance.AddComponent<EnemyAI>();
            SerializedObject aiObj = new SerializedObject(ai);
            aiObj.FindProperty("chaseRange").floatValue = 15f;
            aiObj.FindProperty("turnSpeed").floatValue = 3f; // Slower turn
            aiObj.FindProperty("damage").floatValue = 60f; // Harder hit
            aiObj.ApplyModifiedProperties();

            // 6. Save as new Prefab
            PrefabUtility.SaveAsPrefabAsset(instance, targetPath);
            Debug.Log($"<color=green>Success! FatZombie prefab created at: {targetPath}</color>");
            Debug.Log("Please select the prefab and verify Audio Clips in Inspector (Intro/Death sounds).");

            // 7. Ping references for user
            Selection.activeObject = AssetDatabase.LoadAssetAtPath<GameObject>(targetPath);
            EditorGUIUtility.PingObject(Selection.activeObject);

        }
        catch (System.Exception e)
        {
            Debug.LogError("Error creating Fat Zombie: " + e.Message);
        }
        finally
        {
            GameObject.DestroyImmediate(instance);
        }
    }
}
