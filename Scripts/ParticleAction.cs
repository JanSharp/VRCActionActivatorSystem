using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Reflection;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback, IAction
    #endif
    {
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            particle.Play();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        UdonSharpBehaviour IAction.Activator => activator;
        int IAction.ListenerType { get => listenerType; set => listenerType = value; }

        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<ParticleAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild() => ActivatorEditorUtil.ActionOnBuild(this);
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ParticleAction))]
    public class ParticleActionEditor : ActionEditorBase { }
    #endif
}
