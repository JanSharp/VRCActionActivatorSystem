using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class MemoryActivator : ActivatorBase
    {
        public ActivatorBase activateActivator;
        public ActivatorBase resetActivator;
        [Tooltip("Should this Memory Activator be active in the world's initial state?\n"
            + "If the Reset Activator has an Evaluated Initial State of on, this Memory Activator will be "
            + "off regardless of this option here.\n"
            + "When On By Default is true, the Activate Activator being null will not generate an error.")]
        [SerializeField] private bool onByDefault;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public bool OnByDefault => onByDefault;
#endif
        // to prevent spamming from causing scuff
        private int requestSerializationCount = 0;
        private bool waitingForOwnerToSendData = false;
        private const float LateJoinerSyncDelay = 10f;

        [HideInInspector]
        [SerializeField]
        [UdonSynced]
        [FieldChangeCallback(nameof(SyncedState))]
        private bool syncedState;
        private bool SyncedState
        {
            get => syncedState;
            set
            {
                // Set the state that's on the base class, raising activator events.
                State = value;

                if (value == syncedState)
                    return;
                syncedState = value;

                // syncing the state of this even though the state should already be
                // evaluated by every other client based on inputs.
                // however since this is storing a state the order in which activators got synced
                // could affect the stored state differently on different clients. So to increase
                // the chance of everyone having the same state regardless of order of events
                // we're syncing the state based on the owner's state just a bit delayed
                if (Networking.IsOwner(this.gameObject))
                {
                    requestSerializationCount++;
                    SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 1f);
                }
            }
        }

        public void OnActivateEvent()
        {
            if (resetActivator == null || !(bool)resetActivator.GetProgramVariable("state"))
                SyncedState = true;
        }

        public void OnResetActivateEvent()
        {
            SyncedState = false;
        }

        public void OnResetDeactivateEvent()
        {
            if (activateActivator != null && (bool)activateActivator.GetProgramVariable("state"))
                SyncedState = true;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), LateJoinerSyncDelay);
            }
            else
            {
                waitingForOwnerToSendData = true;
            }
        }

        // for good measure, in case the current owner leaves just after a player joined
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (waitingForOwnerToSendData && Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), LateJoinerSyncDelay);
            }
        }

        public void RequestSerializationDelayed()
        {
            if ((--requestSerializationCount) == 0)
                RequestSerialization();
        }

        public override void OnDeserialization()
        {
            waitingForOwnerToSendData = false;
        }
    }
}
