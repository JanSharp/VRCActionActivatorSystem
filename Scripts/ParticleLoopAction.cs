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
    public class ParticleLoopAction : UdonSharpBehaviour
    {
        public ParticleSystem[] particles;
        public UdonSharpBehaviour activator;

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                foreach (var particle in particles)
                    particle.Play();
            else
                foreach (var particle in particles)
                    particle.Stop();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP

    [InitializeOnLoad]
    public static class ParticleLoopActionOnBuild
    {
        static ParticleLoopActionOnBuild() => OnBuildUtil.RegisterType<ParticleLoopAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ParticleLoopAction particleLoopAction = (ParticleLoopAction)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(particleLoopAction.activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, particleLoopAction);
            return true;
        }
    }

    [CustomEditor(typeof(ParticleLoopAction))]
    public class ParticleLoopActionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            ParticleLoopAction targetAction = this.target as ParticleLoopAction;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targetAction))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            if (GUILayout.Button(new GUIContent("Find Particle Systems", "Searches on this component and its children. Overwrites anything previously set.")))
            {
                targetAction.particles = targetAction.GetComponentsInChildren<ParticleSystem>();
                EditorUtility.SetDirty(targetAction);
                if (PrefabUtility.IsPartOfPrefabInstance(targetAction))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(targetAction);
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p == null)
                && GUILayout.Button(new GUIContent("Remove null particles")))
            {
                targetAction.particles = targetAction.particles.Where(p => p != null).ToArray();
                EditorUtility.SetDirty(targetAction);
                if (PrefabUtility.IsPartOfPrefabInstance(targetAction))
                    PrefabUtility.RecordPrefabInstancePropertyModifications(targetAction);
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
                    if (PrefabUtility.IsPartOfPrefabInstance(particle))
                        PrefabUtility.RecordPrefabInstancePropertyModifications(particle);
                }
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p != null && !p.main.loop)
                && GUILayout.Button(new GUIContent("Make all Particle Systems loop")))
            {
                foreach (var particle in targetAction.particles.Where(p => p != null && !p.main.loop))
                {
                    var main = particle.main;
                    main.loop = true;
                    EditorUtility.SetDirty(particle);
                    if (PrefabUtility.IsPartOfPrefabInstance(particle))
                        PrefabUtility.RecordPrefabInstancePropertyModifications(particle);
                }
            }
        }
    }
    #endif
}
