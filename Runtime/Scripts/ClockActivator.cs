using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ClockActivator : ActivatorBase
    {
        [HideInInspector][SingletonReference] public UpdateManager updateManager;
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
}
