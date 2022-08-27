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
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallback
    #endif
    {
        public float lerpDuration = 0.5f;

        [SerializeField] [HideInInspector] private UpdateManager updateManager;
        // for UpdateManager
        private int customUpdateInternalIndex;

        [UdonSynced]
        [FieldChangeCallback(nameof(TargetPosition))]
        [SerializeField]
        [HideInInspector]
        private Vector3 targetPosition;
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
                    RequestSerialization();
            }
        }
        private Vector3 lerpStartPosition;
        private float lerpStartTime;

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister() => JanSharp.OnBuildUtil.RegisterType<ObjectPositionSync>();
        }
        bool IOnBuildCallback.OnBuild()
        {
            updateManager = GameObject.Find("/UpdateManager")?.GetUdonSharpComponent<UpdateManager>();
            if (updateManager == null)
            {
                Debug.LogError("ObjectPositionSync requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
                return false;
            }
            targetPosition = this.transform.localPosition;
            this.ApplyProxyModifications();
            return true;
        }
        #endif

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
    }
}
