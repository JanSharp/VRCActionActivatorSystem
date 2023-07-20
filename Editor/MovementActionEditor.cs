using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class MovementActionOnBuild
    {
        static MovementActionOnBuild() => OnBuildUtil.RegisterType<MovementAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }
}
