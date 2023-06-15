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
    public class ItemTriggerActivator : UdonSharpBehaviour
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

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

        // Syncing this does not makes sense because it relies on items being inside the trigger zone
        // which should already be the case due to items being synced.
        // If items aren't synced perfectly, then this will be desynced too, and there's no way for
        // this script to accommodate for that. Like even syncing the current state of the boolean
        // whenever someone joins does not guarantee it will stay in sync once items start moving.

        private bool state;
        private bool State
        {
            get => state;
            set
            {
                if (value == state)
                    return;
                state = value;
                if (value)
                    Send(onActivateListeners, onActivateListenerEventNames);
                else
                    Send(onDeactivateListeners, onDeactivateListenerEventNames);
                Send(onStateChangedListeners, onStateChangedListenerEventNames);
            }
        }

        private void Send(UdonSharpBehaviour[] listeners, string[] listenerEventNames)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].SendCustomEvent(listenerEventNames[i]);
        }

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
        static OnBuildRegister() => OnBuildUtil.RegisterType<ItemTriggerActivator>(OnBuild, order: 0);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ItemTriggerActivator itemTriggerActivator = (ItemTriggerActivator)behaviour;
            itemTriggerActivator.onActivateListeners = new UdonSharpBehaviour[0];
            itemTriggerActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            itemTriggerActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            itemTriggerActivator.onActivateListenerEventNames = new string[0];
            itemTriggerActivator.onDeactivateListenerEventNames = new string[0];
            itemTriggerActivator.onStateChangedListenerEventNames = new string[0];
            if (PrefabUtility.IsPartOfPrefabInstance(itemTriggerActivator))
                PrefabUtility.RecordPrefabInstancePropertyModifications(itemTriggerActivator);
            return true;
        }
    }
    #endif
}
