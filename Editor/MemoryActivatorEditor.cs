using System.Linq;
using UdonSharpEditor;
using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class MemoryActivatorOnBuild
    {
        static MemoryActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<MemoryActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
            OnBuildUtil.RegisterType<MemoryActivator>(SecondOnBuild, order: 1);
        }

        private static bool SecondOnBuild(MemoryActivator memoryActivator)
        {
            ActivatorEditorUtil.AddActivatorToListeners(
                memoryActivator.activateActivator,
                ListenerType.OnActivate,
                memoryActivator,
                nameof(MemoryActivator.OnActivateEvent)
            );
            if (memoryActivator.resetActivator != null)
            {
                ActivatorEditorUtil.AddActivatorToListeners(
                    memoryActivator.resetActivator,
                    ListenerType.OnActivate,
                    memoryActivator,
                    nameof(MemoryActivator.OnResetActivateEvent)
                );
                ActivatorEditorUtil.AddActivatorToListeners(
                    memoryActivator.resetActivator,
                    ListenerType.OnDeactivate,
                    memoryActivator,
                    nameof(MemoryActivator.OnResetDeactivateEvent)
                );
            }
            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(MemoryActivator))]
    public class MemoryActivatorEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            serializedObject.Update();
            DrawPropertiesExcluding(serializedObject, "m_Script");
            serializedObject.ApplyModifiedProperties();
            ActivatorEditorUtil.OnActivatorInspectorGUI(targets.Cast<MemoryActivator>().ToList());
        }
    }
}
