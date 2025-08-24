using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ActionBase : UdonSharpBehaviour
    {
        public ActivatorBase activator;
    }
}
