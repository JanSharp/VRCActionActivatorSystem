using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class PlayerTriggerActivatorOnBuild
    {
        static PlayerTriggerActivatorOnBuild() => OnBuildUtil.RegisterType<PlayerTriggerActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(PlayerTriggerActivator))]
    public class PlayerTriggerActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<PlayerTriggerActivator>().ToList());
        }
    }
}
