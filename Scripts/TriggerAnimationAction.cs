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
    public class TriggerAnimationAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IAction
    #endif
    {
        public Animator animator;
        public string triggerParameterName = "trigger";
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            animator.SetTrigger(triggerParameterName);
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        UdonSharpBehaviour IAction.Activator => activator;
        int IAction.ListenerType { get => listenerType; set => listenerType = value; }
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class TriggerAnimationActionOnBuild
    {
        static TriggerAnimationActionOnBuild() => OnBuildUtil.RegisterType<TriggerAnimationAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour) => ActivatorEditorUtil.BasicActionOnBuild((TriggerAnimationAction)behaviour);
    }

    [CustomEditor(typeof(TriggerAnimationAction))]
    public class TriggerAnimationActionEditor : ActionEditorBase
    {
        public override void OnInspectorGUI()
        {
            TriggerAnimationAction targetAction = this.target as TriggerAnimationAction;
            base.OnInspectorGUI();

            if (targetAction.animator == null
                && targetAction.GetComponent<Animator>() != null
                && GUILayout.Button(new GUIContent("Set Animator to this")))
            {
                targetAction.animator = targetAction.GetComponent<Animator>();
                EditorUtility.SetDirty(targetAction);
                if (PrefabUtility.IsPartOfPrefabInstance(targetAction))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(targetAction);
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
