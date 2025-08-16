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
