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
    public class LogicalANDActivator : ActivatorBase
    {
        public ActivatorBase[] inputActivators;

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
            OnBuildUtil.RegisterType<LogicalANDActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalANDActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            LogicalANDActivator logicalANDActivator = (LogicalANDActivator)behaviour;
            foreach (var activator in logicalANDActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ListenerType.OnStateChanged, logicalANDActivator);
            return true;
        }
    }
    #endif
}
