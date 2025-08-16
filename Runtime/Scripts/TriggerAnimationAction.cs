using UdonSharp;
using UnityEngine;

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
