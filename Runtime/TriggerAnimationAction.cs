using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class TriggerAnimationAction : ConfigurableActionBase
    {
        public Animator animator;
        public string triggerParameterName = "trigger";

        public void OnEvent()
        {
            animator.SetTrigger(triggerParameterName);
        }
    }
}
