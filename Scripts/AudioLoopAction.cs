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
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        public AudioSource audioSource;
        [SerializeField] private UdonSharpBehaviour activator;

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                audioSource.Play();
            else
                audioSource.Stop();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => OnBuildUtil.RegisterType<AudioLoopAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild()
        {
            ActivatorEditorUtil.AddActivatorToListeners(activator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, this);
            return true;
        }
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
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
        }
    }
    #endif
}
