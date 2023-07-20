using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
using System.Collections.Generic;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TriggerAnimationAction : ConfigurableActionBase
    {
        public Animator animator;
        public string triggerParameterName = "trigger";

        public void OnEvent()
        {
            animator.SetTrigger(triggerParameterName);
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class TriggerAnimationActionOnBuild
    {
        static TriggerAnimationActionOnBuild() => OnBuildUtil.RegisterType<TriggerAnimationAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(TriggerAnimationAction))]
    public class TriggerAnimationActionEditor : Editor
    {
        private static void SetAnimatorToThis(IEnumerable<TriggerAnimationAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                actionProxy.FindProperty(nameof(TriggerAnimationAction.animator)).objectReferenceValue = action.GetComponent<Animator>();
                actionProxy.ApplyModifiedProperties();
            }
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targets))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            ActionEditorUtil.ConditionalButton(new GUIContent("Set Animator to this"),
                targets.Cast<TriggerAnimationAction>().Where(a => a.animator == null && a.GetComponent<Animator>() != null),
                SetAnimatorToThis);

            // an attempt at validating the animator's parameters. This now says it doesn't find the parameter even though it exists. Weird.
            // if (targetAction.animator != null)
            // {
            //     var param = targetAction.animator.parameters.FirstOrDefault(p => p.name == targetAction.boolParameterName);
            //     if (param == null)
            //     {
            //         EditorGUILayout.Space();
            //         ///cSpell:ignore erroricon
            //         // GUILayout.Label(EditorGUIUtility.TrIconContent("console.erroricon", $"The referenced animator does not have a '{targetAction.boolParameterName}' parameter."));
            //         GUILayout.Label(new GUIContent($"The referenced animator does not have a '{targetAction.boolParameterName}' parameter."));
            //     }
            //     else if (param.type != AnimatorControllerParameterType.Bool)
            //     {
            //         EditorGUILayout.Space();
            //         GUILayout.Label(new GUIContent($"The referenced animator's '{targetAction.boolParameterName}' parameter must be a bool parameter."));
            //     }
            // }
        }
    }
    #endif
}
