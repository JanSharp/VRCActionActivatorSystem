using System.Linq;
using UdonSharpEditor;
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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(LogicalANDActivator))]
    public class LogicalANDActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<LogicalANDActivator>().ToList());
        }
    }
}
