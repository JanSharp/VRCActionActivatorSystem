using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class MovementActionOnBuild
    {
        static MovementActionOnBuild() => OnBuildUtil.RegisterType<MovementAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }
}
