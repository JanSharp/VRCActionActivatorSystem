using System.Collections.Generic;
using System.Linq;
using UdonSharp;
using UnityEditor;
using UnityEngine;

namespace JanSharp
{
    public static class ActivatorEditorUtil
    {
        public static bool BasicActionOnBuild<T>(T action) where T : ConfigurableActionBase
        {
            AddActivatorToListeners(action.activator, action.listenerType, action);
            return true;
        }

        public static void AddActivatorToListeners(ActivatorBase activator, ListenerType listenerType, UdonSharpBehaviour listener, string listenerEventName = "OnEvent")
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
            AppendProperty(listenersField, p => p.objectReferenceValue = listener);
            AppendProperty(eventNamesField, p => p.stringValue = listenerEventName);
            activatorProxy.ApplyModifiedProperties();
        }

        public static void AppendProperty(SerializedProperty property, System.Action<SerializedProperty> setNewValue)
        {
            property.InsertArrayElementAtIndex(property.arraySize);
            setNewValue(property.GetArrayElementAtIndex(property.arraySize - 1));
        }

        public static bool ActivatorOnBuildBase(ActivatorBase activator)
        {
            SerializedObject activatorProxy = new SerializedObject(activator);
            activatorProxy.FindProperty(nameof(ActivatorBase.onActivateListeners)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onDeactivateListeners)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onStateChangedListeners)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onActivateListenerEventNames)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onDeactivateListenerEventNames)).ClearArray();
            activatorProxy.FindProperty(nameof(ActivatorBase.onStateChangedListenerEventNames)).ClearArray();
            activatorProxy.ApplyModifiedProperties();
            return true;
        }

        private static UdonSharpBehaviour[] EmptyListeners = new UdonSharpBehaviour[0];

        public static void OnActivatorInspectorGUI<T>(List<T> targets)
            where T : ActivatorBase
        {
            EditorGUILayout.Space();
            if (GUILayout.Button(new GUIContent(
                $"Update Listeners List{(targets.Count == 1 ? "" : "s")}",
                "Triggers on build handlers, which is to say that pressing this button "
                    + "is purely optional and just exists for convenience")))
            {
                OnBuildUtil.RunOnBuild(showDialogOnFailure: true, useSceneViewNotification: false, abortIfScriptsGotInstantiated: false);
            }

            foreach (T target in targets)
            {
                EditorGUILayout.Space();
                using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                {
                    using (new EditorGUI.DisabledGroupScope(disabled: true))
                        EditorGUILayout.ObjectField("Activator", target, typeof(T), allowSceneObjects: true);
                    using (new GUILayout.VerticalScope(EditorStyles.helpBox))
                    {
                        int listenerCount = target.onActivateListeners?.Length ?? 0
                            + target.onDeactivateListeners?.Length ?? 0
                            + target.onStateChangedListeners?.Length ?? 0;
                        GUILayout.Label($"Listeners: ({listenerCount})", EditorStyles.boldLabel);
                        using (new EditorGUI.DisabledGroupScope(disabled: true))
                        {
                            foreach (var listener in target.onActivateListeners ?? EmptyListeners)
                                EditorGUILayout.ObjectField("On Activate", listener, typeof(UdonSharpBehaviour), allowSceneObjects: true);
                            foreach (var listener in target.onDeactivateListeners ?? EmptyListeners)
                                EditorGUILayout.ObjectField("On Deactivate", listener, typeof(UdonSharpBehaviour), allowSceneObjects: true);
                            foreach (var listener in target.onStateChangedListeners ?? EmptyListeners)
                                EditorGUILayout.ObjectField("On State Changed", listener, typeof(UdonSharpBehaviour), allowSceneObjects: true);
                        }
                    }
                }
            }
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
