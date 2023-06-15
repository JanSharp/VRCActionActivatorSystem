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
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
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

        [UdonSynced]
        [FieldChangeCallback(nameof(State))]
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
            return true;
        }
    }
    #endif
}
