using UdonSharp;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class LogicalANDActivator : ActivatorBase
    {
        public ActivatorBase[] inputActivators;

        public void OnEvent()
        {
            foreach (var activator in inputActivators)
            {
                if (!(bool)activator.GetProgramVariable("state"))
                {
                    State = false;
                    return;
                }
            }
            State = true;
        }
    }
}
