using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ClockActivator : ActivatorBase
    {
        [HideInInspector][SingletonReference] public UpdateManager updateManager;
        public ActivatorBase inputActivator;
        [Tooltip("Inverts the state of the given Activator. Equivalent to having a Logical NOT Activator "
            + "inserted between the given Activator and this Clock Activator.")]
        [SerializeField] private bool invertActivator;
        [SerializeField] private float secondsBetweenTicks = 1f;
        // for UpdateManager
        private int customUpdateInternalIndex;
        /*[UdonSynced]*/ private float syncedElapsedTime;
        private const float SyncLatencyCompensation = 0.1f; // just a guess. Will never be accurate
        private float elapsedTime;
        private bool initialized;

        public void Start() => OnEvent();

        public void OnEvent()
        {
            bool state = inputActivator != null && (bool)inputActivator.GetProgramVariable("state");
            if (invertActivator != state)
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
