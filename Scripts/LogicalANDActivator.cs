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
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

        public UdonSharpBehaviour[] inputActivators;

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
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class LogicalANDActivatorOnBuild
    {
        static LogicalANDActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalANDActivator>(FirstOnBuild, order: 0);
            OnBuildUtil.RegisterType<LogicalANDActivator>(SecondOnBuild, order: 1);
        }

        private static bool FirstOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalANDActivator logicalANDActivator = (LogicalANDActivator)behaviour;
            logicalANDActivator.onActivateListeners = new UdonSharpBehaviour[0];
            logicalANDActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            logicalANDActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            logicalANDActivator.onActivateListenerEventNames = new string[0];
            logicalANDActivator.onDeactivateListenerEventNames = new string[0];
            logicalANDActivator.onStateChangedListenerEventNames = new string[0];
            if (PrefabUtility.IsPartOfPrefabInstance(logicalANDActivator))
                PrefabUtility.RecordPrefabInstancePropertyModifications(logicalANDActivator);
            return true;
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalANDActivator logicalANDActivator = (LogicalANDActivator)behaviour;
            foreach (var activator in logicalANDActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, logicalANDActivator);
            return true;
        }
    }
    #endif
}
