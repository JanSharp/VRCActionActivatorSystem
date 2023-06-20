using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
using System.Collections.Generic;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleAction : ActionBase
    {
        public ParticleSystem[] particles;

        public void OnEvent()
        {
            foreach (var particle in particles)
                particle.Play();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ParticleActionOnBuild
    {
        static ParticleActionOnBuild() => OnBuildUtil.RegisterType<ParticleAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour) => ActivatorEditorUtil.BasicActionOnBuild((ParticleAction)behaviour);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ParticleAction))]
    public class ParticleActionEditor : Editor
    {
        private static void FindParticleSystems(IEnumerable<ParticleAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                ActionEditorUtil.SetArrayProperty(
                    actionProxy.FindProperty(nameof(ParticleAction.particles)),
                    action.GetComponentsInChildren<ParticleSystem>(),
                    (p, v) => p.objectReferenceValue = v);
                actionProxy.ApplyModifiedProperties();
            }
        }

        private static void RemoveNullParticles(IEnumerable<ParticleAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                ActionEditorUtil.SetArrayProperty(
                    actionProxy.FindProperty(nameof(ParticleAction.particles)),
                    action.particles.Where(p => p != null).ToArray(),
                    (p, v) => p.objectReferenceValue = v);
                actionProxy.ApplyModifiedProperties();
            }
        }

        private static void DisablePlayOnAwake(IEnumerable<ParticleSystem> particleSystems)
        {
            SerializedObject particlesProxy = new SerializedObject(particleSystems.ToArray());
            particlesProxy.FindProperty("playOnAwake").boolValue = false;
            particlesProxy.ApplyModifiedProperties();
        }

        private static void MakeAllParticleSystemsNotLoop(IEnumerable<ParticleSystem> particleSystems)
        {
            SerializedObject particlesProxy = new SerializedObject(particleSystems.ToArray());
            particlesProxy.FindProperty("looping").boolValue = false;
            particlesProxy.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targets))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            if (GUILayout.Button(new GUIContent("Find Particle Systems", "Searches on this component and its children. Overwrites anything previously set.")))
            {
                FindParticleSystems(targets.Cast<ParticleAction>());
            }

            ActionEditorUtil.ConditionalButton(new GUIContent("Remove null particles"),
                targets.Cast<ParticleAction>().Where(a => a.particles != null && a.particles.Any(p => p == null)),
                RemoveNullParticles);

            ActionEditorUtil.ConditionalButton(new GUIContent("Disable PlayOnAwake"),
                targets.Cast<ParticleAction>().SelectMany(a => a.particles.EmptyIfNull()).Where(p => p != null && p.main.playOnAwake),
                DisablePlayOnAwake);

            ActionEditorUtil.ConditionalButton(new GUIContent("Make all Particle Systems not loop"),
                targets.Cast<ParticleAction>().SelectMany(a => a.particles.EmptyIfNull()).Where(p => p != null && p.main.loop),
                MakeAllParticleSystemsNotLoop);
        }
    }
    #endif
}
