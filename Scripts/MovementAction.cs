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
    public class MovementAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IAction
    #endif
    {
        [SerializeField] private ObjectPositionSync targetObject;
        [SerializeField] private Vector3 movementOnEvent;
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            targetObject.TargetPosition += movementOnEvent;
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        UdonSharpBehaviour IAction.Activator => activator;
        int IAction.ListenerType { get => listenerType; set => listenerType = value; }
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class MovementActionOnBuild
    {
        static MovementActionOnBuild() => OnBuildUtil.RegisterType<MovementAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour) => ActivatorEditorUtil.BasicActionOnBuild((MovementAction)behaviour);
    }

    [CustomEditor(typeof(MovementAction))]
    public class MovementActionEditor : ActionEditorBase { }
    #endif
}
