using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class LogicalORActivatorOnBuild
    {
        static LogicalORActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalORActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalORActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(LogicalORActivator logicalORActivator)
        {
            foreach (var activator in logicalORActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ListenerType.OnStateChanged, logicalORActivator);
            return true;
        }
    }
}
