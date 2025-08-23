using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ToggleActivatorOnBuild
    {
        static ToggleActivatorOnBuild() => OnBuildUtil.RegisterType<ToggleActivator>(OnBuild, order: 0);

        private static bool OnBuild(ToggleActivator toggleActivator)
        {
            if (!ActivatorEditorUtil.ActivatorOnBuildBase(toggleActivator))
                return false;
            SerializedObject so = new SerializedObject(toggleActivator);
            so.FindProperty("syncedState").boolValue = toggleActivator.OnByDefault;
            so.ApplyModifiedProperties();
            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ToggleActivator))]
    public class ToggleActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<ToggleActivator>().ToList());
        }
    }
}
