using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ButtonActivatorOnBuild
    {
        static ButtonActivatorOnBuild() => OnBuildUtil.RegisterType<ButtonActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ButtonActivator))]
    public class ButtonActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<ButtonActivator>().ToList());
        }
    }
}
