using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ActionBase : UdonSharpBehaviour
    {
        public UdonSharpBehaviour activator;
        public ListenerType listenerType;
    }

    public enum ListenerType
    {
        OnActivate,
        OnDeactivate,
        OnStateChanged,
    }
}
