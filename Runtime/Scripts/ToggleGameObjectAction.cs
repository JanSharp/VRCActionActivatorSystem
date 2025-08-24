using UdonSharp;
using UnityEngine;

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class ToggleGameObjectAction : ActionBase
    {
        [Tooltip("If this differs from the Evaluated Initial State of the Activator, "
            + "the given Game Objects will get toggled on Start, aka map load.\n"
            + "Effectively equivalent to the Invert Activator option for Toggle Animation Action "
            + "for example, this merely makes more sense semantically.")]
        [SerializeField] private bool currentStateInTheScene = false;
        [Tooltip("Toggling these objects means they invert their active state, which is to say "
            + "this list can contain a mixture of active and inactive game objects.")]
        [SerializeField] private GameObject[] gameObjects;

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
