using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.NoVariableSync)]
    public class ButtonActivator : ActivatorBase
    {
        public override void Interact()
        {
            SendCustomNetworkEvent(NetworkEventTarget.All, nameof(PressButton));
        }

        public void PressButton()
        {
            State = true;
            State = false;
        }
    }
}
