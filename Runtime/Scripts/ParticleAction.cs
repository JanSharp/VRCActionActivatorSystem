using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleAction : ConfigurableActionBase
    {
        public ParticleSystem[] particles;

        public void OnEvent()
        {
            foreach (var particle in particles)
                if (particle != null)
                    particle.Play();
        }
    }
}
