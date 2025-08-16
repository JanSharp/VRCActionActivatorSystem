using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ToggleActivatorOnBuild
    {
        static ToggleActivatorOnBuild() => OnBuildUtil.RegisterType<ToggleActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
}
