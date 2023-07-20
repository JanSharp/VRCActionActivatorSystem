using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor;
using UdonSharpEditor;
using System.Linq;
using System.Collections.Generic;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class AudioActionOnBuild
    {
        static AudioActionOnBuild() => OnBuildUtil.RegisterType<AudioAction>(ActivatorEditorUtil.BasicActionOnBuild, order: 1);
    }

    [CanEditMultipleObjects]
    [CustomEditor(typeof(AudioAction))]
    public class AudioActionEditor : Editor
    {
        private static void FindAudioSources(IEnumerable<AudioAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                ActionEditorUtil.SetArrayProperty(
                    actionProxy.FindProperty(nameof(AudioAction.audioSources)),
                    action.GetComponentsInChildren<AudioSource>(),
                    (p, v) => p.objectReferenceValue = v);
                actionProxy.ApplyModifiedProperties();
            }
        }

        private static void RemoveNullAudioSources(IEnumerable<AudioAction> actions)
        {
            foreach (var action in actions)
            {
                SerializedObject actionProxy = new SerializedObject(action);
                ActionEditorUtil.SetArrayProperty(
                    actionProxy.FindProperty(nameof(AudioAction.audioSources)),
                    action.audioSources.Where(s => s != null).ToList(),
                    (p, v) => p.objectReferenceValue = v);
                actionProxy.ApplyModifiedProperties();
            }
        }

        private static void DisablePlayOnAwake(IEnumerable<AudioSource> audioSources)
        {
            SerializedObject audioSourceProxy = new SerializedObject(audioSources.ToArray());
            audioSourceProxy.FindProperty("m_PlayOnAwake").boolValue = false;
            audioSourceProxy.ApplyModifiedProperties();
        }

        private static void MakeAudioSourcesNotLoop(IEnumerable<AudioSource> audioSources)
        {
            SerializedObject audioSourceProxy = new SerializedObject(audioSources.ToArray());
            audioSourceProxy.FindProperty("Loop").boolValue = false;
            audioSourceProxy.ApplyModifiedProperties();
        }

        public override void OnInspectorGUI()
        {
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targets))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            if (GUILayout.Button(new GUIContent("Find Audio Sources", "Searches on this component and its children. Overwrites anything previously set.")))
            {
                FindAudioSources(targets.Cast<AudioAction>());
            }

            ActionEditorUtil.ConditionalButton(new GUIContent("Remove null Audio Sources"),
                targets.Cast<AudioAction>().Where(a => a.audioSources != null && a.audioSources.Any(s => s == null)),
                RemoveNullAudioSources);

            ActionEditorUtil.ConditionalButton(new GUIContent("Disable PlayOnAwake"),
                targets.Cast<AudioAction>().SelectMany(a => a.audioSources.EmptyIfNull()).Where(s => s != null && s.playOnAwake),
                DisablePlayOnAwake);

            ActionEditorUtil.ConditionalButton(new GUIContent("Make Audio Sources not loop"),
                targets.Cast<AudioAction>().SelectMany(a => a.audioSources.EmptyIfNull()).Where(s => s != null && s.loop),
                MakeAudioSourcesNotLoop);
        }
    }
}
