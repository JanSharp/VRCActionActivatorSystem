using System.Collections.Generic;
using System.Linq;
using UdonSharpEditor;
using UnityEditor;
using UnityEngine;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ParticleActionOnBuild
    {
        static ParticleActionOnBuild() => OnBuildUtil.RegisterType<ParticleAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
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

            if (GUILayout.Button(new GUIContent("Find Particle Systems", "Searches on this game object and its children. Overwrites anything previously set.")))
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
}
