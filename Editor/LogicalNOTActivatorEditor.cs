using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class LogicalNOTActivatorOnBuild
    {
        static LogicalNOTActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalNOTActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalNOTActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(LogicalNOTActivator logicalNOTActivator)
        {
            ActivatorEditorUtil.AddActivatorToListeners(logicalNOTActivator.inputActivator, ListenerType.OnStateChanged, logicalNOTActivator);
            return true;
        }
    }
}
