using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

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
}
