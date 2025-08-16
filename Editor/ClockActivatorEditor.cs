using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ClockActivatorOnBuild
    {
        static ClockActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<ClockActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<ClockActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(ClockActivator clockActivator)
        {
            ActivatorEditorUtil.AddActivatorToListeners(clockActivator.inputActivator, ListenerType.OnStateChanged, clockActivator);
            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ClockActivator))]
    public class ClockActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<ClockActivator>().ToList());
        }
    }
}
