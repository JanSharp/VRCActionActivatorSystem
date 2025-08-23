using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleGameObjectAction : ActionBase
    {
        [Tooltip("Toggling these objects means they invert their active state, which is to say "
            + "this list can contain a mixture of active and inactive game objects.")]
        [SerializeField] private GameObject[] gameObjects;
        [Tooltip("If this differs from the Evaluated Initial State of the Activator, "
            + "the given Game Objects will get toggled on Start, aka map load.")]
        [SerializeField] private bool currentStateInTheScene = false;

        public void Start()
        {
            if (currentStateInTheScene != (bool)activator.GetProgramVariable("state"))
                OnEvent();
        }

        public void OnEvent()
        {
            foreach (var obj in gameObjects)
                obj.SetActive(!obj.activeSelf);
        }
    }
}
