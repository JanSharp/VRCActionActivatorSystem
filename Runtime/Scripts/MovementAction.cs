using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class MovementAction : ConfigurableActionBase
    {
        [SerializeField] private ObjectPositionSync targetObject;
        [SerializeField] private Vector3 movementOnEvent;

        public void OnEvent()
        {
            targetObject.TargetPosition += movementOnEvent;
        }
    }
}
