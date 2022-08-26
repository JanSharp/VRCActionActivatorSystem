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
    public class AudioAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback, IAction
    #endif
    {
        public AudioSource[] audioSources;
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            foreach (var audioSource in audioSources)
                audioSource.Play();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        UdonSharpBehaviour IAction.Activator => activator;
        int IAction.ListenerType { get => listenerType; set => listenerType = value; }

        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<AudioAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild() => ActivatorEditorUtil.BasicActionOnBuild(this);
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(AudioAction))]
    public class AudioActionEditor : ActionEditorBase
    {
        public override void OnInspectorGUI()
        {
            AudioAction targetAction = this.target as AudioAction;
            base.OnInspectorGUI();

            if (GUILayout.Button(new GUIContent("Find all child audio sources")))
            {
                targetAction.audioSources = targetAction.GetComponentsInChildren<AudioSource>();
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.audioSources != null
                && targetAction.audioSources.Any(a => a == null)
                && GUILayout.Button(new GUIContent("Remove null Audio Sources")))
            {
                targetAction.audioSources = targetAction.audioSources.Where(p => p != null).ToArray();
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.audioSources != null
                && targetAction.audioSources.Any(a => a != null && a.playOnAwake)
                && GUILayout.Button(new GUIContent("Disable PlayOnAwake")))
            {
                foreach (var audioSource in targetAction.audioSources.Where(a => a.playOnAwake))
                {
                    audioSource.playOnAwake = false;
                    EditorUtility.SetDirty(audioSource);
                }
            }

            if (targetAction.audioSources != null
                && targetAction.audioSources.Any(a => a != null && a.loop)
                && GUILayout.Button(new GUIContent("Make Audio Sources not loop")))
            {
                foreach (var audioSource in targetAction.audioSources.Where(a => a.loop))
                {
                    audioSource.loop = false;
                    EditorUtility.SetDirty(audioSource);
                }
            }
        }
    }
    #endif
}
