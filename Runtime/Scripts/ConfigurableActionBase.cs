using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ConfigurableActionBase : ActionBase
    {
        public ListenerType listenerType;
    }

    public enum ListenerType
    {
        OnActivate,
        OnDeactivate,
        OnStateChanged,
    }
}
