using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ToggleActivator : ActivatorBase
    {
        [SerializeField] private bool onByDefault;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public bool OnByDefault => onByDefault;
#endif

        [UdonSynced]
        [FieldChangeCallback(nameof(SyncedState))]
        [HideInInspector][SerializeField] private bool syncedState;
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
}
