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
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ButtonActivator : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onActivateListeners;
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onDeactivateListeners;
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onStateChangedListeners;

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
                    Send(onActivateListeners);
                else
                    Send(onDeactivateListeners);
                Send(onStateChangedListeners);
            }
        }

        private void Send(UdonSharpBehaviour[] listeners)
        {
            foreach (var listener in listeners)
                listener.SendCustomEvent("OnEvent");
        }

        public override void Interact()
        {
            State = true;
            State = false;
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => JanSharp.OnBuildUtil.RegisterType<ButtonActivator>(order: 0);
        }
        bool IOnBuildCallback.OnBuild()
        {
            onActivateListeners = new UdonSharpBehaviour[0];
            onDeactivateListeners = new UdonSharpBehaviour[0];
            onStateChangedListeners = new UdonSharpBehaviour[0];
            this.ApplyProxyModifications();
            return true;
        }
        #endif
    }
}
