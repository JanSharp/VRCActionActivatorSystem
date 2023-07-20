using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ItemTriggerActivator : ActivatorBase
    {
        [SerializeField] private string containedItemName;
        private int itemCount;
        private int ItemCount
        {
            get => itemCount;
            set
            {
                itemCount = value;
                State = value != 0;
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

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class OnBuildRegister
    {
        static OnBuildRegister() => OnBuildUtil.RegisterType<ItemTriggerActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
    #endif
}
