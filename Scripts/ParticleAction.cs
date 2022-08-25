using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
using System.Reflection;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleAction : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        [SerializeField] private ParticleSystem particle;
        [SerializeField] private UdonSharpBehaviour activator;
        [HideInInspector] public int listenerType;

        public void OnEvent()
        {
            particle.Play();
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => JanSharp.OnBuildUtil.RegisterType<ParticleAction>(order: 1);
        }
        bool IOnBuildCallback.OnBuild()
        {
            if (activator == null)
            {
                Debug.LogError("Missing Activator.", UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
                return false;
            }
            FieldInfo listenersField;
            switch (listenerType)
            {
                case 0:
                    listenersField = activator.GetType().GetField("onActivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case 1:
                    listenersField = activator.GetType().GetField("onDeactivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case 2:
                    listenersField = activator.GetType().GetField("onStateChangedListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                default:
                    Debug.LogError($"Impossible listener type {listenerType}.", UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
                    return false;
            }
            UdonSharpBehaviour[] listeners = listenersField.GetValue(activator) as UdonSharpBehaviour[];
            UdonSharpBehaviour[] newListeners = new UdonSharpBehaviour[listeners.Length + 1];
            listeners.CopyTo(newListeners, 0);
            newListeners[newListeners.Length - 1] = this;
            listenersField.SetValue(activator, newListeners);
            activator.ApplyProxyModifications();
            return true;
        }
        #endif
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [CustomEditor(typeof(ParticleAction))]
    public class ParticleActionEditor : Editor
    {
        private static string[] ListenerTypes = new string[]
        {
            "On Activate",
            "On Deactivate",
            "On State Changed",
        };

        public override void OnInspectorGUI()
        {
            ParticleAction target = this.target as ParticleAction;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(target))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            int newListenerType = EditorGUILayout.Popup(
                new GUIContent("Activator Event", "Which event of the activator to react to."),
                target.listenerType,
                ListenerTypes);

            if (newListenerType != target.listenerType)
            {
                target.listenerType = newListenerType;
                target.ApplyProxyModifications();
            }
        }
    }
    #endif
}
