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
    public class ParticleAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback, IAction
    #endif
    {
        public ParticleSystem[] particles;
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            foreach (var particle in particles)
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
        bool IOnBuildCallback.OnBuild() => ActivatorEditorUtil.BasicActionOnBuild(this);
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ParticleAction))]
    public class ParticleActionEditor : ActionEditorBase
    {
        public override void OnInspectorGUI()
        {
            ParticleAction targetAction = this.target as ParticleAction;
            base.OnInspectorGUI();
            if (GUILayout.Button(new GUIContent("Find all child particle systems")))
            {
                targetAction.particles = targetAction.GetComponentsInChildren<ParticleSystem>();
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }
            if (targetAction.particles.Any(p => p == null) && GUILayout.Button(new GUIContent("Remove null particles")))
            {
                targetAction.particles = targetAction.particles.Where(p => p != null).ToArray();
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }
            if (targetAction.particles.Any(p => p != null && p.main.playOnAwake) && GUILayout.Button(new GUIContent("Disable PlayOnAwake")))
            {
                foreach (var particle in targetAction.particles.Where(p => p.main.playOnAwake))
                {
                    var main = particle.main;
                    main.playOnAwake = false;
                    EditorUtility.SetDirty(particle);
                }
            }
        }
    }
    #endif
}
