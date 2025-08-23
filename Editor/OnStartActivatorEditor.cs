using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class OnStartActivatorOnBuild
    {
        static OnStartActivatorOnBuild() => OnBuildUtil.RegisterType<OnStartActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(OnStartActivator))]
    public class OnStartActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<OnStartActivator>().ToList());
        }
    }
}
