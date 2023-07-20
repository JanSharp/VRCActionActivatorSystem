using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ButtonActivator : ActivatorBase
    {
        public override void Interact()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PressButton));
        }

        public void PressButton()
        {
            State = true;
            State = false;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ButtonActivatorOnBuild
    {
        static ButtonActivatorOnBuild() => OnBuildUtil.RegisterType<ButtonActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
    #endif
}
