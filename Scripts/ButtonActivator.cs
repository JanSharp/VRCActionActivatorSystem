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
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ButtonActivator : UdonSharpBehaviour
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

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
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PressButton));
        }

        public void PressButton()
        {
            State = true;
            State = false;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ButtonActivatorOnBuild
    {
        static ButtonActivatorOnBuild() => OnBuildUtil.RegisterType<ButtonActivator>(OnBuild, order: 0);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ButtonActivator buttonActivator = (ButtonActivator)behaviour;
            buttonActivator.onActivateListeners = new UdonSharpBehaviour[0];
            buttonActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            buttonActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            buttonActivator.onActivateListenerEventNames = new string[0];
            buttonActivator.onDeactivateListenerEventNames = new string[0];
            buttonActivator.onStateChangedListenerEventNames = new string[0];
            buttonActivator.ApplyProxyModifications();
            return true;
        }
    }
    #endif
}
