using UdonSharp;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalORActivator : ActivatorBase
    {
        public ActivatorBase[] inputActivators;

        public void OnEvent()
        {
            foreach (var activator in inputActivators)
            {
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
