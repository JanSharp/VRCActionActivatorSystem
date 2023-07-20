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
    public static class DropItemActionOnBuild
    {
        static DropItemActionOnBuild() => OnBuildUtil.RegisterType<DropItemAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }
}
