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
    public static class ButtonActivatorOnBuild
    {
        static ButtonActivatorOnBuild() => OnBuildUtil.RegisterType<ButtonActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
}
