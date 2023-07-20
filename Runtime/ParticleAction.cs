using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleAction : ConfigurableActionBase
    {
        public ParticleSystem[] particles;

        public void OnEvent()
        {
            foreach (var particle in particles)
                particle.Play();
        }
    }
}
