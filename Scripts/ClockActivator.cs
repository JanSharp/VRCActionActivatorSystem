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
    public class ClockActivator : ActivatorBase
    {
        [HideInInspector] public UpdateManager updateManager;
        public ActivatorBase inputActivator;
        [SerializeField] private float secondsBetweenTicks = 1f;
        // for UpdateManager
        private int customUpdateInternalIndex;
        /*[UdonSynced]*/ private float syncedElapsedTime;
        private const float SyncLatencyCompensation = 0.1f; // just a guess. Will never be accurate
        private float elapsedTime;
        private bool initialized;

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

        private static bool FirstOnBuild(ClockActivator clockActivator)
        {
            UpdateManager updateManager = GameObject.Find("/UpdateManager")?.GetComponent<UpdateManager>();
            if (updateManager == null)
            {
                Debug.LogError("ClockActivator requires a GameObject that must be at the root of the scene "
                        + "with the exact name 'UpdateManager' which has the 'UpdateManager' UdonBehaviour.",
                    clockActivator);
                return false;
            }
            if (clockActivator.updateManager != updateManager)
            {
                SerializedObject activatorProxy = new SerializedObject(clockActivator);
                activatorProxy.FindProperty(nameof(ClockActivator.updateManager)).objectReferenceValue = updateManager;
                activatorProxy.ApplyModifiedProperties();
            }
            return ActivatorEditorUtil.ActivatorOnBuildBase(clockActivator);
        }

        private static bool SecondOnBuild(ClockActivator clockActivator)
        {
            ActivatorEditorUtil.AddActivatorToListeners(clockActivator.inputActivator, ListenerType.OnStateChanged, clockActivator);
            return true;
        }
    }
    #endif
}
