using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class LogicalANDActivatorOnBuild
    {
        static LogicalANDActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<LogicalANDActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<LogicalANDActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(LogicalANDActivator logicalANDActivator)
        {
            foreach (var activator in logicalANDActivator.inputActivators)
                ActivatorEditorUtil.AddActivatorToListeners(activator, ListenerType.OnStateChanged, logicalANDActivator);
            return true;
        }
    }
}
