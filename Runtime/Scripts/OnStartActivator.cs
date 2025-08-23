using UdonSharp;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class OnStartActivator : ActivatorBase
    {
        public void Start()
        {
            State = true;
            State = false;
        }
    }
}
