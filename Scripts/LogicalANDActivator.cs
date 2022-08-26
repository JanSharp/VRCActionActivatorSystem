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
    public class LogicalANDActivator : UdonSharpBehaviour
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

        public UdonSharpBehaviour[] inputActivators;

        public void OnEvent()
        {
            foreach (var activator in inputActivators)
            {
                if (!(bool)activator.GetProgramVariable("state"))
                {
                    State = false;
                    return;
                }
            }
            State = true;
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister()
            {
                OnBuildUtil.RegisterTypeV2<LogicalANDActivator>(order: 0);
                OnBuildUtil.RegisterTypeV2<LogicalANDActivator>(order: 1);
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
                this.ApplyProxyModifications();
            }
            else
            {
                foreach (var activator in inputActivators)
                    ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, this);
            }
            return true;
        }
        #endif
    }
}
