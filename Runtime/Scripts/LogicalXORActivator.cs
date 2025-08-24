using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalXORActivator : ActivatorBase
    {
        [Tooltip("At build time null entries in this list will generate errors.\n"
            + "At runtime null entires will get ignored as though they were not in the list at all.\n"
            + "Input Activators tagged as Editor Only get treated as null even at build time, but only "
            + "generate errors if this activator itself is not also Editor Only.")]
        public ActivatorBase[] inputActivators;

        public void OnEvent()
        {
            bool active = false;
            foreach (var activator in inputActivators)
            {
                if (activator == null)
                    continue;
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
