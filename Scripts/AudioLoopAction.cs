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
    public class AudioLoopAction : ActionBase
    {
        public AudioSource audioSource;

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP

    [InitializeOnLoad]
    public static class AudioLoopActionOnBuild
    {
        static AudioLoopActionOnBuild() => OnBuildUtil.RegisterType<AudioLoopAction>(OnBuild, order: 1);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            AudioLoopAction audioLoopAction = (AudioLoopAction)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(audioLoopAction.activator, ListenerType.OnStateChanged, audioLoopAction);
            return true;
        }
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioLoopAction))]
    public class AudioLoopActionEditor : Editor
    {
        private static void SetAudioSourceToThis(IEnumerable<AudioLoopAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                actionProxy.FindProperty(nameof(AudioLoopAction.audioSource)).objectReferenceValue = action.GetComponent<AudioSource>();
                actionProxy.ApplyModifiedProperties();
            }
        }

        private static void DisablePlayOnAwake(IEnumerable<AudioLoopAction> actions)
        {
            SerializedObject audioSourceProxy = new SerializedObject(actions.Select(a => a.audioSource).ToArray());
            audioSourceProxy.FindProperty("m_PlayOnAwake").boolValue = false;
            audioSourceProxy.ApplyModifiedProperties();
        }

        private static void MakeAudioSourcesLoop(IEnumerable<AudioLoopAction> actions)
        {
            SerializedObject audioSourceProxy = new SerializedObject(actions.Select(a => a.audioSource).ToArray());
            audioSourceProxy.FindProperty("Loop").boolValue = true;
            audioSourceProxy.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targets))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            ActionEditorUtil.ConditionalButton(new GUIContent("Set Audio Source to this"),
                targets.Cast<AudioLoopAction>().Where(a => a.audioSource == null && a.GetComponent<AudioSource>() != null),
                SetAudioSourceToThis);

            ActionEditorUtil.ConditionalButton(new GUIContent("Disable PlayOnAwake"),
                targets.Cast<AudioLoopAction>().Where(a => a.audioSource != null && a.audioSource.playOnAwake),
                DisablePlayOnAwake);

            ActionEditorUtil.ConditionalButton(new GUIContent("Make Audio Source loop"),
                targets.Cast<AudioLoopAction>().Where(a => a.audioSource != null && !a.audioSource.loop),
                MakeAudioSourcesLoop);
        }
    }
    #endif
}
