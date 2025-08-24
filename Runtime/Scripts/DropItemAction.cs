using UdonSharp;
using UnityEngine;
using VRC.SDKBase;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class DropItemAction : ConfigurableActionBase
    {
        [SerializeField] private VRC_Pickup pickup;

        public void OnEvent()
        {
            if (pickup != null && pickup.IsHeld && pickup.currentPlayer.isLocal)
                pickup.Drop();
        }
    }
}
