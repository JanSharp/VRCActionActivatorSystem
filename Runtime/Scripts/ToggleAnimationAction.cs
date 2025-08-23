using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleAnimationAction : ActionBase
    {
        public Animator animator;
        public string boolParameterName = "state";

        public void Start() => OnEvent();

        public void OnEvent()
        {
            animator.SetBool(boolParameterName, (bool)activator.GetProgramVariable("state"));
        }
    }
}
