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
}
