using System.Linq;
using UdonSharpEditor;
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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(LogicalORActivator))]
    public class LogicalORActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<LogicalORActivator>().ToList());
        }
    }
}
