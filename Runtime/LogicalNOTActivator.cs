using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalNOTActivator : ActivatorBase
    {
        public ActivatorBase inputActivator;

        private void Start()
        {
            OnEvent();
        }

        public void OnEvent()
        {
            State = !(bool)inputActivator.GetProgramVariable("state");
        }
    }
}
