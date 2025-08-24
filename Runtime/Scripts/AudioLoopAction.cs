using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioLoopAction : ActionBase
    {
        [Tooltip("Inverts the state of the given Activator. Equivalent to having a Logical NOT Activator "
            + "inserted between the given Activator and this Action.")]
        [SerializeField] private bool invertActivator;
        public AudioSource audioSource;

        public void Start() => OnEvent();

        public void OnEvent()
        {
            if (invertActivator != (bool)activator.GetProgramVariable("state"))
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }
}
