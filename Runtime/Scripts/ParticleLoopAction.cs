using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleLoopAction : ActionBase
    {
        [Tooltip("Inverts the state of the given Activator. Equivalent to having a Logical NOT Activator "
            + "inserted between the given Activator and this Action.")]
        [SerializeField] private bool invertActivator;
        public ParticleSystem[] particles;

        public void Start() => OnEvent();

        public void OnEvent()
        {
            bool state = activator != null && (bool)activator.GetProgramVariable("state");
            if (invertActivator != state)
                foreach (var particle in particles)
                    particle.Play();
            else
                foreach (var particle in particles)
                    particle.Stop();
        }
    }
}
