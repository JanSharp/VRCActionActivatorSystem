using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleAnimationAction : ActionBase
    {
        public Animator animator;
        public string boolParameterName = "state";

        public void OnEvent()
        {
            animator.SetBool(boolParameterName, (bool)activator.GetProgramVariable("state"));
        }
    }
}
