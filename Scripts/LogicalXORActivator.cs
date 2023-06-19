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
    public class LogicalXORActivator : ActivatorBase
    {
        public UdonSharpBehaviour[] inputActivators;

        public void OnEvent()
        {
            bool active = false;
            foreach (var activator in inputActivators)
            {
                if ((bool)activator.GetProgramVariable("state"))
                {
                    if (active)
                    {
                        State = false;
                        return;
                    }
                    active = true;
                }
            }
            State = active;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class LogicalXORActivatorOnBuild
    {
        static LogicalXORActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalXORActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalXORActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalXORActivator logicalXORActivator = (LogicalXORActivator)behaviour;
            foreach (var activator in logicalXORActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, logicalXORActivator);
            return true;
        }
    }
    #endif
}
