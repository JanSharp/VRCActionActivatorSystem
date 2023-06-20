#if UNITY_EDITOR
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UdonSharp;
using System.Linq;

namespace JanSharp
{
    public static class ActivatorEditorUtil
    {
        public static bool BasicActionOnBuild<T>(T action) where T : ActionBase
        {
            AddActivatorToListeners(action.activator, action.listenerType, action);
            return true;
        }

        public static void AddActivatorToListeners(UdonSharpBehaviour activator, ListenerType listenerType, UdonSharpBehaviour listener, string listenerEventName = "OnEvent")
        {
            if (activator == null)
            {
                Debug.LogError($"Missing/null Activator for {listener.name}.", listener);
                return;
            }
            SerializedObject activatorProxy = new SerializedObject(activator);
            SerializedProperty listenersField;
            SerializedProperty eventNamesField;
            switch (listenerType)
            {
                case ListenerType.OnActivate:
                    listenersField = activatorProxy.FindProperty(nameof(ActivatorBase.onActivateListeners));
                    eventNamesField = activatorProxy.FindProperty(nameof(ActivatorBase.onActivateListenerEventNames));
                    break;
                case ListenerType.OnDeactivate:
                    listenersField = activatorProxy.FindProperty(nameof(ActivatorBase.onDeactivateListeners));
                    eventNamesField = activatorProxy.FindProperty(nameof(ActivatorBase.onDeactivateListenerEventNames));
                    break;
                case ListenerType.OnStateChanged:
                    listenersField = activatorProxy.FindProperty(nameof(ActivatorBase.onStateChangedListeners));
                    eventNamesField = activatorProxy.FindProperty(nameof(ActivatorBase.onStateChangedListenerEventNames));
                    break;
                default:
                    Debug.LogError($"Impossible listener type {listenerType}.", listener);
                    return;
            }
            Append(listenersField, p => p.objectReferenceValue = listener);
            Append(eventNamesField, p => p.stringValue = listenerEventName);
            activatorProxy.ApplyModifiedProperties();
        }

        private static void Append(SerializedProperty property, System.Action<SerializedProperty> setNewValue)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            setNewValue(property.GetArrayElementAtIndex(property.arraySize - 1));
        }

        public static bool ActivatorOnBuildBase(UdonSharpBehaviour behaviour)
        {
            SerializedObject activatorProxy = new SerializedObject(behaviour);
            activatorProxy.FindProperty(nameof(ActivatorBase.onActivateListeners)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onDeactivateListeners)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onStateChangedListeners)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onActivateListenerEventNames)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onDeactivateListenerEventNames)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onStateChangedListenerEventNames)).ClearArray();
            activatorProxy.ApplyModifiedProperties();
            return true;
        }
    }

    public static class ActionEditorUtil
    {
        public static void SetArrayProperty<T>(SerializedProperty property, ICollection<T> newValues, System.Action<SerializedProperty, T> setValue)
        {
            property.ClearArray();
            property.arraySize = newValues.Count;
            int i = 0;
            foreach (T value in newValues)
                setValue(property.GetArrayElementAtIndex(i++), value);
        }

        public static void ConditionalButton<T>(GUIContent buttonContent, IEnumerable<T> targets, System.Action<IEnumerable<T>> onButtonClick)
        {
            if (targets.Any() && GUILayout.Button(buttonContent))
                onButtonClick(targets);
        }

        public static IEnumerable<T> EmptyIfNull<T>(this IEnumerable<T> enumerable)
        {
            if (enumerable != null)
                foreach (T value in enumerable)
                    yield return value;
        }
    }
}
#endif
