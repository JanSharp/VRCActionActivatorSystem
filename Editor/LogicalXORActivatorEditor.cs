using System.Linq;
using UdonSharpEditor;
using UnityEditor;

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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(LogicalXORActivator))]
    public class LogicalXORActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<LogicalXORActivator>().ToList());
        }
    }
}
