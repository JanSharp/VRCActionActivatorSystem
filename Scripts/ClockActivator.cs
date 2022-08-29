using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ClockActivator : UdonSharpBehaviour
    #if UNITY_EDITOR && !COMPILER_UDONSHARP
        , IOnBuildCallbackV2
    #endif
    {
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onActivateListeners;
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onDeactivateListeners;
        [SerializeField] [HideInInspector] private UdonSharpBehaviour[] onStateChangedListeners;
        [SerializeField] [HideInInspector] private string[] onActivateListenerEventNames;
        [SerializeField] [HideInInspector] private string[] onDeactivateListenerEventNames;
        [SerializeField] [HideInInspector] private string[] onStateChangedListenerEventNames;

        [SerializeField] [HideInInspector] private UpdateManager updateManager;
        // for UpdateManager
        private int customUpdateInternalIndex;
        [SerializeField] private UdonSharpBehaviour inputActivator;
        [SerializeField] private float secondsBetweenTicks = 1f;
        [UdonSynced] private float syncedElapsedTime;
        private const float SyncLatencyCompensation = 0.1f; // just a guess. Will never be accurate
        private float elapsedTime;
        private bool initialized;

        private bool state;
        private bool State
        {
            get => state;
            set
            {
                if (value == state)
                    return;
                state = value;
                if (value)
                    Send(onActivateListeners, onActivateListenerEventNames);
                else
                    Send(onDeactivateListeners, onDeactivateListenerEventNames);
                Send(onStateChangedListeners, onStateChangedListenerEventNames);
            }
        }

        private void Send(UdonSharpBehaviour[] listeners, string[] listenerEventNames)
        {
            for (int i = 0; i < listeners.Length; i++)
                listeners[i].SendCustomEvent(listenerEventNames[i]);
        }

        public void OnEvent()
        {
            if ((bool)inputActivator.GetProgramVariable("state"))
                updateManager.Register(this);
            else
                updateManager.Deregister(this);
        }

        public void CustomUpdate()
        {
            elapsedTime += Time.deltaTime;
            if (elapsedTime >= secondsBetweenTicks)
            {
                elapsedTime -= secondsBetweenTicks;
                State = true;
                State = false;
            }
        }

        public override void OnPreSerialization()
        {
            syncedElapsedTime = elapsedTime;
        }

        public override void OnDeserialization()
        {
            if (!initialized)
            {
                initialized = true;
                elapsedTime = syncedElapsedTime + Mathf.Min(SyncLatencyCompensation, secondsBetweenTicks * 0.8f);
            }
        }

        #if UNITY_EDITOR && !COMPILER_UDONSHARP
        [InitializeOnLoad]
        public static class OnBuildRegister
        {
            static OnBuildRegister()
            {
                OnBuildUtil.RegisterTypeV2<ClockActivator>(order: 0);
                OnBuildUtil.RegisterTypeV2<ClockActivator>(order: 1);
            }
        }
        bool IOnBuildCallbackV2.OnBuild(int order)
        {
            if (order == 0)
            {
                updateManager = GameObject.Find("/UpdateManager")?.GetUdonSharpComponent<UpdateManager>();
                if (updateManager == null)
                {
                    Debug.LogError("ClockActivator requires a GameObject that must be at the root of the scene "
                            + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                        UdonSharpEditorUtility.GetBackingUdonBehaviour(this));
                    return false;
                }
                onActivateListeners = new UdonSharpBehaviour[0];
                onDeactivateListeners = new UdonSharpBehaviour[0];
                onStateChangedListeners = new UdonSharpBehaviour[0];
                onActivateListenerEventNames = new string[0];
                onDeactivateListenerEventNames = new string[0];
                onStateChangedListenerEventNames = new string[0];
                this.ApplyProxyModifications();
            }
            else
            {
                ActivatorEditorUtil.AddActivatorToListeners(inputActivator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, this);
            }
            return true;
        }
        #endif
    }
}
