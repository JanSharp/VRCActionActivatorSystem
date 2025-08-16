using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ActionBase : UdonSharpBehaviour
    {
        public ActivatorBase activator;
    }
}
