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
        , IAction
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
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class DropItemActionOnBuild
    {
        static DropItemActionOnBuild() => OnBuildUtil.RegisterType<DropItemAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour) => ActivatorEditorUtil.BasicActionOnBuild((DropItemAction)behaviour);
    }

    [CustomEditor(typeof(DropItemAction))]
    public class DropItemActionEditor : ActionEditorBase { }
    #endif
}
