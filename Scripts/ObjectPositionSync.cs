using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.Manual)]
    public class ObjectPositionSync : UdonSharpBehaviour
    {
        public float lerpDuration = 0.5f;

        [HideInInspector] public UpdateManager updateManager;
        // for UpdateManager
        private int customUpdateInternalIndex;

        [UdonSynced]
        [FieldChangeCallback(nameof(TargetPosition))]
        [HideInInspector]
        public Vector3 targetPosition;
        public Vector3 TargetPosition
        {
            get => targetPosition;
            set
            {
                if (value == targetPosition)
                    return;
                targetPosition = value;
                updateManager.Register(this);
                lerpStartPosition = this.transform.localPosition;
                lerpStartTime = Time.time;
                if (Networking.IsOwner(Networking.LocalPlayer, this.gameObject))
                {
                    requestSerializationCount++;
                    SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), 2f);
                }
            }
        }
        private Vector3 lerpStartPosition;
        private float lerpStartTime;

        public void CustomUpdate()
        {
            var percent = (Time.time - lerpStartTime) / lerpDuration;
            if (percent >= 1f)
            {
                this.transform.localPosition = targetPosition;
                updateManager.Deregister(this);
                return;
            }
            this.transform.localPosition = Vector3.Lerp(lerpStartPosition, targetPosition, percent);
        }

        private int requestSerializationCount = 0;
        private bool waitingForOwnerToSendData = false;
        private const float LateJoinerSyncDelay = 8f;

        public override void OnPlayerJoined(VRCPlayerApi player)
        {
            if (Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), LateJoinerSyncDelay);
            }
            else
            {
                waitingForOwnerToSendData = true;
            }
        }

        public override void OnOwnershipTransferred(VRCPlayerApi player)
        {
            if (waitingForOwnerToSendData && Networking.IsOwner(this.gameObject))
            {
                requestSerializationCount++;
                SendCustomEventDelayedSeconds(nameof(RequestSerializationDelayed), LateJoinerSyncDelay);
            }
        }

        public void RequestSerializationDelayed()
        {
            if ((--requestSerializationCount) == 0)
                RequestSerialization();
        }

        public override void OnDeserialization()
        {
            waitingForOwnerToSendData = false;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ObjectPositionSyncOnBuild
    {
        static ObjectPositionSyncOnBuild() => JanSharp.OnBuildUtil.RegisterType<ObjectPositionSync>(OnBuild);

        private static bool OnBuild(UdonSharpBehaviour behaviour)
        {
            ObjectPositionSync objectPositionSync = (ObjectPositionSync)behaviour;
            objectPositionSync.updateManager = GameObject.Find("/UpdateManager")?.GetComponent<UpdateManager>();
            if (objectPositionSync.updateManager == null)
            {
                Debug.LogError("ObjectPositionSync requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    objectPositionSync);
                return false;
            }
            objectPositionSync.targetPosition = objectPositionSync.transform.localPosition;
            if (PrefabUtility.IsPartOfPrefabInstance(objectPositionSync))
                PrefabUtility.RecordPrefabInstancePropertyModifications(objectPositionSync);
            return true;
        }
    }
    #endif
}
