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
    public class LogicalNOTActivator : UdonSharpBehaviour
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

        public UdonSharpBehaviour inputActivator;

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

        private void Start()
        {
            OnEvent();
        }

        public void OnEvent()
        {
            State = !(bool)inputActivator.GetProgramVariable("state");
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class LogicalNOTActivatorOnBuild
    {
        static LogicalNOTActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalNOTActivator>(FirstOnBuild, order: 0);
            OnBuildUtil.RegisterType<LogicalNOTActivator>(SecondOnBuild, order: 1);
        }

        private static bool FirstOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalNOTActivator logicalNOTActivator = (LogicalNOTActivator)behaviour;
            logicalNOTActivator.onActivateListeners = new UdonSharpBehaviour[0];
            logicalNOTActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            logicalNOTActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            logicalNOTActivator.onActivateListenerEventNames = new string[0];
            logicalNOTActivator.onDeactivateListenerEventNames = new string[0];
            logicalNOTActivator.onStateChangedListenerEventNames = new string[0];
            if (PrefabUtility.IsPartOfPrefabInstance(logicalNOTActivator))
                PrefabUtility.RecordPrefabInstancePropertyModifications(logicalNOTActivator);
            return true;
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalNOTActivator logicalNOTActivator = (LogicalNOTActivator)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(logicalNOTActivator.inputActivator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, logicalNOTActivator);
            return true;
        }
    }
    #endif
}
