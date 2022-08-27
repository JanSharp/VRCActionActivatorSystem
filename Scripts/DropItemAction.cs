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
    public class DropItemAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback, IAction
    #endif
    {
        [SerializeField] private VRC_Pickup pickup;
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            if (pickup.IsHeld && pickup.currentPlayer.isLocal)
                pickup.Drop();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        UdonSharpBehaviour IAction.Activator => activator;
        int IAction.ListenerType { get => listenerType; set => listenerType = value; }

        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<DropItemAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild() => ActivatorEditorUtil.BasicActionOnBuild(this);
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(DropItemAction))]
    public class DropItemActionEditor : ActionEditorBase { }
    #endif
}
