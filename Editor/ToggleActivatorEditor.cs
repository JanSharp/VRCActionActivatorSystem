using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ToggleActivatorOnBuild
    {
        static ToggleActivatorOnBuild() => OnBuildUtil.RegisterType<ToggleActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
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
