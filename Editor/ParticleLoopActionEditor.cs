using System.Collections.Generic;
using System.Linq;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ParticleLoopActionOnBuild
    {
        static ParticleLoopActionOnBuild() => OnBuildUtil.RegisterType<ParticleLoopAction>(OnBuild, order: 1);

        private static bool OnBuild(ParticleLoopAction particleLoopAction)
        {
            ActivatorEditorUtil.AddActivatorToListeners(particleLoopAction.activator, ListenerType.OnStateChanged, particleLoopAction);
            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(ParticleLoopAction))]
    public class ParticleLoopActionEditor : Editor
    {
        private static void FindParticleSystems(IEnumerable<ParticleLoopAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                ActionEditorUtil.SetArrayProperty(
                    actionProxy.FindProperty(nameof(ParticleLoopAction.particles)),
                    action.GetComponentsInChildren<ParticleSystem>(),
                    (p, v) => p.objectReferenceValue = v);
                actionProxy.ApplyModifiedProperties();
            }
        }

        private static void RemoveNullParticles(IEnumerable<ParticleLoopAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                ActionEditorUtil.SetArrayProperty(
                    actionProxy.FindProperty(nameof(ParticleLoopAction.particles)),
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

        private static void MakeAllParticleSystemsLoop(IEnumerable<ParticleSystem> particleSystems)
        {
            SerializedObject particlesProxy = new SerializedObject(particleSystems.ToArray());
            particlesProxy.FindProperty("looping").boolValue = true;
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
                FindParticleSystems(targets.Cast<ParticleLoopAction>());
            }

            ActionEditorUtil.ConditionalButton(new GUIContent("Remove null particles"),
                targets.Cast<ParticleLoopAction>().Where(a => a.particles != null && a.particles.Any(p => p == null)),
                RemoveNullParticles);

            ActionEditorUtil.ConditionalButton(new GUIContent("Disable PlayOnAwake"),
                targets.Cast<ParticleLoopAction>().SelectMany(a => a.particles.EmptyIfNull()).Where(p => p != null && p.main.playOnAwake),
                DisablePlayOnAwake);

            ActionEditorUtil.ConditionalButton(new GUIContent("Make all Particle Systems loop"),
                targets.Cast<ParticleLoopAction>().SelectMany(a => a.particles.EmptyIfNull()).Where(p => p != null && p.main.loop),
                MakeAllParticleSystemsLoop);
        }
    }
}
