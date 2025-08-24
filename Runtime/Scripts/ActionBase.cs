using UdonSharp;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ActionBase : UdonSharpBehaviour
    {
        public ActivatorBase activator;
    }
}
