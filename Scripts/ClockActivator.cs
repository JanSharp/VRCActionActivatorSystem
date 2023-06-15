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
    {
        [HideInInspector] public UdonSharpBehaviour[] onActivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onDeactivateListeners;
        [HideInInspector] public UdonSharpBehaviour[] onStateChangedListeners;
        [HideInInspector] public string[] onActivateListenerEventNames;
        [HideInInspector] public string[] onDeactivateListenerEventNames;
        [HideInInspector] public string[] onStateChangedListenerEventNames;

        [HideInInspector] public UpdateManager updateManager;
        public UdonSharpBehaviour inputActivator;
        [SerializeField] private float secondsBetweenTicks = 1f;
        // for UpdateManager
        private int customUpdateInternalIndex;
        /*[UdonSynced]*/ private float syncedElapsedTime;
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
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class ClockActivatorOnBuild
    {
        static ClockActivatorOnBuild()
        {
            OnBuildUtil.RegisterType<ClockActivator>(FirstOnBuild, order: 0);
            OnBuildUtil.RegisterType<ClockActivator>(SecondOnBuild, order: 1);
        }

        private static bool FirstOnBuild(UdonSharpBehaviour behaviour)
        {
            ClockActivator clockActivator = (ClockActivator)behaviour;
            clockActivator.updateManager = GameObject.Find("/UpdateManager")?.GetUdonSharpComponent<UpdateManager>();
            if (clockActivator.updateManager == null)
            {
                Debug.LogError("ClockActivator requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    UdonSharpEditorUtility.GetBackingUdonBehaviour(clockActivator));
                return false;
            }
            clockActivator.onActivateListeners = new UdonSharpBehaviour[0];
            clockActivator.onDeactivateListeners = new UdonSharpBehaviour[0];
            clockActivator.onStateChangedListeners = new UdonSharpBehaviour[0];
            clockActivator.onActivateListenerEventNames = new string[0];
            clockActivator.onDeactivateListenerEventNames = new string[0];
            clockActivator.onStateChangedListenerEventNames = new string[0];
            return true;
        }

        private static bool SecondOnBuild(UdonSharpBehaviour behaviour)
        {
            ClockActivator clockActivator = (ClockActivator)behaviour;
            ActivatorEditorUtil.AddActivatorToListeners(clockActivator.inputActivator, ActivatorEditorUtil.ListenerEventType.OnStateChanged, clockActivator);
            return true;
        }
    }
    #endif
}
