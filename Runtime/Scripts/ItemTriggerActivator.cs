using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ItemTriggerActivator : ActivatorBase
    {
        [SerializeField] private string containedItemName;
        [Tooltip("This defines how many relevant items are already in the trigger in the default state of "
            + "the scene.\n"
            + "If there are multiple colliders on this game object, an item colliding with multiple of said "
            + "colliders counts multiple times.\n"
            + "Setting this to a value greater than the amount of items actually within the this "
            + "Item Trigger Activator will result in it not turning off even when no items are in it.")]
        [Min(0)]
        [SerializeField] private int initialItemCount; // Called "initial" just to display as such in the inspector.
#if UNITY_EDITOR && !COMPILER_UDONSHARP
        public int ItemCount
#else
        private int ItemCount
#endif
        {
            get => initialItemCount;
            set
            {
                initialItemCount = System.Math.Max(0, value);
                State = initialItemCount != 0;
            }
        }

        // Syncing the state does not makes sense because it relies on items being inside the trigger zone
        // which should already be the case due to items being synced.
        // If items aren't synced perfectly, then this will be desynced too, and there's no way for
        // this script to accommodate for that. Like even syncing the current state of the boolean
        // whenever someone joins does not guarantee it will stay in sync once items start moving.

        private void OnTriggerEnter(Collider other)
        {
            if (other.name.Contains(containedItemName))
                ItemCount++;
        }

        private void OnTriggerExit(Collider other)
        {
            if (other.name.Contains(containedItemName))
                ItemCount--;
        }
    }
}
