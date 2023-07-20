using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using UnityEditor;
using UdonSharpEditor;

namespace JanSharp
{
    [InitializeOnLoad]
    public static class ObjectPositionSyncOnBuild
    {
        static ObjectPositionSyncOnBuild() => JanSharp.OnBuildUtil.RegisterType<ObjectPositionSync>(OnBuild);

        private static bool OnBuild(ObjectPositionSync objectPositionSync)
        {
            UpdateManager updateManager = GameObject.Find("/UpdateManager")?.GetComponent<UpdateManager>();
            if (updateManager == null)
            {
                Debug.LogError("ObjectPositionSync requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    objectPositionSync);
                return false;
            }
            if (objectPositionSync.updateManager != updateManager)
            {
                SerializedObject activatorProxy = new SerializedObject(objectPositionSync);
                activatorProxy.FindProperty(nameof(ObjectPositionSync.updateManager)).objectReferenceValue = updateManager;
                activatorProxy.ApplyModifiedProperties();
            }
            if (objectPositionSync.targetPosition != objectPositionSync.transform.localPosition)
            {
                SerializedObject activatorProxy = new SerializedObject(objectPositionSync);
                activatorProxy.FindProperty(nameof(ObjectPositionSync.targetPosition)).vector3Value = objectPositionSync.transform.localPosition;
                activatorProxy.ApplyModifiedProperties();
            }
            return true;
        }
    }
}
