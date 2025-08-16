using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class AudioLoopAction : ActionBase
    {
        public AudioSource audioSource;

        public void OnEvent()
        {
            if ((bool)activator.GetProgramVariable("state"))
                audioSource.Play();
            else
                audioSource.Stop();
        }
    }
}
