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
    public class PlayerTriggerActivator : UdonSharpBehaviour
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

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

        // Syncing this does not makes sense because it relies on players being inside the trigger zone
        // which should already be the case due to players being synced.
        // If players aren't synced perfectly, then this will be desynced too, and there's no way for
        // this script to accommodate for that. Like even syncing the current state of the boolean
        // whenever someone joins does not guarantee it will stay in sync once players start moving.

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

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            PlayerCount++;
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            PlayerCount--;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class PlayerTriggerActivatorOnBuild
    {
        static PlayerTriggerActivatorOnBuild() => OnBuildUtil.RegisterType<PlayerTriggerActivator>(OnBuild, order: 0);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            PlayerTriggerActivator playerTriggerActivator = (PlayerTriggerActivator)behaviour;
            playerTriggerActivator.onActivateListeners = new UdonSharpBehaviour[0];
            playerTriggerActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            playerTriggerActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            playerTriggerActivator.onActivateListenerEventNames = new string[0];
            playerTriggerActivator.onDeactivateListenerEventNames = new string[0];
            playerTriggerActivator.onStateChangedListenerEventNames = new string[0];
            if (PrefabUtility.IsPartOfPrefabInstance(playerTriggerActivator))
                PrefabUtility.RecordPrefabInstancePropertyModifications(playerTriggerActivator);
            return true;
        }
    }
    #endif
}
