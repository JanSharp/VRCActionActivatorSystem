using System.Linq;
using UdonSharpEditor;
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

    [CanEditMultipleObjects]
    [CustomEditor(typeof(LogicalNOTActivator))]
    public class LogicalNOTActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<LogicalNOTActivator>().ToList());
        }
    }
}
