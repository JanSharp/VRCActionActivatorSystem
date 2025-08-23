using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioLoopAction : ActionBase
    {
        public AudioSource audioSource;

        public void Start() => OnEvent();

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }
}
