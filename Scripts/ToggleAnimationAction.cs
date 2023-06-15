using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Reflection;
using System.Linq;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleAnimationAction : UdonSharpBehaviour
    {
        public Animator animator;
        public string boolParameterName = "state";
        public UdonSharpBehaviour activator;

        public void OnEvent()
        {
            animator.SetBool(boolParameterName, (bool)activator.GetProgramVariable("state"));
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP

    [InitializeOnLoad]
    public static class ToggleAnimationActionOnBuild
    {
        static ToggleAnimationActionOnBuild() => OnBuildUtil.RegisterType<ToggleAnimationAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ToggleAnimationAction toggleAnimationAction = (ToggleAnimationAction)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(toggleAnimationAction.activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, toggleAnimationAction);
            return true;
        }
    }

    [CustomEditor(typeof(ToggleAnimationAction))]
    public class ToggleAnimationActionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ToggleAnimationAction targetAction = this.target as ToggleAnimationAction;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targetAction))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            if (targetAction.animator == null
                && targetAction.GetComponent<Animator>() != null
                && GUILayout.Button(new GUIContent("Set Animator to this")))
            {
                targetAction.animator = targetAction.GetComponent<Animator>();
                EditorUtility.SetDirty(targetAction);
            }

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
