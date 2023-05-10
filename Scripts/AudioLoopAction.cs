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
    public class AudioLoopAction : UdonSharpBehaviour
    {
        public AudioSource audioSource;
        public UdonSharpBehaviour activator;

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class AudioLoopActionOnBuild
    {
        static AudioLoopActionOnBuild() => OnBuildUtil.RegisterType<AudioLoopAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            AudioLoopAction audioLoopAction = (AudioLoopAction)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(audioLoopAction.activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, audioLoopAction);
            return true;
        }
    }
    #endif

    [CustomEditor(typeof(AudioLoopAction))]
    public class AudioLoopActionEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            AudioLoopAction targetAction = this.target as AudioLoopAction;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targetAction))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            if (targetAction.audioSource == null
                && targetAction.GetComponent<AudioSource>() != null
                && GUILayout.Button(new GUIContent("Set Audio Source to this")))
            {
                targetAction.audioSource = targetAction.GetComponent<AudioSource>();
                targetAction.ApplyProxyModifications();
                EditorUtility.SetDirty(targetAction);
            }

            if (targetAction.audioSource != null
                && targetAction.audioSource.playOnAwake
                && GUILayout.Button(new GUIContent("Disable paly on awake")))
            {
                targetAction.audioSource.playOnAwake = false;
                EditorUtility.SetDirty(targetAction.audioSource);
            }

            if (targetAction.audioSource != null
                && !targetAction.audioSource.loop
                && GUILayout.Button(new GUIContent("Make Audio Source loop")))
            {
                targetAction.audioSource.loop = true;
                EditorUtility.SetDirty(targetAction.audioSource);
            }
        }
    }
    #endif
}
