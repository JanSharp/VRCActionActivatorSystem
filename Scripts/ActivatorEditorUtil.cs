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
        public static bool ActionOnBuild<T>(T action) where T : UdonSharpBehaviour, IAction
        {
            if (action.Activator == null)
            {
                Debug.LogError("Missing Activator.", UdonSharpEditorUtility.GetBackingUdonBehaviour(action));
                return false;
            }
            FieldInfo listenersField;
            FieldInfo eventNamesField;
            switch (action.ListenerType)
            {
                case 0:
                    listenersField = action.Activator.GetType().GetField("onActivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    eventNamesField = action.Activator.GetType().GetField("onActivateListenerEventNames", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case 1:
                    listenersField = action.Activator.GetType().GetField("onDeactivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    eventNamesField = action.Activator.GetType().GetField("onDeactivateListenerEventNames", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case 2:
                    listenersField = action.Activator.GetType().GetField("onStateChangedListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    eventNamesField = action.Activator.GetType().GetField("onStateChangedListenerEventNames", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                default:
                    Debug.LogError($"Impossible listener type {action.ListenerType}.", UdonSharpEditorUtility.GetBackingUdonBehaviour(action));
                    return false;
            }
            // must explicitly define generic type parameter because `action` itself is a different type
            GrowArray<UdonSharpBehaviour>(action.Activator, listenersField, action);
            GrowArray(action.Activator, eventNamesField, "OnEvent");
            action.Activator.ApplyProxyModifications();
            return true;
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
