using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioAction : ConfigurableActionBase
    {
        public AudioSource[] audioSources;

        public void OnEvent()
        {
            foreach (var audioSource in audioSources)
                audioSource.Play();
        }
    }
}
