using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ParticleLoopAction : ActionBase
    {
        public ParticleSystem[] particles;

        public void Start() => OnEvent();

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                foreach (var particle in particles)
                    particle.Play();
            else
                foreach (var particle in particles)
                    particle.Stop();
        }
    }
}
