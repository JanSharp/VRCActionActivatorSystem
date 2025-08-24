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
            animator.SetBool(boolParameterName, invertActivator != (bool)activator.GetProgramVariable("state"));
        }
    }
}
