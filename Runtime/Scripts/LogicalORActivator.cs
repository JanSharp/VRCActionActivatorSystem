using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalORActivator : ActivatorBase
    {
        [Tooltip("At build time null entries in this list will generate errors.\n"
            + "At runtime null entires will get ignored as though they were not in the list at all.\n"
            + "Input Activators tagged as Editor Only get treated as null even at build time, but only "
            + "generate errors if this activator itself is not also Editor Only.")]
        public ActivatorBase[] inputActivators;

        public void OnEvent()
        {
            foreach (var activator in inputActivators)
            {
                if (activator == null)
                    continue;
                if ((bool)activator.GetProgramVariable("state"))
                {
                    State = true;
                    return;
                }
            }
            State = false;
        }
    }
}
