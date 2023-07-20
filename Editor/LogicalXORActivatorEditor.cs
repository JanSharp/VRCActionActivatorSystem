using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEditor;
using UdonSharpEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class LogicalXORActivatorOnBuild
    {
        static LogicalXORActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalXORActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalXORActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(LogicalXORActivator logicalXORActivator)
        {
            foreach (var activator in logicalXORActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ListenerType.OnStateChanged, logicalXORActivator);
            return true;
        }
    }
}
