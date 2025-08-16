using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class OnBuildRegister
    {
        static OnBuildRegister() => OnBuildUtil.RegisterType<ItemTriggerActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ItemTriggerActivator))]
    public class ItemTriggerActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<ItemTriggerActivator>().ToList());
        }
    }
}
