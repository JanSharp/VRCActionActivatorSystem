using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [AddComponentMenu("")]
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public abstract class ActionBase : UdonSharpBehaviour
    {
        public ActivatorBase activator;
    }
}
