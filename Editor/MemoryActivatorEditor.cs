using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
using UnityEditor;
using UdonSharpEditor;

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
}
