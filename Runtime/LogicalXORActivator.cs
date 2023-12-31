﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalXORActivator : ActivatorBase
    {
        public ActivatorBase[] inputActivators;

        public void OnEvent()
        {
            bool active = false;
            foreach (var activator in inputActivators)
            {
                if ((bool)activator.GetProgramVariable("state"))
                {
                    if (active)
                    {
                        State = false;
                        return;
                    }
                    active = true;
                }
            }
            State = active;
        }
    }
}
