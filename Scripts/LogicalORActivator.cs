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
    public class LogicalORActivator : UdonSharpBehaviour
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
                if ((bool)activator.GetProgramVariable("state"))
                {
                    State = true;
                    return;
                }
            }
            State = false;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class LogicalORActivatorOnBuild
    {
        static LogicalORActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalORActivator>(FirstOnBuild, order: 0);
            OnBuildUtil.RegisterType<LogicalORActivator>(SecondOnBuild, order: 1);
        }

        private static bool FirstOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalORActivator logicalORActivator = (LogicalORActivator)behaviour;
            logicalORActivator.onActivateListeners = new UdonSharpBehaviour[0];
            logicalORActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            logicalORActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            logicalORActivator.onActivateListenerEventNames = new string[0];
            logicalORActivator.onDeactivateListenerEventNames = new string[0];
            logicalORActivator.onStateChangedListenerEventNames = new string[0];
            if (PrefabUtility.IsPartOfPrefabInstance(logicalORActivator))
                PrefabUtility.RecordPrefabInstancePropertyModifications(logicalORActivator);
            return true;
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalORActivator logicalORActivator = (LogicalORActivator)behaviour;
            foreach (var activator in logicalORActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, logicalORActivator);
            return true;
        }
    }
    #endif
}
