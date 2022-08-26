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
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class PlayerTriggerActivator : UdonSharpBehaviour
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
                
                if (currentlyInteractingPlayer != null && currentlyInteractingPlayer.isLocal)
                    Sync();
            }
        }

        private void Send(UdonSharpBehaviour[] listeners, string[] listenerEventNames)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].SendCustomEvent(listenerEventNames[i]);
        }

        // an ugly way of passing a "parameter" to a property setter. I hate it but I also
        // don't want to deviate from the copy pasted code too much
        private VRCPlayerApi currentlyInteractingPlayer;
        private bool localPlayerIsInTrigger;
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

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            if (player.isLocal)
                localPlayerIsInTrigger = true;
            currentlyInteractingPlayer = player;
            PlayerCount++;
            currentlyInteractingPlayer = null;
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            if (player.isLocal)
                localPlayerIsInTrigger = false;
            currentlyInteractingPlayer = player;
            PlayerCount--;
            currentlyInteractingPlayer = null;
        }

        public override void OnDeserialization()
        {
            if (!State && localPlayerIsInTrigger)
            {
                State = true;
                SendCustomEventDelayedFrames(nameof(Sync), 1);
            }
        }

        public void Sync()
        {
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            RequestSerialization();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<PlayerTriggerActivator>(order: 0);
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