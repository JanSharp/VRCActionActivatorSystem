﻿using UdonSharp;
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
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onActivateListeners;
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onDeactivateListeners;
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onStateChangedListeners;
        [SerializeField] [HideInInspector] private string[] onActivateListenerEventNames;
        [SerializeField] [HideInInspector] private string[] onDeactivateListenerEventNames;
        [SerializeField] [HideInInspector] private string[] onStateChangedListenerEventNames;

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

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<ItemTriggerActivator>(order: 0);
        }
        bool IOnBuildCallback.OnBuild()
        {
            onActivateListeners = new UdonSharpBehaviour[0];
            onDeactivateListeners = new UdonSharpBehaviour[0];
            onStateChangedListeners = new UdonSharpBehaviour[0];
            onActivateListenerEventNames = new string[0];
            onDeactivateListenerEventNames = new string[0];
            onStateChangedListenerEventNames = new string[0];
            this.ApplyProxyModifications();
            return true;
        }
        #endif
    }
}