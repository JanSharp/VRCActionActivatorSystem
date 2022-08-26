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
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        public ParticleSystem[] particles;
        [SerializeField] private UdonSharpBehaviour activator;

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                foreach (var particle in particles)
                    particle.Play();
            else
                foreach (var particle in particles)
                    particle.Stop();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<ParticleLoopAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild()
        {
            ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, this);
            return true;
        }
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
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
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p == null)
                && GUILayout.Button(new GUIContent("Remove null particles")))
            {
                targetAction.particles = targetAction.particles.Where(p => p != null).ToArray();
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.particles != null
                && targetAction.particles.Any(p => p != null && p.main.playOnAwake)
                && GUILayout.Button(new GUIContent("Disable PlayOnAwake")))
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
