#if UNITY_EDITOR
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharp;
using UdonSharpEditor;
using System.Reflection;

namespace JanSharp
{
    public static class ActivatorEditorUtil
    {
        public enum ListenerEventType
        {
            OnActivate = 0,
            OnDeactivate = 1,
            OnStateChanged = 2,
        }

        public static bool BasicActionOnBuild<T>(T action) where T : UdonSharpBehaviour, IAction
        {
            AddActivatorToListeners(action.Activator, (ListenerEventType)action.ListenerType, action);
            return true;
        }

        public static void AddActivatorToListeners(UdonSharpBehaviour activator, ListenerEventType listenerType, UdonSharpBehaviour listener, string listenerEventName = "OnEvent")
        {
            if (activator == null)
            {
                Debug.LogError($"Missing/null Activator for {listener.name}.", UdonSharpEditorUtility.GetBackingUdonBehaviour(listener));
                return;
            }
            FieldInfo listenersField;
            FieldInfo eventNamesField;
            switch (listenerType)
            {
                case ListenerEventType.OnActivate:
                    listenersField = activator.GetType().GetField("onActivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    eventNamesField = activator.GetType().GetField("onActivateListenerEventNames", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case ListenerEventType.OnDeactivate:
                    listenersField = activator.GetType().GetField("onDeactivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    eventNamesField = activator.GetType().GetField("onDeactivateListenerEventNames", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case ListenerEventType.OnStateChanged:
                    listenersField = activator.GetType().GetField("onStateChangedListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    eventNamesField = activator.GetType().GetField("onStateChangedListenerEventNames", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                default:
                    Debug.LogError($"Impossible listener type {listenerType}.", UdonSharpEditorUtility.GetBackingUdonBehaviour(listener));
                    return;
            }
            GrowArray(activator, listenersField, listener);
            GrowArray(activator, eventNamesField, listenerEventName);
            activator.ApplyProxyModifications();
        }

        private static void GrowArray<T>(object instance, FieldInfo field, T newValue)
        {
            T[] listeners = field.GetValue(instance) as T[];
            T[] newListeners = new T[listeners.Length + 1];
            listeners.CopyTo(newListeners, 0);
            newListeners[newListeners.Length - 1] = newValue;
            field.SetValue(instance, newListeners);
        }
    }

    public interface IAction
    {
        UdonSharpBehaviour Activator { get; }
        int ListenerType { get; set; }
    }

    public abstract class ActionEditorBase : Editor
    {
        private static string[] ListenerTypes = new string[]
        {
            "On Activate",
            "On Deactivate",
            "On State Changed",
        };

        public override void OnInspectorGUI()
        {
            IAction targetAction = this.target as IAction;
            UdonSharpBehaviour targetBehaviour = this.target as UdonSharpBehaviour;
            if (UdonSharpGUI.DrawDefaultUdonSharpBehaviourHeader(targetBehaviour))
                return;
            EditorGUILayout.Space();
            base.OnInspectorGUI(); // draws public/serializable fields

            int newListenerType = EditorGUILayout.Popup(
                new GUIContent("Activator Event", "Which event of the activator to react to."),
                targetAction.ListenerType,
                ListenerTypes);

            if (newListenerType != targetAction.ListenerType)
            {
                targetAction.ListenerType = newListenerType;
                targetBehaviour.ApplyProxyModifications();
                EditorUtility.SetDirty(targetBehaviour);
            }
        }
    }
}
#endif
