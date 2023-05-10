using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ToggleActivator : UdonSharpBehaviour
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

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

        public override void Interact()
        {
            State = !State;
            Networking.SetOwner(Networking.LocalPlayer, this.gameObject);
            RequestSerialization();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ToggleActivatorOnBuild
    {
        static ToggleActivatorOnBuild() => OnBuildUtil.RegisterType<ToggleActivator>(OnBuild, order: 0);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ToggleActivator toggleActivator = (ToggleActivator)behaviour;
            toggleActivator.onActivateListeners = new UdonSharpBehaviour[0];
            toggleActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            toggleActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            toggleActivator.onActivateListenerEventNames = new string[0];
            toggleActivator.onDeactivateListenerEventNames = new string[0];
            toggleActivator.onStateChangedListenerEventNames = new string[0];
            toggleActivator.ApplyProxyModifications();
            return true;
        }
    }
    #endif
}
