using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleAnimationAction : ActionBase
    {
        [Tooltip("Inverts the state of the given Activator. Equivalent to having a Logical NOT Activator "
            + "inserted between the given Activator and this Action.")]
        [SerializeField] private bool invertActivator;
        public Animator animator;
        public string boolParameterName = "state";

        public void Start() => OnEvent();

        public void OnEvent()
        {
            if (animator == null)
                return;
            bool state = activator != null && (bool)activator.GetProgramVariable("state");
            animator.SetBool(boolParameterName, invertActivator != state);
        }
    }
}
