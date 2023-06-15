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
        , IAction
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
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ParticleActionOnBuild
    {
        static ParticleActionOnBuild() => OnBuildUtil.RegisterType<ParticleAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour) => ActivatorEditorUtil.BasicActionOnBuild((ParticleAction)behaviour);
    }

    [CustomEditor(typeof(ParticleAction))]
    public class ParticleActionEditor : ActionEditorBase
    {
        public override void OnInspectorGUI()
        {
            ParticleAction targetAction = this.target as ParticleAction;
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Find Particle Systems", "Searches on this component and its children. Overwrites anything previously set.")))
            {
                targetAction.particles = targetAction.GetComponentsInChildren<ParticleSystem>();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p == null)
                && GUILayout.Button(new GUIContent("Remove null particles")))
            {
                targetAction.particles = targetAction.particles.Where(p => p != null).ToArray();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p != null && p.main.playOnAwake)
                && GUILayout.Button(new GUIContent("Disable PlayOnAwake")))
            {
                foreach (var particle in targetAction.particles.Where(p => p != null && p.main.playOnAwake))
                {
                    var main = particle.main;
                    main.playOnAwake = false;
                    EditorUtility.SetDirty(particle);
                }
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p != null && p.main.loop)
                && GUILayout.Button(new GUIContent("Make all Particle Systems not loop")))
            {
                foreach (var particle in targetAction.particles.Where(p => p != null && p.main.loop))
                {
                    var main = particle.main;
                    main.loop = false;
                    EditorUtility.SetDirty(particle);
                }
            }
        }
    }
    #endif
}
