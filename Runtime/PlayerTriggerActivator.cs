﻿using UdonSharp;
using UnityEngine;
using VRC.SDKBase;
using VRC.Udon;
using VRC.Udon.Common.Interfaces;
#if UNITY_EDITOR && !COMPILER_UDONSHARP
using UnityEditor;
using UdonSharpEditor;
#endif

namespace JanSharp
{
    [UdonBehaviourSyncMode(BehaviourSyncMode.None)]
    public class PlayerTriggerActivator : ActivatorBase
    {
        private int playerCount;
        private int PlayerCount
        {
            get => playerCount;
            set
            {
                playerCount = value;
                State = value != 0;
            }
        }

        // Syncing the state does not makes sense because it relies on players being inside the trigger zone
        // which should already be the case due to players being synced.
        // If players aren't synced perfectly, then this will be desynced too, and there's no way for
        // this script to accommodate for that. Like even syncing the current state of the boolean
        // whenever someone joins does not guarantee it will stay in sync once players start moving.

        public override void OnPlayerTriggerEnter(VRCPlayerApi player)
        {
            PlayerCount++;
        }

        public override void OnPlayerTriggerExit(VRCPlayerApi player)
        {
            PlayerCount--;
        }
    }

    #if UNITY_EDITOR && !COMPILER_UDONSHARP
    [InitializeOnLoad]
    public static class PlayerTriggerActivatorOnBuild
    {
        static PlayerTriggerActivatorOnBuild() => OnBuildUtil.RegisterType<PlayerTriggerActivator>(ActivatorEditorUtil.ActivatorOnBuildBase, order: 0);
    }
    #endif
}