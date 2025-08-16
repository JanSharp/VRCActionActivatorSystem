using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class DropItemActionOnBuild
    {
        static DropItemActionOnBuild() => OnBuildUtil.RegisterType<DropItemAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }
}
