using UnityEngine;
using UnityEditor;
using UnityEditor.Animations;

public class FixZombieController : Editor
{
    [MenuItem("Tools/Fix Zombie Controller")]
    public static void Fix()
    {
        string path = "Assets/Asset Packs/Zombie/Animations/Zombie.controller";
        AnimatorController controller = AssetDatabase.LoadAssetAtPath<AnimatorController>(path);

        if (controller == null)
        {
            Debug.LogError($"Could not find controller at {path}");
            return;
        }

        // 1. Add Parameters
        AddParameter(controller, "Attack", AnimatorControllerParameterType.Bool);
        AddParameter(controller, "Move", AnimatorControllerParameterType.Trigger);
        AddParameter(controller, "Die", AnimatorControllerParameterType.Trigger);
        AddParameter(controller, "Idle", AnimatorControllerParameterType.Trigger);

        // 2. Fix States and Transitions
        AnimatorStateMachine rootStateMachine = controller.layers[0].stateMachine;
        
        // Find States (ensure they exist or find by name)
        AnimatorState idleState = FindState(rootStateMachine, "Z_Idle");
        AnimatorState walkState = FindState(rootStateMachine, "Z_Walk_InPlace"); // Prefer InPlace for NavMesh
        if (walkState == null) walkState = FindState(rootStateMachine, "Z_Walk");
        AnimatorState attackState = FindState(rootStateMachine, "Z_Attack");
        AnimatorState deadState = FindState(rootStateMachine, "Z_FallingBack");
        
        if (idleState == null || walkState == null || attackState == null || deadState == null)
        {
            Debug.LogError("Could not find one of the core states (Idle, Walk, Attack, FallingBack). Please check animation names.");
            // Print all names to help
            foreach(var s in rootStateMachine.states) Debug.Log("Found state: " + s.state.name);
            return;
        }

        // Clear all existing transitions to prevent bad loops
        foreach (var state in rootStateMachine.states)
        {
            state.state.transitions = new AnimatorStateTransition[0];
        }
        rootStateMachine.anyStateTransitions = new AnimatorStateTransition[0];

        // 3. Setup Correct Logic

        // Any State -> Die
        AnimatorStateTransition dieTrans = rootStateMachine.AddAnyStateTransition(deadState);
        dieTrans.AddCondition(AnimatorConditionMode.If, 0, "Die");
        dieTrans.duration = 0.1f;

        // Idle -> Move (Walk)
        AnimatorStateTransition walkTrans = idleState.AddTransition(walkState);
        walkTrans.AddCondition(AnimatorConditionMode.If, 0, "Move");
        walkTrans.hasExitTime = false;
        
        // Walk -> Idle (When trigger consumed/stops)
        // Since AI spams Trigger("Move"), we rely on ExitTime to loop walk, or transition back if no trigger.
        // Actually for Trigger-based Movement, usually you transition back to Idle at end of clip automatically
        AnimatorStateTransition stopWalkTrans = walkState.AddTransition(idleState);
        stopWalkTrans.hasExitTime = true; // Let anim finish then go back to idle (looping if re-triggered)

        // Idle -> Attack
        AnimatorStateTransition attackTrans = idleState.AddTransition(attackState);
        attackTrans.AddCondition(AnimatorConditionMode.If, 0, "Attack"); // Bool is true
        attackTrans.hasExitTime = false;

        // Walk -> Attack (Can attack while walking)
        AnimatorStateTransition walkAttackTrans = walkState.AddTransition(attackState);
        walkAttackTrans.AddCondition(AnimatorConditionMode.If, 0, "Attack");
        walkAttackTrans.hasExitTime = false;

        // Attack -> Idle (When Attack Bool is false)
        AnimatorStateTransition stopAttackTrans = attackState.AddTransition(idleState);
        stopAttackTrans.AddCondition(AnimatorConditionMode.IfNot, 0, "Attack");
        stopAttackTrans.hasExitTime = true; // Wait for punch to finish? Or break instant? Usually exit time for attack is good.
        
        // Set Default
        rootStateMachine.defaultState = idleState;

        EditorUtility.SetDirty(controller);
        AssetDatabase.SaveAssets();

        Debug.Log("<color=green>Fixed Zombie Controller! Rewrote State Machine logic completely.</color>");
    }

    static AnimatorState FindState(AnimatorStateMachine sm, string name)
    {
        foreach (var s in sm.states)
        {
            if (s.state.name == name) return s.state;
        }
        return null;
    }

    static void AddParameter(AnimatorController controller, string name, AnimatorControllerParameterType type)
    {
        bool exists = false;
        foreach (var p in controller.parameters)
        {
            if (p.name == name) { exists = true; break; }
        }

        if (!exists)
        {
            controller.AddParameter(name, type);
            Debug.Log($"Added parameter: {name}");
        }
    }
}
