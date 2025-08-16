using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ButtonActivatorOnBuild
    {
        static ButtonActivatorOnBuild() => OnBuildUtil.RegisterType<ButtonActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
}
