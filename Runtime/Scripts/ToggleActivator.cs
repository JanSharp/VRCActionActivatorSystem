using UdonSharp;
using VRC.SDKBase;

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
}
