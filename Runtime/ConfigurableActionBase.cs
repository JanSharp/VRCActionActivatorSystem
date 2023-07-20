using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ConfigurableActionBase : ActionBase
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
