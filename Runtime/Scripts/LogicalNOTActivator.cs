using UdonSharp;

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
