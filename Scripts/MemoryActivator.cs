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
    public class MemoryActivator : UdonSharpBehaviour
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

        public UdonSharpBehaviour activateActivator;
        public UdonSharpBehaviour resetActivator;
        // to prevent spamming from causing scuff
        private int delayedSerializationCount;
        private const float LateJoinerSyncDelay = 10f;

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

                // syncing the state of this even though the state should already be
                // evaluated by every other client based on inputs.
                // however since this is storing a state the order in which activators got synced
                // could affect the stored state differently on different clients. So to increase
                // the chance of everyone having the same state regardless of order of events
                // we're syncing the state based on the owner's state just a bit delayed
                if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                {
                    delayedSerializationCount++;
                    SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 1f);
                }
            }
        }

        private void Send(UdonSharpBehaviour[] listeners, string[] listenerEventNames)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].SendCustomEvent(listenerEventNames[i]);
        }

        public void OnActivateEvent()
        {
            if (resetActivator == null || !(bool)resetActivator.GetProgramVariable("state"))
                State = true;
        }

        public void OnResetActivateEvent()
        {
            State = false;
        }

        public void OnResetDeactivateEvent()
        {
            if ((bool)activateActivator.GetProgramVariable("state"))
                State = true;
        }

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
            {
                delayedSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), LateJoinerSyncDelay);
            }
        }

        // for good measure, in case the current owner leaves just after a player joined
        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (player.isLocal)
            {
                delayedSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), LateJoinerSyncDelay);
            }
        }

        public void RequestSerializationDelayed()
        {
            if ((--delayedSerializationCount) != 0)
                return;
            RequestSerialization();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class MemoryActivatorOnBuild
    {
        static MemoryActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<MemoryActivator>(FirstOnBuild, order: 0);
            OnBuildUtil.RegisterType<MemoryActivator>(SecondOnBuild, order: 1);
        }

        private static bool FirstOnBuild(UdonSharpBehaviour behaviour)
        {
            MemoryActivator memoryActivator = (MemoryActivator)behaviour;
            memoryActivator.onActivateListeners = new UdonSharpBehaviour[0];
            memoryActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            memoryActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            memoryActivator.onActivateListenerEventNames = new string[0];
            memoryActivator.onDeactivateListenerEventNames = new string[0];
            memoryActivator.onStateChangedListenerEventNames = new string[0];
            if (PrefabUtility.IsPartOfPrefabInstance(memoryActivator))
                PrefabUtility.RecordPrefabInstancePropertyModifications(memoryActivator);
            return true;
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            MemoryActivator memoryActivator = (MemoryActivator)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(
                memoryActivator.activateActivator,
                ActivatorEditorUtil.ListenerEventType.OnActivate,
                memoryActivator,
                nameof(MemoryActivator.OnActivateEvent)
            );
            if (memoryActivator.resetActivator != null)
            {
                ActivatorEditorUtil.AddActivatorToListeners(
                    memoryActivator.resetActivator,
                    ActivatorEditorUtil.ListenerEventType.OnActivate,
                    memoryActivator,
                    nameof(MemoryActivator.OnResetActivateEvent)
                );
                ActivatorEditorUtil.AddActivatorToListeners(
                    memoryActivator.resetActivator,
                    ActivatorEditorUtil.ListenerEventType.OnDeactivate,
                    memoryActivator,
                    nameof(MemoryActivator.OnResetDeactivateEvent)
                );
            }
            return true;
        }
    }
    #endif
}
