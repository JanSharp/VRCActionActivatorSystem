using UnityEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class OnBuildRegister
    {
        static OnBuildRegister() => OnBuildUtil.RegisterType<ItemTriggerActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
}
