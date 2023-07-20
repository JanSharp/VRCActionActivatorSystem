using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
using System.Collections.Generic;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ToggleAnimationActionOnBuild
    {
        static ToggleAnimationActionOnBuild() => OnBuildUtil.RegisterType<ToggleAnimationAction>(OnBuild, order: 1);

        private static bool OnBuild(ToggleAnimationAction toggleAnimationAction)
        {
            ActivatorEditorUtil.AddActivatorToListeners(toggleAnimationAction.activator, ListenerType.OnStateChanged, toggleAnimationAction);
            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ToggleAnimationAction))]
    public class ToggleAnimationActionEditor : Editor
    {
        private static void SetAnimatorToThis(IEnumerable<ToggleAnimationAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                actionProxy.FindProperty(nameof(ToggleAnimationAction.animator)).objectReferenceValue = action.GetComponent<Animator>();
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
                targets.Cast<ToggleAnimationAction>().Where(a => a.animator == null && a.GetComponent<Animator>() != null),
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
}
