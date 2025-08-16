using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class PlayerTriggerActivatorOnBuild
    {
        static PlayerTriggerActivatorOnBuild() => OnBuildUtil.RegisterType<PlayerTriggerActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
}
