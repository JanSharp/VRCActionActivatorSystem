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
            switch (action.ListenerType)
            {
                case 0:
                    listenersField = action.Activator.GetType().GetField("onActivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case 1:
                    listenersField = action.Activator.GetType().GetField("onDeactivateListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                case 2:
                    listenersField = action.Activator.GetType().GetField("onStateChangedListeners", BindingFlags.NonPublic | BindingFlags.Instance);
                    break;
                default:
                    Debug.LogError($"Impossible listener type {action.ListenerType}.", UdonSharpEditorUtility.GetBackingUdonBehaviour(action));
                    return false;
            }
            UdonSharpBehaviour[] listeners = listenersField.GetValue(action.Activator) as UdonSharpBehaviour[];
            UdonSharpBehaviour[] newListeners = new UdonSharpBehaviour[listeners.Length + 1];
            listeners.CopyTo(newListeners, 0);
            newListeners[newListeners.Length - 1] = action;
            listenersField.SetValue(action.Activator, newListeners);
            action.Activator.ApplyProxyModifications();
            return true;
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
