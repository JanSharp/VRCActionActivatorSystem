using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ToggleActivator : ActivatorBase
    {
        [UdonSynced]
        [FieldChangeCallback(nameof(SyncedState))]
        private bool syncedState;
        private bool SyncedState
        {
            get => syncedState;
            set
            {
                State = value;
                syncedState = value;
            }
        }

        public override void Interact()
        {
            SyncedState = !SyncedState;
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            RequestSerialization();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ToggleActivatorOnBuild
    {
        static ToggleActivatorOnBuild() => OnBuildUtil.RegisterType<ToggleActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
    #endif
}
