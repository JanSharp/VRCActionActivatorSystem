using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DropItemAction : ConfigurableActionBase
    {
        [SerializeField] private VRC_Pickup pickup;

        public void OnEvent()
        {
            if (pickup.IsHeld && pickup.currentPlayer.isLocal)
                pickup.Drop();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class DropItemActionOnBuild
    {
        static DropItemActionOnBuild() => OnBuildUtil.RegisterType<DropItemAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour) => ActivatorEditorUtil.BasicActionOnBuild((DropItemAction)behaviour);
    }
    #endif
}
