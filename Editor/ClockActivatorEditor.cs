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
            OnBuildUtil.RegisterType<ClockActivator>(FirstOnBuild, order: 0);
            OnBuildUtil.RegisterType<ClockActivator>(SecondOnBuild, order: 1);
        }

        private static bool FirstOnBuild(ClockActivator clockActivator)
        {
            UpdateManager updateManager = GameObject.Find("/UpdateManager")?.GetComponent<UpdateManager>();
            if (updateManager == null)
            {
                Debug.LogError("ClockActivator requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    clockActivator);
                return false;
            }
            if (clockActivator.updateManager != updateManager)
            {
                SerializedObject activatorProxy = new SerializedObject(clockActivator);
                activatorProxy.FindProperty(nameof(ClockActivator.updateManager)).objectReferenceValue = updateManager;
                activatorProxy.ApplyModifiedProperties();
            }
            return ActivatorEditorUtil.ActivatorOnBuildBase(clockActivator);
        }

        private static bool SecondOnBuild(ClockActivator clockActivator)
        {
            ActivatorEditorUtil.AddActivatorToListeners(clockActivator.inputActivator, ListenerType.OnStateChanged, clockActivator);
            return true;
        }
    }
}
