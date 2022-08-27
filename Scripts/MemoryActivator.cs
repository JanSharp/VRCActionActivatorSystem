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
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallbackV2
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

        [SerializeField] private UdonSharpBehaviour activateActivator;
        [SerializeField] private UdonSharpBehaviour resetActivator;
        // to prevent spamming from causing scuff
        private int delayedSerializationCount;
        private const float LateJoinerSyncDelay = 10f;

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

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister()
            {
                OnBuildUtil.RegisterTypeV2<MemoryActivator>(order: 0);
                OnBuildUtil.RegisterTypeV2<MemoryActivator>(order: 1);
            }
        }
        bool IOnBuildCallbackV2.OnBuild(int order)
        {
            if (order == 0)
            {
                onActivateListeners = new UdonSharpBehaviour[0];
                onDeactivateListeners = new UdonSharpBehaviour[0];
                onStateChangedListeners = new UdonSharpBehaviour[0];
                onActivateListenerEventNames = new string[0];
                onDeactivateListenerEventNames = new string[0];
                onStateChangedListenerEventNames = new string[0];
                activateActivator = activateActivator;
                resetActivator = resetActivator;
                this.ApplyProxyModifications();
            }
            else
            {
                ActivatorEditorUtil.AddActivatorToListeners(activateActivator, ActivatorEditorUtil.ListenerEventType.OnActivate, this, nameof(OnActivateEvent));
                if (resetActivator != null)
                {
                    ActivatorEditorUtil.AddActivatorToListeners(resetActivator, ActivatorEditorUtil.ListenerEventType.OnActivate, this, nameof(OnResetActivateEvent));
                    ActivatorEditorUtil.AddActivatorToListeners(resetActivator, ActivatorEditorUtil.ListenerEventType.OnDeactivate, this, nameof(OnResetDeactivateEvent));
                }
            }
            return true;
        }
        #endif
    }
}
