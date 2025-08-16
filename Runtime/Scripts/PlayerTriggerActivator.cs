using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTriggerActivator : ActivatorBase
    {
        [Tooltip("Only react to the local player?")]
        [SerializeField] private bool localOnly;

        private int playerCount;
        private int PlayerCount
        {
            get => playerCount;
            set
            {
                playerCount = value;
                State = value != 0;
            }
        }

        // Syncing the state does not makes sense because it relies on players being inside the trigger zone
        // which should already be the case due to players being synced.
        // If players aren't synced perfectly, then this will be desynced too, and there's no way for
        // this script to accommodate for that. Like even syncing the current state of the boolean
        // whenever someone joins does not guarantee it will stay in sync once players start moving.

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (localOnly && !player.isLocal)
                return;
            PlayerCount++;
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (localOnly && !player.isLocal)
                return;
            PlayerCount--;
        }
    }
}
