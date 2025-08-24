using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioAction : ConfigurableActionBase
    {
        public AudioSource[] audioSources;

        public void OnEvent()
        {
            foreach (var audioSource in audioSources)
                if (audioSource != null)
                    audioSource.Play();
        }
    }
}
