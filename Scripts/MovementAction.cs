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
        , IOnBuildCallback, IAction
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

        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<MovementAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild() => ActivatorEditorUtil.BasicActionOnBuild(this);
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(MovementAction))]
    public class MovementActionEditor : ActionEditorBase { }
    #endif
}
