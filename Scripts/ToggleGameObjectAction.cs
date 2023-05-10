using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Reflection;
using System.Linq;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleGameObjectAction : UdonSharpBehaviour
    {
        [SerializeField] private GameObject[] gameObjects;
        public UdonSharpBehaviour activator;

        public void OnEvent()
        {
            foreach (var obj in gameObjects)
                obj.SetActive(!obj.activeSelf);
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ToggleGameObjectActionOnBuild
    {
        static ToggleGameObjectActionOnBuild() => OnBuildUtil.RegisterType<ToggleGameObjectAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ToggleGameObjectAction toggleGameObjectAction = (ToggleGameObjectAction)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(toggleGameObjectAction.activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, toggleGameObjectAction);
            return true;
        }
    }
    #endif
}
